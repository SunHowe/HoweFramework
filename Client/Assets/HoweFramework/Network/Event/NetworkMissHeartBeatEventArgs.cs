﻿namespace HoweFramework
{
    /// <summary>
    /// 网络心跳包丢失事件。
    /// </summary>
    public sealed class NetworkMissHeartBeatEventArgs : GameEventArgs
    {
        /// <summary>
        /// 事件Id。
        /// </summary>
        public static readonly int EventId = typeof(NetworkMissHeartBeatEventArgs).GetHashCode();

        public override int Id => EventId;

        /// <summary>
        /// 初始化网络心跳包丢失事件的新实例。
        /// </summary>
        public NetworkMissHeartBeatEventArgs()
        {
            NetworkChannel = null;
            MissCount = 0;
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
        /// 获取心跳包已丢失次数。
        /// </summary>
        public int MissCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建网络心跳包丢失事件。
        /// </summary>
        /// <param name="networkChannel">网络频道。</param>
        /// <param name="missCount">心跳包已丢失次数。</param>
        /// <returns>创建的网络心跳包丢失事件。</returns>
        public static NetworkMissHeartBeatEventArgs Create(INetworkChannel networkChannel, int missCount)
        {
            NetworkMissHeartBeatEventArgs networkMissHeartBeatEventArgs = ReferencePool.Acquire<NetworkMissHeartBeatEventArgs>();
            networkMissHeartBeatEventArgs.NetworkChannel = networkChannel;
            networkMissHeartBeatEventArgs.MissCount = missCount;
            return networkMissHeartBeatEventArgs;
        }

        /// <summary>
        /// 创建网络心跳包丢失事件。
        /// </summary>
        /// <param name="eventArgs">网络心跳包丢失事件。</param>
        /// <returns>创建的网络心跳包丢失事件。</returns>
        public static NetworkMissHeartBeatEventArgs Create(NetworkMissHeartBeatEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                Log.Error("Network miss heart beat event args is invalid.");
                return null;
            }

            return Create(eventArgs.NetworkChannel, eventArgs.MissCount);
        }
        
        /// <summary>
        /// 清理网络心跳包丢失事件。
        /// </summary>
        public override void Clear()
        {
            NetworkChannel = null;
            MissCount = 0;
        }
    }
}
