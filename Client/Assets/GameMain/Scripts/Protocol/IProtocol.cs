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

    /// <summary>
    /// 协议基类.
    /// </summary>
    public abstract class ProtocolBase :
#if NETCOREAPP
        IProtocol
#else
        HoweFramework.Packet, IProtocol
#endif
    {
    }
}