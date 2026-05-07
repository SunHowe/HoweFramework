using HoweFramework;
using Protocol;

namespace GameMain
{
    /// <summary>
    /// 基于Orleans的协议包头。
    /// </summary>
    public class OrleansPacketHeader : IPacketHeader, IReference
    {
        public const int PacketHeaderLength = ProtocolHeaderHelper.PacketHeaderLength;

        public const int PacketBodyLengthLimit = ProtocolHeaderHelper.PacketBodyLengthLimit;

        /// <summary>
        /// 包体长度。
        /// </summary>
        public int PacketLength => m_Header.BodyLength;

        /// <summary>
        /// 协议ID。
        /// </summary>
        public ushort ProtocolId 
        {
            get => m_Header.ProtocolId;
            set => m_Header.ProtocolId = value;
        }

        /// <summary>
        /// 请求ID。
        /// </summary>
        public int RpcId => m_Header.RpcId;
        
        /// <summary>
        /// 错误码。
        /// </summary>
        public int ErrorCode => m_Header.Param;

        private ProtocolHeader m_Header;

        public void Clear()
        {
            m_Header = default;
        }

        /// <summary>
        /// 创建协议头。
        /// </summary>
        /// <param name="header">协议头。</param>
        public static OrleansPacketHeader Create(in ProtocolHeader header)
        {
            var instance = ReferencePool.Acquire<OrleansPacketHeader>();
            instance.m_Header = header;
            return instance;
        }
    }
}
