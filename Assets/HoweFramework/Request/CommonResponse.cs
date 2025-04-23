namespace HoweFramework
{
    /// <summary>
    /// 通用响应包实例。
    /// </summary>
    public sealed class CommonResponse : ResponseBase
    {
        public static CommonResponse Create(int errorCode)
        {
            var response = ReferencePool.Acquire<CommonResponse>();
            response.ErrorCode = errorCode;
            return response;
        }
    }
}
