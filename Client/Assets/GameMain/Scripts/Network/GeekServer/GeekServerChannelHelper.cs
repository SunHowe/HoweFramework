using System;
using System.IO;
using HoweFramework;
using MessagePack;

namespace GameMain
{
    /// <summary>
    /// 对接GeekServer的服务器网络频道辅助器。
    /// </summary>
    public class GeekServerChannelHelper : NetworkChannelHelperBase
    {
        private const int Magic = 0x1234;
        int m_SendCount = 0;

        public override int PacketHeaderLength => GeekServerPacketHeader.PacketHeaderLength;

        /// <summary>
        /// 反序列化消息包。
        /// </summary>
        /// <param name="packetHeader">消息包头。</param>
        /// <param name="source">消息包源。</param>
        /// <param name="customErrorData">自定义错误数据。</param>
        /// <returns>反序列化后的消息包。</returns>
        public override Packet DeserializePacket(IPacketHeader packetHeader, Stream source, out object customErrorData)
        {
            var header = (GeekServerPacketHeader)packetHeader;
            var msgId = header.MsgId;
            var msg = MessagePackSerializer.Deserialize<Message>(source);
            if (msg.MsgId != msgId)
            {
                customErrorData = new ErrorCodeException(ErrorCode.NetworkDeserializePacketError, $"消息ID不匹配: {msgId} != {msg.MsgId}");
                return null;
            }

            customErrorData = null;
            return GeekServerPacket.Create(msg);
        }

        /// <summary>
        /// 反序列化消息包头。
        /// </summary>
        /// <param name="source">消息包头源。</param>
        /// <param name="customErrorData">自定义错误数据。</param>
        /// <returns>反序列化后的消息包头。</returns>
        public override IPacketHeader DeserializePacketHeader(Stream source, out object customErrorData)
        {
            var packetLength = source.ReadInt32();
            var msgId = source.ReadInt32();
            customErrorData = null;
            return GeekServerPacketHeader.Create(msgId, packetLength);
        }

        public override void PrepareForConnecting()
        {
        }

        public override bool SendHeartBeat()
        {
            return true;
        }

        public override bool Serialize<T>(T packet, Stream destination)
        {
            if (packet is not GeekServerPacket messagePacket)
            {
                return false;
            }

            if (messagePacket.Message == null)
            {
                return false;
            }

            const int headerLength = sizeof(int) + sizeof(long) + sizeof(int) + sizeof(int);

            var cachePosition = destination.Position;
            var cacheBufferPosition = cachePosition + headerLength;

            destination.Position = cacheBufferPosition;
            MessagePackSerializer.Serialize(destination, messagePacket.Message);
            var cacheBufferEndPosition = destination.Position;

            var packetLength = (int)(cacheBufferEndPosition - cacheBufferPosition);

            destination.Position = cachePosition;

            // packetLength
            destination.WriteInt32(packetLength);

            // unixTime
            destination.WriteInt64(DateTime.UtcNow.Ticks);

            // magic number
            var encodeMagicNumber = Magic + ++m_SendCount;
            encodeMagicNumber ^= Magic << 8;
            encodeMagicNumber ^= packetLength;
            destination.WriteInt32(encodeMagicNumber);

            // msgId
            destination.WriteInt32(messagePacket.Message.MsgId);
            destination.Position = cacheBufferEndPosition;
            return true;
        }

        protected override void OnDispose()
        {
        }

        protected override void OnInitialize()
        {
        }
    }
}