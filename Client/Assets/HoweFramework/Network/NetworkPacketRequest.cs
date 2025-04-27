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
        /// 网络频道名。
        /// </summary>
        public string ChannelName { get; set; }

        protected override UniTask<IResponse> OnExecute(CancellationToken token)
        {
            if (string.IsNullOrEmpty(ChannelName))
            {
                throw new ErrorCodeException(ErrorCode.NetworkChannelNotExist);
            }

            var networkChannel = NetworkModule.Instance.GetNetworkChannel(ChannelName);
            if (networkChannel == null)
            {
                throw new ErrorCodeException(ErrorCode.NetworkChannelNotExist);
            }

            if (Packet is not IRemoteRequest remoteRequest)
            {
                throw new ErrorCodeException(ErrorCode.NetworkPacketRequestNotRemoteRequest);
            }

            var packet = Packet;
            Packet = null;

            var (requestId, task) = networkChannel.Helper.RequestDispatcher.CreateRemoteRequest();

            // 设置请求id。
            remoteRequest.RequestId = requestId;

            // 发送协议包。
            networkChannel.Send(packet);

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
        public static NetworkPacketRequest Create(Packet packet, string channelName = null)
        {
            channelName ??= NetworkModule.Instance.DefaultChannelName;

            var request = ReferencePool.Acquire<NetworkPacketRequest>();
            request.Packet = packet;
            request.ChannelName = channelName;
            return request;
        }
    }
}
