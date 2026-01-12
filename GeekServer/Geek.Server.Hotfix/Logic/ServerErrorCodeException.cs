namespace Server.Logic
{
    /// <summary>
    /// 服务器错误码异常。
    /// </summary>
    public sealed class ServerErrorCodeException : Exception
    {
        /// <summary>
        /// 错误码。
        /// </summary>
        public ServerErrorCode ErrorCode { get; }

        public ServerErrorCodeException(ServerErrorCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public ServerErrorCodeException(ServerErrorCode errorCode) : base()
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// 创建服务器错误码异常。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        /// <param name="message">错误描述。</param>
        /// <returns></returns>
        public static ServerErrorCodeException Create(ServerErrorCode errorCode, string message)
        {
            return new ServerErrorCodeException(errorCode, message);
        }

        /// <summary>
        /// 创建服务器错误码异常。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        /// <returns></returns>
        public static ServerErrorCodeException Create(ServerErrorCode errorCode)
        {
            return new ServerErrorCodeException(errorCode);
        }

    }
}