using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 游戏错误码。
    /// </summary>
    public static class ErrorCode
    {
        /// <summary>
        /// 成功。
        /// </summary>
        public const int Success = FrameworkErrorCode.Success;

        /// <summary>
        /// 触发异常。
        /// </summary>
        public const int Exception = FrameworkErrorCode.Exception;

        /// <summary>
        /// 框架异常。
        /// </summary>
        public const int FrameworkException = FrameworkErrorCode.FrameworkException;

        /// <summary>
        /// 参数无效。
        /// </summary>
        public const int InvalidParam = FrameworkErrorCode.InvalidParam;

        /// <summary>
        /// 操作无效。
        /// </summary>
        public const int InvalidOperationException = FrameworkErrorCode.InvalidOperationException;

        /// <summary>
        /// 未知错误。
        /// </summary>
        public const int Unknown = FrameworkErrorCode.Unknown;

        /// <summary>
        /// 框架错误码最大值。
        /// </summary>
        public const int FrameworkErrorCodeMax = FrameworkErrorCode.FrameworkErrorCodeMax;

        /// <summary>
        /// 服务器错误码最大值。
        /// </summary>
        public const int ServerErrorCodeMax = Protocol.ServerErrorCode.ServerErrorCodeMax;

        /// <summary>
        /// 客户端错误码最小值。
        /// </summary>
        public const int ClientErrorCodeMin = ServerErrorCodeMax + 1;
    }
}