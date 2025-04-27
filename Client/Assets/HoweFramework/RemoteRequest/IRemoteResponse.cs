namespace HoweFramework
{
    /// <summary>
    /// 远程响应接口。
    /// </summary>
    public interface IRemoteResponse
    {
        /// <summary>
        /// 请求id。
        /// </summary>
        int RequestId { get; set; }

        /// <summary>
        /// 实际的响应包实例。
        /// </summary>
        ResponseBase Response { get; }

        /// <summary>
        /// 声明外部已获取响应包，防止被意外回收。
        /// </summary>
        void PickResponse();
    }
}

