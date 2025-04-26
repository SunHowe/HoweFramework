using HoweFramework;
using Protocol;

namespace GameMain
{
    /// <summary>
    /// Orleans协议包。这里为了保证前后端协议统一，包了一层。
    /// </summary>
    public abstract class OrleansPacket : Packet
    {
        /// <summary>
        /// 协议Id。
        /// </summary>
        public override int Id => Protocol?.Id ?? 0;

        /// <summary>
        /// 真正的协议实例。
        /// </summary>
        public IProtocol Protocol { get; set; }

        public override void Clear()
        {
            if (Protocol != null)
            {
                ReferencePool.Release(Protocol);
                Protocol = null;
            }
        }
    }
    
    /// <summary>
    /// Orleans请求包。
    /// </summary>
    public sealed class OrleansRequestPacket : OrleansPacket, IRemoteRequest
    {
        /// <summary>
        /// 请求Id。
        /// </summary>
        public int RequestId { get; set; }

        public override void Clear()
        {
            base.Clear();
            RequestId = 0;
        }

        /// <summary>
        /// 创建Orleans请求包。
        /// </summary>
        /// <param name="requestId">请求Id。</param>
        /// <param name="protocol">真正的协议实例。</param>
        /// <returns>Orleans请求包。</returns>
        public static OrleansRequestPacket Create(int requestId, IProtocol protocol)
        {
            var packet = ReferencePool.Acquire<OrleansRequestPacket>();
            packet.RequestId = requestId;
            packet.Protocol = protocol;
            return packet;
        }
    }

    /// <summary>
    /// Orleans响应包。
    /// </summary>
    public sealed class OrleansResponsePacket : OrleansPacket, IRemoteResponse
    {
        /// <summary>
        /// 请求Id。
        /// </summary>
        public int RequestId { get; set; }

        /// <summary>
        /// 实际的响应包实例。
        /// </summary>
        public ResponseBase Response { get; set; }

        public override void Clear()
        {
            base.Clear();
            RequestId = 0;
            if (Response != null)
            {
                ReferencePool.Release(Response);
                Response = null;
            }
        }

        /// <summary>
        /// 声明外部已获取响应包，防止被意外回收。
        /// </summary>
        public void PickResponse()
        {
            Response = null;
        }

        /// <summary>
        /// 创建Orleans响应包。
        /// </summary>
        /// <param name="requestId">请求Id。</param>
        /// <param name="response">实际的响应包实例。</param>
        /// <returns>Orleans响应包。</returns>
        public static OrleansResponsePacket Create(int requestId, IProtocol protocol)
        {
            var packet = ReferencePool.Acquire<OrleansResponsePacket>();
            packet.RequestId = requestId;
            packet.Protocol = protocol;
            return packet;
        }
    }

    /// <summary>
    /// Orleans通知包。
    /// </summary>
    public sealed class OrleansNotifyPacket : OrleansPacket
    {
        /// <summary>
        /// 创建Orleans通知包。
        /// </summary>
        /// <param name="protocol">真正的协议实例。</param>
        /// <returns>Orleans通知包。</returns>
        public static OrleansNotifyPacket Create(IProtocol protocol)
        {
            var packet = ReferencePool.Acquire<OrleansNotifyPacket>();
            packet.Protocol = protocol;
            return packet;
        }
    }
}
