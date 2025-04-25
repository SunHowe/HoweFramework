using System.Buffers;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using IGrains;
using NetCoreServer;
using Orleans.Streams;
using Protocol;
using ServerProtocol;

namespace GatewayServer;

public class GatewaySession : TcpSession
{
    /// <summary>
    /// 0 表示未处理，1 表示正在处理
    /// </summary>
    private int isProcessing = 0;

    private readonly IClusterClient m_ClusterClient;
    private StreamSubscriptionHandle<ServerPackage>? m_SubscriptionHandle;
    private readonly ConcurrentQueue<ServerPackage> messageQueue = new();
    private readonly SemaphoreSlim queueLock = new(1, 1);
    private bool m_IsConnected;

    public GatewaySession(TcpServer server, IClusterClient clusterClient) : base(server)
    {
        m_ClusterClient = clusterClient;
    }

    protected override void OnConnected()
    {
        Console.WriteLine($"Gateway Session({Id}) connected: RemoteEndPoint={Socket.RemoteEndPoint}, LocalEndPoint={Socket.LocalEndPoint}");
        m_IsConnected = true;
        ConnectGameServer().Wait();
    }

    protected override void OnDisconnected()
    {
        Console.WriteLine($"Gateway Session({Id}) disconnected: RemoteEndPoint={Socket.RemoteEndPoint}, LocalEndPoint={Socket.LocalEndPoint}");
        m_IsConnected = false;
        DisconnectGameServer().Wait();
    }

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        var package = ParseMessage(buffer, offset, size);
        messageQueue.Enqueue(package);
        _ = ProcessQueueAsync().ConfigureAwait(false);
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Gateway Session({Id}) caught an error: {error}, RemoteEndPoint={Socket.RemoteEndPoint}, LocalEndPoint={Socket.LocalEndPoint}");
    }

    private async Task ConnectGameServer()
    {
        var streamProvider = m_ClusterClient.GetStreamProvider(StreamingProviderConst.ProviderName);
        var streamId = StreamId.Create(StreamingProviderConst.StreamId, Id);
        var stream = streamProvider.GetStream<ServerPackage>(streamId);
        m_SubscriptionHandle = await stream.SubscribeAsync(OnReceiveGameServerPackage);

        Console.WriteLine($"Gateway Session({Id}) subscribed to Orleans Stream.");

        var receiverGrain = m_ClusterClient.GetGrain<IReceiverGrain>(Id);
        await receiverGrain.OnGatewayConnected();
    }

    private async Task DisconnectGameServer()
    {
        if (m_SubscriptionHandle != null)
        {
            await m_SubscriptionHandle.UnsubscribeAsync();
            Console.WriteLine($"Gateway Session({Id}) unsubscribed from Orleans Stream.");
            m_SubscriptionHandle = null;
        }
        else
        {
            Console.WriteLine($"Gateway Session({Id}) no active subscription to unsubscribe.");
        }

        var receiverGrain = m_ClusterClient.GetGrain<IReceiverGrain>(Id);
        await receiverGrain.OnGatewayDisconnected();
    }

    private async Task ProcessQueueAsync()
    {
        if (Interlocked.Exchange(ref isProcessing, 1) == 1)
        {
            // 如果已在处理，则直接返回
            return;
        }

        await queueLock.WaitAsync();

        try
        {
            while (messageQueue.TryDequeue(out var message))
            {
                if (!m_IsConnected)
                {
                    messageQueue.Clear();
                    break;
                }

                Console.WriteLine($"Gateway Session({Id}) send to game server: {message.ProtocolId} {message.RpcId}");

                var receiverGrain = m_ClusterClient.GetGrain<IReceiverGrain>(Id);
                await receiverGrain.OnReceive(message);
            }
        }
        finally
        {
            queueLock.Release();

            // 重置标志位以允许下一次处理
            Interlocked.Exchange(ref isProcessing, 0);

            // 再次检查队列是否有新消息，如有则继续处理，确保不会遗漏任何消息
            if (!messageQueue.IsEmpty)
            {
                await ProcessQueueAsync().ConfigureAwait(false);
            }
        }
    }

    private ServerPackage ParseMessage(byte[] buffer, long offset, long size)
    {
        // 确保有足够的数据读取头部
        var headerSize = Marshal.SizeOf<RequestHeader>();
        if (size < headerSize)
        {
            throw new Exception("Invalid protocol size");
        }

        // 读取消息头
        RequestHeader header;
        unsafe
        {
            fixed (byte* pBuffer = buffer)
            {
                header = *(RequestHeader*)(pBuffer + offset);
            }
        }

        // 检查消息长度是否匹配
        if (size != headerSize + header.BodyLength)
        {
            throw new Exception("Protocol length mismatch");
        }

        // 读取消息体.
        var bodySpan = buffer.AsSpan((int)(offset + headerSize), header.BodyLength);
        return GatewayServerPackageHelper.Pack(header, bodySpan.ToArray());
    }

    private Task OnReceiveGameServerPackage(ServerPackage package, StreamSequenceToken token)
    {
        if (!m_IsConnected)
        {
            return Task.CompletedTask;
        }

        GatewayServerPackageHelper.Unpack(package, out var responseHeader, out var bytes);
        Send(responseHeader, bytes);

        return Task.CompletedTask;
    }

    private void Send(in ResponseHeader header, byte[]? body)
    {
        var headerSize = Marshal.SizeOf<ResponseHeader>();
        var size = headerSize + header.BodyLength;

        Console.WriteLine($"Gateway Session({Id}) preparing to send: headerSize={headerSize}, bodySize={header.BodyLength}, totalSize={size}, IsConnected={m_IsConnected}");

        if (!m_IsConnected)
        {
            Console.WriteLine($"Gateway Session({Id}) cannot send: client is not connected");
            return;
        }

        // 使用ArrayPool减少内存分配
        var buffer = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            unsafe
            {
                fixed (byte* pBuffer = buffer)
                {
                    *(ResponseHeader*)pBuffer = header;
                }
            }

            if (body != null)
            {
                Array.Copy(body, 0, buffer, headerSize, header.BodyLength);
            }
            
            var sendSize = Send(buffer, 0, size);
            Console.WriteLine($"Gateway Session({Id}) send to client: header={header}, sendSize={sendSize}, expectedSize={size}, IsConnected={m_IsConnected}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Gateway Session({Id}) send error: {ex}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}