using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 网络模块。
    /// </summary>
    public sealed class NetworkModule : ModuleBase<NetworkModule>
    {
        /// <summary>
        /// 默认网络频道名称。
        /// </summary>
        public string DefaultChannelName { get; private set; }

        /// <summary>
        /// 默认网络频道。
        /// </summary>
        public INetworkChannel DefaultChannel { get; private set; }

        /// <summary>
        /// 获取网络频道数量。
        /// </summary>
        public int NetworkChannelCount { get; }

        /// <summary>
        /// 网络管理器。
        /// </summary>
        private INetworkManager m_NetworkManager = null;

        /// <summary>
        /// 检查是否存在网络频道。
        /// </summary>
        /// <param name="name">网络频道名称。</param>
        /// <returns>是否存在网络频道。</returns>
        public bool HasNetworkChannel(string name)
        {
            return m_NetworkManager.HasNetworkChannel(name);
        }

        /// <summary>
        /// 获取网络频道。
        /// </summary>
        /// <param name="name">网络频道名称。</param>
        /// <returns>要获取的网络频道。</returns>
        public INetworkChannel GetNetworkChannel(string name)
        {
            return m_NetworkManager.GetNetworkChannel(name);
        }

        /// <summary>
        /// 获取所有网络频道。
        /// </summary>
        /// <returns>所有网络频道。</returns>
        public INetworkChannel[] GetAllNetworkChannels()
        {
            return m_NetworkManager.GetAllNetworkChannels();
        }

        /// <summary>
        /// 获取所有网络频道。
        /// </summary>
        /// <param name="results">所有网络频道。</param>
        public void GetAllNetworkChannels(List<INetworkChannel> results)
        {
            m_NetworkManager.GetAllNetworkChannels(results);
        }

        /// <summary>
        /// 创建网络频道。
        /// </summary>
        /// <param name="name">网络频道名称。</param>
        /// <param name="serviceType">网络服务类型。</param>
        /// <param name="networkChannelHelper">网络频道辅助器。</param>
        /// <returns>要创建的网络频道。</returns>
        public INetworkChannel CreateNetworkChannel(string name, ServiceType serviceType, INetworkChannelHelper networkChannelHelper)
        {
            return m_NetworkManager.CreateNetworkChannel(name, serviceType, networkChannelHelper);
        }

        /// <summary>
        /// 销毁网络频道。
        /// </summary>
        /// <param name="name">网络频道名称。</param>
        /// <returns>是否销毁网络频道成功。</returns>
        public bool DestroyNetworkChannel(string name)
        {
            return m_NetworkManager.DestroyNetworkChannel(name);
        }
        
        /// <summary>
        /// 创建默认网络频道。
        /// </summary>
        /// <param name="module">网络模块。</param>
        /// <param name="name">网络频道名。</param>
        /// <param name="serviceType">网络服务类型。</param>
        /// <param name="networkChannelHelper">网络频道辅助器。</param>
        /// <returns>网络频道。</returns>
        public INetworkChannel CreateDefaultNetworkChannel(string name, ServiceType serviceType, INetworkChannelHelper networkChannelHelper)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam);
            }
            
            if (!string.IsNullOrEmpty(DefaultChannelName))
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "Default channel name has been set.");
            }

            var channel = CreateNetworkChannel(name, serviceType, networkChannelHelper);
            DefaultChannelName = name;
            DefaultChannel = channel;
            return channel;
        }

        protected override void OnInit()
        {
            m_NetworkManager = new NetworkManager();
            m_NetworkManager.NetworkConnected += OnNetworkConnected;
            m_NetworkManager.NetworkClosed += OnNetworkClosed;
            m_NetworkManager.NetworkMissHeartBeat += OnNetworkMissHeartBeat;
            m_NetworkManager.NetworkError += OnNetworkError;
            m_NetworkManager.NetworkCustomError += OnNetworkCustomError;
        }

        protected override void OnDestroy()
        {
            m_NetworkManager.Dispose();
            DefaultChannelName = null;
            DefaultChannel = null;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            m_NetworkManager.Update(elapseSeconds, realElapseSeconds);
        }

        private void OnNetworkConnected(object sender, NetworkConnectedEventArgs e)
        {
            EventModule.Instance.Dispatch(this, e);
        }

        private void OnNetworkClosed(object sender, NetworkClosedEventArgs e)
        {
            EventModule.Instance.Dispatch(this, e);
        }

        private void OnNetworkMissHeartBeat(object sender, NetworkMissHeartBeatEventArgs e)
        {
            EventModule.Instance.Dispatch(this, e);
        }

        private void OnNetworkError(object sender, NetworkErrorEventArgs e)
        {
            EventModule.Instance.Dispatch(this, e);
        }

        private void OnNetworkCustomError(object sender, NetworkCustomErrorEventArgs e)
        {
            EventModule.Instance.Dispatch(this, e);
        }
    }
}
