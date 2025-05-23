﻿using System.Net.Sockets;

namespace HoweFramework
{
    /// <summary>
    /// 网络错误事件。
    /// </summary>
    public sealed class NetworkErrorEventArgs : GameEventArgs
    {
        /// <summary>
        /// 事件Id。
        /// </summary>
        public static readonly int EventId = typeof(NetworkErrorEventArgs).GetHashCode();

        public override int Id => EventId;

        /// <summary>
        /// 初始化网络错误事件的新实例。
        /// </summary>
        public NetworkErrorEventArgs()
        {
            NetworkChannel = null;
            ErrorCode = 0;
            SocketErrorCode = SocketError.Success;
            ErrorMessage = null;
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
        /// 获取错误码。
        /// </summary>
        public int ErrorCode
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取 Socket 错误码。
        /// </summary>
        public SocketError SocketErrorCode
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取错误信息。
        /// </summary>
        public string ErrorMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建网络错误事件。
        /// </summary>
        /// <param name="networkChannel">网络频道。</param>
        /// <param name="errorCode">错误码。</param>
        /// <param name="socketErrorCode">Socket 错误码。</param>
        /// <param name="errorMessage">错误信息。</param>
        /// <returns>创建的网络错误事件。</returns>
        public static NetworkErrorEventArgs Create(INetworkChannel networkChannel, int errorCode, SocketError socketErrorCode, string errorMessage)
        {
            NetworkErrorEventArgs networkErrorEventArgs = ReferencePool.Acquire<NetworkErrorEventArgs>();
            networkErrorEventArgs.NetworkChannel = networkChannel;
            networkErrorEventArgs.ErrorCode = errorCode;
            networkErrorEventArgs.SocketErrorCode = socketErrorCode;
            networkErrorEventArgs.ErrorMessage = errorMessage;
            return networkErrorEventArgs;
        }

        /// <summary>
        /// 创建网络错误事件。
        /// </summary>
        /// <param name="eventArgs">网络错误事件。</param>
        /// <returns>创建的网络错误事件。</returns>
        public static NetworkErrorEventArgs Create(NetworkErrorEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                Log.Error("Network error event args is invalid.");
                return null;
            }

            return Create(eventArgs.NetworkChannel, eventArgs.ErrorCode, eventArgs.SocketErrorCode, eventArgs.ErrorMessage);
        }

        /// <summary>
        /// 清理网络错误事件。
        /// </summary>
        public override void Clear()
        {
            NetworkChannel = null;
            ErrorCode = 0;
            SocketErrorCode = SocketError.Success;
            ErrorMessage = null;
        }
    }
}
