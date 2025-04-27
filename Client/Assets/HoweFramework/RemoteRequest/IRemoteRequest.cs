namespace HoweFramework
{
    /// <summary>
    /// 远程请求接口。
    /// </summary>
    public interface IRemoteRequest : IRequest
    {
        /// <summary>
        /// 请求id。
        /// </summary>
        int RequestId { get; set; }
    }
}
