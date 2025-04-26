using HoweFramework;

namespace HoweFramework
{
    /// <summary>
    /// 基于Orleans的协议包头。
    /// </summary>
    public class OrleansPacketHeader : IPacketHeader, IReference
    {
        public const int PacketHeaderLength = 12;

        public const int PacketBodyLengthLimit = 65536;

        public int PacketLength => BodyLength;

        /// <summary>
        /// 协议ID。
        /// </summary>
        public ushort ProtocolId { get; set; }

        /// <summary>
        /// 协议包体长度。
        /// </summary>
        public ushort BodyLength { get; set; }

        /// <summary>
        /// 请求ID。
        /// </summary>
        public int RpcId { get; set; }

        /// <summary>
        /// 错误码。
        /// </summary>
        public int ErrorCode { get; set; }

        public void Clear()
        {
            BodyLength = 0;
            ProtocolId = 0;
            RpcId = 0;
            ErrorCode = 0;
        }
    }
}
