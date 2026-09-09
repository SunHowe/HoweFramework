using System;
using System.IO;

namespace HoweFramework
{
    internal sealed partial class NetworkManager : INetworkManager
    {
        private sealed class ReceiveState : IDisposable
        {
            private const int DefaultBufferLength = 1024 * 64;

            /// <summary>
            /// 消息包长度上限（64MB），防止恶意或损坏的长度字段导致超大内存分配（OOM）。
            /// </summary>
            private const int MaxPacketLength = 1024 * 1024 * 64;

            private MemoryStream m_Stream;
            private IPacketHeader m_PacketHeader;
            private bool m_Disposed;

            public ReceiveState()
            {
                m_Stream = new MemoryStream(DefaultBufferLength);
                m_PacketHeader = null;
                m_Disposed = false;
            }

            public MemoryStream Stream
            {
                get
                {
                    return m_Stream;
                }
            }

            public IPacketHeader PacketHeader
            {
                get
                {
                    return m_PacketHeader;
                }
            }

            public void PrepareForPacketHeader(int packetHeaderLength)
            {
                Reset(packetHeaderLength, null);
            }

            public void PrepareForPacket(IPacketHeader packetHeader)
            {
                if (packetHeader == null)
                {
                    throw new ErrorCodeException(FrameworkErrorCode.InvalidParam, "Packet header is invalid.");
                }

                Reset(packetHeader.PacketLength, packetHeader);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (m_Disposed)
                {
                    return;
                }

                if (disposing)
                {
                    if (m_Stream != null)
                    {
                        m_Stream.Dispose();
                        m_Stream = null;
                    }
                }

                m_Disposed = true;
            }

            private void Reset(int targetLength, IPacketHeader packetHeader)
            {
                if (targetLength < 0 || targetLength > MaxPacketLength)
                {
                    throw new ErrorCodeException(FrameworkErrorCode.InvalidParam, $"Target length '{targetLength}' is invalid.");
                }

                m_Stream.Position = 0L;
                m_Stream.SetLength(targetLength);
                m_PacketHeader = packetHeader;
            }
        }
    }
}
