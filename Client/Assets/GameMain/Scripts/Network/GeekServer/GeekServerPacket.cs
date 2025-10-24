using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 对接GeekServer的服务器消息包。
    /// </summary>
    public sealed class GeekServerPacket : Packet
    {
        public override int Id => Message.MsgId;

        /// <summary>
        /// 实际的协议包实例。
        /// </summary>
        public Message Message { get; set; }

        public override void Clear()
        {
            base.Clear();

            Message = null;
        }

        /// <summary>
        /// 创建消息包。
        /// </summary>
        /// <param name="message">实际的协议包实例。</param>
        /// <returns>创建的消息包。</returns>
        /// <returns></returns>
        public static GeekServerPacket Create(Message message)
        {
            var packet = ReferencePool.Acquire<GeekServerPacket>();
            packet.Message = message;
            return packet;
        }
    }
}