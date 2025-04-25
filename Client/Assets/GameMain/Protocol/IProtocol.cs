namespace Protocol
{
    /// <summary>
    /// 协议接口.
    /// </summary>
    public interface IProtocol
    {
    }

    /// <summary>
    /// 响应协议接口.
    /// </summary>
    public interface IResponse : IProtocol
    {
        /// <summary>
        /// 错误码.
        /// </summary>
        int ErrorCode { get; set; }
    }

}