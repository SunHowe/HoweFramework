using IGrains;
using Orleans.Streams;
using Protocol;
using ServerProtocol;

namespace Grains;

/// <summary>
/// 会话Grain, 用于游戏服务器发送消息给客户端.
/// </summary>
public class SessionGrain : Grain, ISessionGrain
{
    private IAsyncStream<ServerPackage> m_Stream;

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        base.OnActivateAsync(cancellationToken);

        var streamProvider = this.GetStreamProvider(StreamingProviderConst.ProviderName);
        var streamId = StreamId.Create(StreamingProviderConst.StreamGatewaySession, this.GetPrimaryKey());
        m_Stream = streamProvider.GetStream<ServerPackage>(streamId);

        return Task.CompletedTask;
    }

    public async Task Send(ServerPackage package)
    {
        try
        {
            await m_Stream.OnNextAsync(package);
        }
        catch (Exception e)
        {
            Console.WriteLine("SessionGrain Send Error: " + e.Message);
        }
    }
}

/// <summary>
/// SessionGrain扩展方法
/// </summary>
public static class SessionGrainExtensions
{
    /// <summary>
    /// 发送响应包给客户端。
    /// </summary>
    public static async Task SendResponse(this ISessionGrain sessionGrain, int rpcId, int errorCode)
    {
        var serverPackage = new ServerPackage
        {
            RpcId = rpcId,
            ErrorCode = errorCode,
            ProtocolId = (ushort)ProtocolId.CommonResp,
        };

        await sessionGrain.Send(serverPackage);
    }

    /// <summary>
    /// 发送响应包给客户端。
    /// </summary>
    public static async Task SendResponse(this ISessionGrain sessionGrain, int rpcId, IProtocolResponse response)
    {
        var serverPackage = ServerPackageHelper.Pack(response);
        serverPackage.RpcId = rpcId;
        serverPackage.ErrorCode = response.ErrorCode;

        await sessionGrain.Send(serverPackage);
    }
}