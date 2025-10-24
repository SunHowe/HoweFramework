using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 对接GeekServer的服务器消息包头。
    /// </summary>
    public class GeekServerPacketHeader : IPacketHeader, IReference
    {
        /// <summary>
        /// 消息包头长度。
        /// </summary>
        public const int PacketHeaderLength = sizeof(int) + sizeof(int);

        /// <summary>
        /// 消息包长度。
        /// </summary>
        public int PacketLength { get; set; }

        /// <summary>
        /// 消息ID。
        /// </summary>
        public int MsgId { get; set; }

        /// <summary>
        /// 创建消息包头。
        /// </summary>
        /// <param name="msgId">消息ID。</param>
        /// <param name="packetLength">消息包长度。</param>
        /// <returns>创建的消息包头。</returns>
        public static GeekServerPacketHeader Create(int msgId, int packetLength)
        {
            var header = ReferencePool.Acquire<GeekServerPacketHeader>();
            header.MsgId = msgId;
            header.PacketLength = packetLength;
            return header;
        }

        public void Clear()
        {
            MsgId = 0;
            PacketLength = 0;
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }
    }
}