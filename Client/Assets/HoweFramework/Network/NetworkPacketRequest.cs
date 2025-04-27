using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 网络协议交互请求。
    /// </summary>
    public sealed class NetworkPacketRequest : RequestBase
    {
        /// <summary>
        /// 协议包实例。
        /// </summary>
        public Packet Packet { get; set; }

        /// <summary>
        /// 网络频道。
        /// </summary>
        public INetworkChannel NetworkChannel { get; set; }

        protected override UniTask<IResponse> OnExecute(CancellationToken token)
        {
            if (NetworkChannel == null)
            {
                throw new ErrorCodeException(ErrorCode.NetworkChannelNotExist);
            }

            if (Packet is not IRemoteRequest remoteRequest)
            {
                throw new ErrorCodeException(ErrorCode.NetworkPacketRequestNotRemoteRequest);
            }

            var packet = Packet;
            Packet = null;

            var (requestId, task) = NetworkChannel.Helper.RequestDispatcher.CreateRemoteRequest();

            // 设置请求id。
            remoteRequest.RequestId = requestId;

            // 发送协议包。
            NetworkChannel.Send(packet);

            return task;
        }

        public override void Clear()
        {
            base.Clear();
            if (Packet != null)
            {
                ReferencePool.Release(Packet);
                Packet = null;
            }
        }

        /// <summary>
        /// 创建网络协议交互请求。
        /// </summary>
        /// <param name="packet">协议包。</param>
        /// <param name="channelName">网络频道名。</param>
        /// <returns>网络协议交互请求。</returns>
        public static NetworkPacketRequest Create(Packet packet, string channelName)
        {
            var request = ReferencePool.Acquire<NetworkPacketRequest>();
            request.Packet = packet;
            request.NetworkChannel = NetworkModule.Instance.GetNetworkChannel(channelName);
            return request;
        }

        /// <summary>
        /// 创建网络协议交互请求。
        /// </summary>
        /// <param name="packet">协议包。</param>
        /// <param name="networkChannel">网络频道。</param>
        /// <returns>网络协议交互请求。</returns>
        public static NetworkPacketRequest Create(Packet packet, INetworkChannel networkChannel = null)
        {
            var request = ReferencePool.Acquire<NetworkPacketRequest>();
            request.Packet = packet;
            request.NetworkChannel = networkChannel ?? NetworkModule.Instance.DefaultChannel;
            return request;
        }
    }
}
