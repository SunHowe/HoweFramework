namespace Protocol
{
    /// <summary>
    /// 服务器错误码。
    /// </summary>
    public static class ServerErrorCode
    {
        #region [Framework 0-99999]

        /// <summary>
        /// 成功。
        /// </summary>
        public const int Success = 0;

        /// <summary>
        /// 触发异常。
        /// </summary>
        public const int Exception = 1;

        /// <summary>
        /// 框架异常。
        /// </summary>
        public const int FrameworkException = 2;

        /// <summary>
        /// 参数无效。
        /// </summary>
        public const int InvalidParam = 3;

        /// <summary>
        /// 操作无效。
        /// </summary>
        public const int InvalidOperationException = 4;

        /// <summary>
        /// 未知错误。
        /// </summary>
        public const int Unknown = 5;

        /// <summary>
        /// 框架错误码最大值。
        /// </summary>
        public const int FrameworkErrorCodeMax = 9999;

        #region [Network]   

        /// <summary>
        /// 地址族错误。
        /// </summary>
        public const int NetworkAddressFamilyError = 401;

        /// <summary>
        /// Socket 错误。
        /// </summary>
        public const int NetworkSocketError = 402;

        /// <summary>
        /// 连接错误。
        /// </summary>
        public const int NetworkConnectError = 403;

        /// <summary>
        /// 发送错误。
        /// </summary>
        public const int NetworkSendError = 404;

        /// <summary>
        /// 接收错误。
        /// </summary>
        public const int NetworkReceiveError = 405;

        /// <summary>
        /// 序列化错误。
        /// </summary>
        public const int NetworkSerializeError = 406;

        /// <summary>
        /// 反序列化消息包头错误。
        /// </summary>
        public const int NetworkDeserializePacketHeaderError = 407;

        /// <summary>
        /// 反序列化消息包错误。
        /// </summary>
        public const int NetworkDeserializePacketError = 408;

        /// <summary>
        /// 网络频道已存在。
        /// </summary>
        public const int NetworkChannelAlreadyExists = 409;

        /// <summary>
        /// 不支持的网络服务类型。
        /// </summary>
        public const int NetworkNotSupportServiceType = 410;

        /// <summary>
        /// 网络频道不存在。
        /// </summary>
        public const int NetworkChannelNotExist = 411;

        /// <summary>
        /// 网络协议包不是远程请求。
        /// </summary>
        public const int NetworkPacketRequestNotRemoteRequest = 412;

        /// <summary>
        /// 不支持的网络协议包。
        /// </summary>
        public const int NetworkNotSupportPacket = 413;

        #endregion

        #endregion

        public const int ServerErrorCodeMin = FrameworkErrorCodeMax + 1;

        #region [Login 10001-10100]

        /// <summary>
        /// 重复登录。
        /// </summary>
        public const int LoginDuplicate = 10001;

        /// <summary>
        /// 未登录。
        /// </summary>
        public const int NoLogin = 10002;

        /// <summary>
        /// 登录认证失败。
        /// </summary>
        public const int LoginAuthFailed = 10003;

        #endregion

        /// <summary>
        /// 服务器错误码最大值。
        /// </summary>
        public const int ServerErrorCodeMax = 99999;
    }
}