using System;
using Protocol;
using ServerProtocol;

namespace GatewayServer;

/// <summary>
/// 网关接收状态, 用于处理接收到的数据.
/// </summary>
public class GatewayReceiveState : IDisposable
{
    private enum State : byte
    {
        Header = 0,
        Body = 1,
    }

    private bool m_IsDisposed;
    private readonly Action<ServerPackage> m_OnReceived;

    private State m_State = State.Header;
    private byte[] m_ReceivedBuffer = new byte[8192];
    private int m_ReceivedBufferIndex = 0;
    private const int m_ReceivedBufferCapacityLimit = 1024 * 1024 * 10;
    private ProtocolHeader m_ProtocolHeader = new();

    public GatewayReceiveState(Action<ServerPackage> onReceived)
    {
        m_OnReceived = onReceived;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 重置接收状态.
    /// </summary>
    public void Reset()
    {
        m_State = State.Header;
        m_ReceivedBufferIndex = 0;
        m_ProtocolHeader = new();
    }

    /// <summary>
    /// 处理接收到的数据.
    /// </summary>
    /// <param name="buffer">接收到的数据缓冲数据。</param>
    /// <param name="offset">数据的偏移量。</param>
    /// <param name="size">数据的大小。</param>
    public void Handle(byte[] buffer, long offset, long size)
    {
        if (size <= 0)
        {
            return;
        }

        while (size > 0)
        {
            int handledSize = 0;
            switch (m_State)
            {
                case State.Header:
                    handledSize = HandleHeader(buffer, offset, size);
                    break;
                case State.Body:
                    handledSize = HandleBody(buffer, offset, size);
                    break;
            }

            if (handledSize < 0)
            {
                // 处理失败，直接退出。
                Reset();
                break;
            }

            offset += handledSize;
            size -= handledSize;
        }
    }

    /// <summary>
    /// 处理头部数据.
    /// </summary>
    /// <param name="buffer">接收到的数据缓冲数据。</param>
    /// <param name="offset">数据的偏移量。</param>
    /// <param name="size">数据的大小。</param>
    private int HandleHeader(byte[] buffer, long offset, long size)
    {
        var headerSize = ProtocolHeaderHelper.HeaderLength;

        if (m_ReceivedBufferIndex > 0)
        {
            // 有一部分头数据在缓冲区里，需要先写入到缓冲区中。
            var remainingSize = headerSize - m_ReceivedBufferIndex;
            if (size < remainingSize)
            {
                // 剩余空间不足，需要先写入到缓冲区中。
                Array.Copy(buffer, offset, m_ReceivedBuffer, m_ReceivedBufferIndex, size);
                m_ReceivedBufferIndex += (int)size;
                return (int)size;
            }

            // 剩余空间足够，可以一次性读取。
            Array.Copy(buffer, offset, m_ReceivedBuffer, m_ReceivedBufferIndex, remainingSize);
            m_ReceivedBufferIndex += (int)remainingSize;

            // 尝试反序列化头部数据。
            if (ProtocolHeaderHelper.TryDeserialize(m_ReceivedBuffer.AsSpan(0, m_ReceivedBufferIndex), out m_ProtocolHeader))
            {
                m_State = State.Body;
                // 清空缓冲区。
                m_ReceivedBufferIndex = 0;
            }

            return remainingSize;
        }
        else if (size < headerSize)
        {
            // 剩余空间不足，需要先写入到缓冲区中。
            Array.Copy(buffer, offset, m_ReceivedBuffer, m_ReceivedBufferIndex, size);
            m_ReceivedBufferIndex += (int)size;
            return (int)size;
        }
        else
        {
            // 直接反序列化头部数据。
            if (offset < int.MaxValue)
            {
                if (ProtocolHeaderHelper.TryDeserialize(buffer.AsSpan((int)offset, headerSize), out m_ProtocolHeader))
                {
                    m_State = State.Body;
                }
            }
            else
            {
                // 超过int范围，先拷贝到缓冲区。
                Array.Copy(buffer, offset, m_ReceivedBuffer, m_ReceivedBufferIndex, headerSize);

                if (ProtocolHeaderHelper.TryDeserialize(m_ReceivedBuffer.AsSpan(0, headerSize), out m_ProtocolHeader))
                {
                    m_State = State.Body;
                }
            }

            return headerSize;
        }
    }

    /// <summary>
    /// 处理body数据.
    /// </summary>
    /// <param name="buffer">接收到的数据缓冲数据。</param>
    /// <param name="offset">数据的偏移量。</param>
    /// <param name="size">数据的大小。</param>
    private int HandleBody(byte[] buffer, long offset, long size)
    {
        var bodySize = m_ProtocolHeader.BodyLength;
        if (bodySize <= 0)
        {
            // 包体长度为0，直接返回。
            m_State = State.Header;
            return 0;
        }

        if (m_ReceivedBufferIndex > 0)
        {
            var remainingSize = bodySize - m_ReceivedBufferIndex;
            if (size < remainingSize)
            {
                // 剩余空间不足，需要先写入到缓冲区中。
                var iSize = (int)size;
                EnsureReceivedBufferCapacity(iSize);
                Array.Copy(buffer, offset, m_ReceivedBuffer, m_ReceivedBufferIndex, iSize);
                m_ReceivedBufferIndex += iSize;
                return iSize;
            }

            // 剩余空间足够，可以一次性读取。
            EnsureReceivedBufferCapacity(remainingSize);
            Array.Copy(buffer, offset, m_ReceivedBuffer, m_ReceivedBufferIndex, remainingSize);
            m_ReceivedBufferIndex += remainingSize;
            var bodySpan = m_ReceivedBuffer.AsSpan(0, m_ReceivedBufferIndex);
            var serverPackage = GatewayServerPackageHelper.Pack(m_ProtocolHeader, bodySpan.ToArray());
            m_OnReceived(serverPackage);
            
            Reset();
            return remainingSize;
        }
        else if (size < bodySize)
        {
            // 剩余空间不足，需要先写入到缓冲区中。
            var iSize = (int)size;
            EnsureReceivedBufferCapacity(iSize);
            Array.Copy(buffer, offset, m_ReceivedBuffer, m_ReceivedBufferIndex, iSize);
            m_ReceivedBufferIndex += iSize;
            return iSize;
        }
        else
        {
            // 直接反序列化包体数据。
            if (offset < int.MaxValue)
            {
                var serverPackage = GatewayServerPackageHelper.Pack(m_ProtocolHeader, buffer.AsSpan((int)offset, bodySize).ToArray());
                m_OnReceived(serverPackage);
            }
            else
            {
                // 超过int范围，先拷贝到缓冲区。
                EnsureReceivedBufferCapacity(bodySize);
                Array.Copy(buffer, offset, m_ReceivedBuffer, m_ReceivedBufferIndex, bodySize);
                m_ReceivedBufferIndex += bodySize;
                var bodySpan = m_ReceivedBuffer.AsSpan(0, m_ReceivedBufferIndex);
                var serverPackage = GatewayServerPackageHelper.Pack(m_ProtocolHeader, bodySpan.ToArray());
                m_OnReceived(serverPackage);
            }

            Reset();

            return bodySize;
        }
    }

    /// <summary>
    /// 确保接收缓冲区有足够的容量.
    /// </summary>
    private void EnsureReceivedBufferCapacity(int size)
    {
        var requiredSize = m_ReceivedBufferIndex + size;
        if (requiredSize <= m_ReceivedBuffer.Length)
        {
            return;
        }

        var newCapacity = m_ReceivedBuffer.Length * 2;
        while (newCapacity < requiredSize)
        {
            newCapacity *= 2;
        }

        if (newCapacity > m_ReceivedBufferCapacityLimit)
        {
            throw new Exception($"Received buffer capacity limit exceeded: {newCapacity}");
        }

        Array.Resize(ref m_ReceivedBuffer, newCapacity);
    }

    private void Dispose(bool disposing)
    {
        if (m_IsDisposed)
        {
            return;
        }

        m_IsDisposed = true;

        if (disposing)
        {
        }
    }
}