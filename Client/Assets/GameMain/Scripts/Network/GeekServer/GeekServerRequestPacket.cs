using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// GeekServer请求协议包。
    /// </summary>
    public sealed class GeekServerRequestPacket : GeekServerPacket, IRemoteRequest
    {
        /// <summary>
        /// 请求id。
        /// </summary>
        public int RequestId
        {
            get => Message?.UniId ?? 0;
            set
            {
                if (Message != null)
                {
                    Message.UniId = value;
                }
            }
        }
    }
}