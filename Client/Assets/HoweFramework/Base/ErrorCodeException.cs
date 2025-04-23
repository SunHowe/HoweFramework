using System;

namespace HoweFramework
{
    /// <summary>
    /// 错误码异常类。
    /// </summary>
    public class ErrorCodeException : Exception
    {
        /// <summary>
        /// 错误码。
        /// </summary>
        public int ErrorCode { get; }

        public ErrorCodeException(int errorCode)
        {
            ErrorCode = errorCode;
        }

        public ErrorCodeException(int errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}

