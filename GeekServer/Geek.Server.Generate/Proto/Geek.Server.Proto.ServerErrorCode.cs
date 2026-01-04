//auto generated, do not modify it

namespace Geek.Server.Proto
{
    public enum ServerErrorCode
    {
        Success = 0,

        /// <summary>
        /// 服务器内部错误。
        /// </summary>
        InternalError = 100001,
        
        // Login
        Login_AccountCannotBeNull = 100100,
        Login_UnknownPlatform,
    }
}
