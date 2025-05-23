﻿namespace HoweFramework
{
    /// <summary>
    /// 网络连接关闭事件。
    /// </summary>
    public sealed class NetworkClosedEventArgs : GameEventArgs
    {
        /// <summary>
        /// 事件Id。
        /// </summary>
        public static readonly int EventId = typeof(NetworkClosedEventArgs).GetHashCode();

        public override int Id => EventId;

        /// <summary>
        /// 初始化网络连接关闭事件的新实例。
        /// </summary>
        public NetworkClosedEventArgs()
        {
            NetworkChannel = null;
        }

        /// <summary>
        /// 获取网络频道。
        /// </summary>
        public INetworkChannel NetworkChannel
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建网络连接关闭事件。
        /// </summary>
        /// <param name="networkChannel">网络频道。</param>
        /// <returns>创建的网络连接关闭事件。</returns>
        public static NetworkClosedEventArgs Create(INetworkChannel networkChannel)
        {
            NetworkClosedEventArgs networkClosedEventArgs = ReferencePool.Acquire<NetworkClosedEventArgs>();
            networkClosedEventArgs.NetworkChannel = networkChannel;
            return networkClosedEventArgs;
        }

        /// <summary>
        /// 创建网络连接关闭事件。
        /// </summary>
        /// <param name="eventArgs">网络连接关闭事件。</param>
        /// <returns>创建的网络连接关闭事件。</returns>
        public static NetworkClosedEventArgs Create(NetworkClosedEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                Log.Error("Network closed event args is invalid.");
                return null;
            }

            return Create(eventArgs.NetworkChannel);
        }

        /// <summary>
        /// 清理网络连接关闭事件。
        /// </summary>
        public override void Clear()
        {
            NetworkChannel = null;
        }
    }
}
