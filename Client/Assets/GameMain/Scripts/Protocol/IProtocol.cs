using MemoryPack;

namespace Protocol
{
    /// <summary>
    /// 协议接口。
    /// </summary>
    public interface IProtocol
#if !NETCOREAPP
     : HoweFramework.IReference
#endif
    {
        /// <summary>
        /// 协议ID.
        /// </summary>
        ushort Id { get; }
    }

    /// <summary>
    /// 响应协议接口。
    /// </summary>
    public interface IProtocolResponse : IProtocol
    {
        /// <summary>
        /// 错误码.
        /// </summary>
        [MemoryPackIgnore]
        int ErrorCode { get; set; }
    }

    /// <summary>
    /// 协议基类。
    /// </summary>
    public abstract class ProtocolBase : IProtocol
    {
        public abstract ushort Id { get; }
        public abstract void Clear();
    }
}