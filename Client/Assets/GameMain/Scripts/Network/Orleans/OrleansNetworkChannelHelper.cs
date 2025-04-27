using System;
using System.IO;
using HoweFramework;
using MemoryPack;
using Protocol;

namespace GameMain
{
    /// <summary>
    /// 基于Orleans的网络通道辅助类。
    /// </summary>
    public sealed class OrleansNetworkChannelHelper : NetworkChannelHelperBase
    {
        /// <summary>
        /// 包头缓冲区。
        /// </summary>
        private readonly byte[] m_PacketHeaderBuffer = new byte[OrleansPacketHeader.PacketHeaderLength];

        /// <summary>
        /// 包体缓冲区。
        /// </summary>
        private readonly byte[] m_PacketBodyBuffer = new byte[OrleansPacketHeader.PacketBodyLengthLimit];

        /// <summary>
        /// 包头长度。参考RequestHeader和ResponseHeader
        /// </summary>
        public override int PacketHeaderLength => OrleansPacketHeader.PacketHeaderLength;

        /// <summary>
        /// 反序列化包。
        /// </summary>
        /// <param name="packetHeader">包头。</param>
        /// <param name="source">数据流。</param>
        /// <param name="customErrorData">自定义错误数据。</param>
        /// <returns>包。</returns>
        public override Packet DeserializePacket(IPacketHeader packetHeader, Stream source, out object customErrorData)
        {
            var header = (OrleansPacketHeader)packetHeader;
            
            var messageType = ProtocolBinder.GetProtocolType(header.ProtocolId);
            if (messageType == null)
            {
                customErrorData = new ErrorCodeException(ErrorCode.NetworkDeserializePacketError, "Protocol id is invalid.");
                return null;
            }

            ProtocolBase protocol;

            if (header.PacketLength <= 0)
            {
                // 允许包体为空。
                protocol = ReferencePool.Acquire(messageType) as ProtocolBase;
                if (protocol == null)
                {
                    customErrorData = new ErrorCodeException(ErrorCode.NetworkDeserializePacketError, "Protocol id is invalid.");
                    return null;
                }
            }
            else
            {
                // 读取包体数据到缓冲区。
                if (source.Read(m_PacketBodyBuffer, 0, header.PacketLength) != PacketHeaderLength)
                {
                    customErrorData = new ErrorCodeException(ErrorCode.NetworkDeserializePacketHeaderError, "Packet header length is invalid.");
                    return null;
                }

                protocol = ReferencePool.Acquire(messageType) as ProtocolBase;
                if (protocol == null)
                {
                    customErrorData = new ErrorCodeException(ErrorCode.NetworkDeserializePacketError, "Protocol id is invalid.");
                    return null;
                }

                object obj = protocol;

                ReadOnlySpan<byte> span = m_PacketBodyBuffer.AsSpan(0, header.PacketLength);
                MemoryPackSerializerOptions options = null;
                MemoryPackSerializer.Deserialize(messageType, span, ref obj, options);

                protocol = obj as ProtocolBase;
                if (protocol == null)
                {
                    customErrorData = new ErrorCodeException(ErrorCode.NetworkDeserializePacketError, "Protocol id is invalid.");
                    return null;
                }
            }

            customErrorData = null;

            if (protocol is IProtocolResponse response)
            {
                response.ErrorCode = header.ErrorCode;
                response.RequestId = header.RpcId;
            }

            return protocol;
        }

        /// <summary>
        /// 反序列化包头。
        /// </summary>
        /// <param name="source">数据流。</param>
        /// <param name="customErrorData">自定义错误数据。</param>
        /// <returns>包头。</returns>
        public override IPacketHeader DeserializePacketHeader(Stream source, out object customErrorData)
        {
            if (source.Read(m_PacketHeaderBuffer, 0, PacketHeaderLength) != PacketHeaderLength)
            {
                customErrorData = new ErrorCodeException(ErrorCode.NetworkDeserializePacketHeaderError, "Packet header length is invalid.");
                return null;
            }

            customErrorData = null;

            var header = ReferencePool.Acquire<OrleansPacketHeader>();
            header.ProtocolId = BitConverter.ToUInt16(m_PacketHeaderBuffer, 0);
            header.BodyLength = BitConverter.ToUInt16(m_PacketHeaderBuffer, 2);
            header.RpcId = BitConverter.ToInt32(m_PacketHeaderBuffer, 4);
            header.ErrorCode = BitConverter.ToInt32(m_PacketHeaderBuffer, 8);
            return header;
        }

        public override void PrepareForConnecting()
        {
        }

        /// <summary>
        /// 发送心跳包。
        /// </summary>
        public override bool SendHeartBeat()
        {
            // m_NetworkChannel.Send(OrleansRequestPacket.Create(0, ReferencePool.Acquire<Heartbeat>()));
            return true;
        }

        /// <summary>
        /// 序列化包体信息。
        /// </summary>
        /// <typeparam name="T">包类型。</typeparam>
        /// <param name="packet">包。</param>
        /// <param name="destination">目标流。</param>
        /// <returns>是否序列化成功。</returns>
        public override bool Serialize<T>(T packet, Stream destination)
        {
            if (packet is not IRemoteRequest remoteRequest)
            {
                throw new ErrorCodeException(ErrorCode.NetworkSerializeError, "Only accept IRemoteRequest Packet.");
            }

            // 写入协议id(ushort)。
            destination.Write(BitConverter.GetBytes(packet.Id), 0, 2);

            // 偏移包体长度(ushort)。
            var seekPosition = destination.Position;
            destination.Seek(2, SeekOrigin.Current);

            // 写入请求id(int)。
            destination.Write(BitConverter.GetBytes(remoteRequest.RequestId), 0, 4);

            // 写入魔法数字(int)。
            var magicNumber = 0x12345678 ^ packet.Id ^ remoteRequest.RequestId;
            destination.Write(BitConverter.GetBytes(magicNumber), 0, 4);

            // 序列化包体内容。
            var bytes = MemoryPackSerializer.Serialize(packet.GetType(), packet);
            destination.Write(bytes, 0, bytes.Length);
            var endPosition = destination.Position;

            // 偏移回包体长度写入位置，写入包体长度。
            destination.Seek(seekPosition, SeekOrigin.Begin);
            destination.Write(BitConverter.GetBytes(bytes.Length), 0, 2);

            // 偏移回结尾位置。
            destination.Seek(endPosition, SeekOrigin.Begin);

            return true;
        }

        protected override void OnInitialize()
        {
        }

        protected override void OnDispose()
        {
        }
    }
}