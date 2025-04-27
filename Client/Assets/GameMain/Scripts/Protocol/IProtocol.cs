using MemoryPack;
#if !NETCOREAPP
using HoweFramework;
#endif

namespace Protocol
{
    /// <summary>
    /// 协议接口。
    /// </summary>
    public interface IProtocol
    {
        /// <summary>
        /// 协议ID.
        /// </summary>
        int Id { get; }
    }

    /// <summary>
    /// 请求协议接口。
    /// </summary>
    public interface IProtocolRequest
#if !NETCOREAPP
        : IProtocol, IRemoteRequest
#else
        : IProtocol
#endif
    {
#if NETCOREAPP
        /// <summary>
        /// 请求ID.
        /// </summary>
        int RequestId { get; set; }
#endif
    }

    /// <summary>
    /// 响应协议接口。
    /// </summary>
    public interface IProtocolResponse
#if !NETCOREAPP
        : IProtocol, IRemoteRequest, IResponse
#else
        : IProtocol
#endif
    {
#if NETCOREAPP
        /// <summary>
        /// 请求ID.
        /// </summary>
        int RequestId { get; set; }

        /// <summary>
        /// 错误码.
        /// </summary>
        int ErrorCode { get; set; }
#endif
    }

    /// <summary>
    /// 协议基类。
    /// </summary>
    public abstract class ProtocolBase
#if !NETCOREAPP
        : Packet, IProtocol
#else
        : IProtocol
#endif
    {
#if NETCOREAPP
        [MemoryPackIgnore]
        public abstract int Id { get; }

        public virtual void Clear()
        {
        }
#else
        public void Dispose()
        {
            ReferencePool.Release(this);
        }
#endif
    }
}