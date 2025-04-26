using HoweFramework;
using Protocol;

namespace HoweFramework
{
    /// <summary>
    /// Orleans协议包。这里为了保证前后端协议统一，包了一层。
    /// </summary>
    public sealed class OrleansPacket : Packet
    {
        /// <summary>
        /// 协议Id。
        /// </summary>
        public override int Id => Protocol?.Id ?? 0;

        /// <summary>
        /// 真正的协议实例。
        /// </summary>
        public IProtocol Protocol { get; set; }

        /// <summary>
        /// 请求Id。
        /// </summary>
        public int RpcId { get; set; }

        public override void Clear()
        {
            if (Protocol != null)
            {
                ReferencePool.Release(Protocol);
                Protocol = null;
            }

            RpcId = 0;
        }

        /// <summary>
        /// 创建Orleans协议包。
        /// </summary>
        public static OrleansPacket Create(int rpcId, IProtocol protocol)
        {
            var packet = ReferencePool.Acquire<OrleansPacket>();
            packet.RpcId = rpcId;
            packet.Protocol = protocol;
            return packet;
        }
    }
}
