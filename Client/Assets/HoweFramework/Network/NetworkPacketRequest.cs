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

        protected override UniTask<ResponseBase> OnExecute(CancellationToken token)
        {
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
    }
}
