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
                customErrorData = new ErrorCodeException(FrameworkErrorCode.NetworkDeserializePacketError, "Protocol id is invalid.");
                return null;
            }

            ProtocolBase protocol;

            if (header.PacketLength <= 0)
            {
                // 允许包体为空。
                protocol = ReferencePool.Acquire(messageType) as ProtocolBase;
                if (protocol == null)
                {
                    customErrorData = new ErrorCodeException(FrameworkErrorCode.NetworkDeserializePacketError, "Protocol id is invalid.");
                    return null;
                }
            }
            else
            {
                // 读取包体数据到缓冲区。
                if (source.Read(m_PacketBodyBuffer, 0, header.PacketLength) != header.PacketLength)
                {
                    customErrorData = new ErrorCodeException(FrameworkErrorCode.NetworkDeserializePacketHeaderError, "Packet header length is invalid.");
                    return null;
                }

                protocol = ReferencePool.Acquire(messageType) as ProtocolBase;
                if (protocol == null)
                {
                    customErrorData = new ErrorCodeException(FrameworkErrorCode.NetworkDeserializePacketError, "Protocol id is invalid.");
                    return null;
                }

                object obj = protocol;

                ReadOnlySpan<byte> span = m_PacketBodyBuffer.AsSpan(0, header.PacketLength);
                MemoryPackSerializerOptions options = null;
                MemoryPackSerializer.Deserialize(messageType, span, ref obj, options);

                protocol = obj as ProtocolBase;
                if (protocol == null)
                {
                    customErrorData = new ErrorCodeException(FrameworkErrorCode.NetworkDeserializePacketError, "Protocol id is invalid.");
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
                customErrorData = new ErrorCodeException(FrameworkErrorCode.NetworkDeserializePacketHeaderError, "Packet header length is invalid.");
                return null;
            }

            customErrorData = null;

            if (!ProtocolHeaderHelper.TryDeserialize(m_PacketHeaderBuffer.AsSpan(), out var header))
            {
                customErrorData = new ErrorCodeException(FrameworkErrorCode.NetworkDeserializePacketHeaderError, "Packet header is invalid.");
                return null;
            }

            return OrleansPacketHeader.Create(header);
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
                throw new ErrorCodeException(FrameworkErrorCode.NetworkSerializeError, "Only accept IRemoteRequest Packet.");
            }

            // 序列化包体内容。
            var bytes = MemoryPackSerializer.Serialize(packet.GetType(), packet);
            if (bytes.Length > OrleansPacketHeader.PacketBodyLengthLimit)
            {
                throw new ErrorCodeException(FrameworkErrorCode.NetworkSerializeError, "Packet body length is too long.");
            }

            var packetId = (ushort)packet.Id;
            var bodyLength = (ushort)bytes.Length;

            // 计算魔法数字。
            var magicNumber = 0x12345678 ^ packetId ^ remoteRequest.RequestId;
            var header = new ProtocolHeader
            {
                ProtocolId = packetId,
                BodyLength = bodyLength,
                RpcId = remoteRequest.RequestId,
                Param = magicNumber,
            };
            
            // 写入header。
            ProtocolHeaderHelper.Serialize(header, destination);

            // 写入包体内容。
            destination.Write(bytes, 0, bytes.Length);

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