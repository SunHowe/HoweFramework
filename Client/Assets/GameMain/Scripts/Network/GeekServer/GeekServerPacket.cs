using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 对接GeekServer的服务器消息包。
    /// </summary>
    public class GeekServerPacket : Packet
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

        public override string ToString()
        {
            return Message != null ? JsonUtility.ToJson(Message) : string.Empty;
        }
    }
}