namespace HoweFramework
{
    /// <summary>
    /// 远程响应接口。
    /// </summary>
    public interface IRemoteResponse : IResponse
    {
        /// <summary>
        /// 请求id。
        /// </summary>
        int RequestId { get; set; }
    }
}

