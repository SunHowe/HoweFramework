using System;
using System.IO;
using HoweFramework;
using Protocol;

namespace GameMain
{
    /// <summary>
    /// 基于Orleans的网络通道辅助类。
    /// </summary>
    public sealed class OrleansNetworkChannelHelper : INetworkChannelHelper
    {
        /// <summary>
        /// 包头缓冲区。
        /// </summary>
        private readonly byte[] m_PacketHeaderBuffer = new byte[OrleansPacketHeader.PacketHeaderLength];

        /// <summary>
        /// 包头长度。参考RequestHeader和ResponseHeader
        /// </summary>
        public int PacketHeaderLength => OrleansPacketHeader.PacketHeaderLength;

        /// <summary>
        /// 反序列化包。
        /// </summary>
        /// <param name="packetHeader">包头。</param>
        /// <param name="source">数据流。</param>
        /// <param name="customErrorData">自定义错误数据。</param>
        /// <returns>包。</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Packet DeserializePacket(IPacketHeader packetHeader, Stream source, out object customErrorData)
        {
            var header = (OrleansPacketHeader)packetHeader;

            var messageType = ProtocolBinder.GetProtocolType(header.ProtocolId);
            if (messageType == null)
            {
                customErrorData = new ErrorCodeException(ErrorCode.NetworkDeserializePacketError, "Protocol id is invalid.");
                return null;
            }

            if (header.PacketLength <= 0)
            {

            }

            customErrorData = null;

            return null;
        }
        
        /// <summary>
        /// 反序列化包头。
        /// </summary>
        /// <param name="source">数据流。</param>
        /// <param name="customErrorData">自定义错误数据。</param>
        /// <returns>包头。</returns>
        public IPacketHeader DeserializePacketHeader(Stream source, out object customErrorData)
        {
            if (source.Read(m_PacketHeaderBuffer, 0, PacketHeaderLength) != PacketHeaderLength)
            {
                customErrorData = new ErrorCodeException(ErrorCode.NetworkDeserializePacketHeaderError, "Packet header length is invalid.");
                return null;
            }

            customErrorData = null;

            var header = ReferencePool.Acquire<OrleansPacketHeader>();
            header.ProtocolId = BitConverter.ToUInt16(m_PacketHeaderBuffer, 0);
            header.PacketLength = BitConverter.ToUInt16(m_PacketHeaderBuffer, 2);
            header.RpcId = BitConverter.ToInt32(m_PacketHeaderBuffer, 4);
            header.ErrorCode = BitConverter.ToInt32(m_PacketHeaderBuffer, 8);
            return header;
        }

        public void Initialize(INetworkChannel networkChannel)
        {
            throw new System.NotImplementedException();
        }

        public void PrepareForConnecting()
        {
            throw new System.NotImplementedException();
        }

        public bool SendHeartBeat()
        {
            throw new System.NotImplementedException();
        }

        public bool Serialize<T>(T packet, Stream destination) where T : Packet
        {
            throw new System.NotImplementedException();
        }

        public void Shutdown()
        {
            throw new System.NotImplementedException();
        }
    }
}