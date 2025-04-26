using System.Net;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 网络频道连接请求。
    /// </summary>
    public sealed class NetworkChannelConnectRequest : RequestBase
    {
        /// <summary>
        /// 网络频道。
        /// </summary>
        public INetworkChannel NetworkChannel { get; set; }

        /// <summary>
        /// 连接地址。
        /// </summary>
        public IPAddress Address { get; set; }

        /// <summary>
        /// 连接端口。
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 连接结果。
        /// </summary>
        private AutoResetUniTaskCompletionSource<IResponse> m_UniTaskCompletionSource;

        protected override async UniTask<IResponse> OnExecute(CancellationToken token)
        {
            if (NetworkChannel == null)
            {
                throw new ErrorCodeException(ErrorCode.NetworkChannelNotExist);
            }

            if (NetworkChannel.Connected)
            {
                return CommonResponse.Create(ErrorCode.Success);
            }

            m_UniTaskCompletionSource = AutoResetUniTaskCompletionSource<IResponse>.Create();

            using var _ = EventCapture.Create(NetworkConnectedEventArgs.EventId, OnNetworkConnected);
            using var ___ = EventCapture.Create(NetworkErrorEventArgs.EventId, OnNetworkError);
            using var ____ = EventCapture.Create(NetworkCustomErrorEventArgs.EventId, OnNetworkCustomError);

            NetworkChannel.Connect(Address, Port);

            return await m_UniTaskCompletionSource.Task;
        }

        public override void Clear()
        {
            base.Clear();

            m_UniTaskCompletionSource = null;
            NetworkChannel = null;
            Address = null;
            Port = 0;
        }

        /// <summary>
        /// 网络连接成功事件。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="e">网络连接成功事件。</param>
        private void OnNetworkConnected(object sender, GameEventArgs e)
        {
            var networkConnectedEventArgs = e as NetworkConnectedEventArgs;
            if (networkConnectedEventArgs == null)
            {
                return;
            }

            if (networkConnectedEventArgs.NetworkChannel != NetworkChannel)
            {
                return;
            }

            if (m_UniTaskCompletionSource == null)
            {
                return;
            }

            m_UniTaskCompletionSource.TrySetResult(CommonResponse.Create(ErrorCode.Success));
        }

        /// <summary>
        /// 网络连接错误事件。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="e">网络连接错误事件。</param>
        private void OnNetworkError(object sender, GameEventArgs e)
        {
            var networkErrorEventArgs = e as NetworkErrorEventArgs;
            if (networkErrorEventArgs == null)
            {
                return;
            }

            if (networkErrorEventArgs.NetworkChannel != NetworkChannel)
            {
                return;
            }

            if (m_UniTaskCompletionSource == null)
            {
                return;
            }

            m_UniTaskCompletionSource.TrySetResult(CommonResponse.Create(networkErrorEventArgs.ErrorCode, networkErrorEventArgs.ErrorMessage));
        }

        /// <summary>
        /// 网络自定义错误事件。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="e">网络自定义错误事件。</param>
        private void OnNetworkCustomError(object sender, GameEventArgs e)
        {
            var networkCustomErrorEventArgs = e as NetworkCustomErrorEventArgs;
            if (networkCustomErrorEventArgs == null)
            {
                return;
            }

            if (networkCustomErrorEventArgs.NetworkChannel != NetworkChannel)
            {
                return;
            }

            if (m_UniTaskCompletionSource == null)
            {
                return;
            }

            if (networkCustomErrorEventArgs.CustomErrorData is ErrorCodeException errorCodeException)
            {
                m_UniTaskCompletionSource.TrySetResult(CommonResponse.Create(errorCodeException.ErrorCode));
            }
            else
            {
                m_UniTaskCompletionSource.TrySetResult(CommonResponse.Create(ErrorCode.NetworkConnectError));
            }
        }

        /// <summary>
        /// 创建网络频道连接请求。
        /// </summary>
        /// <param name="address">连接地址。</param>
        /// <param name="port">连接端口。</param>
        /// <param name="networkChannel">网络频道。若未指定则使用默认网络频道。</param>
        /// <returns>网络频道连接请求。</returns>
        public static NetworkChannelConnectRequest Create(IPAddress address, int port, INetworkChannel networkChannel = null)
        {
            var request = ReferencePool.Acquire<NetworkChannelConnectRequest>();
            request.NetworkChannel = networkChannel ?? NetworkModule.Instance.DefaultChannel;
            request.Address = address;
            request.Port = port;

            return request;
        }
    }
}
