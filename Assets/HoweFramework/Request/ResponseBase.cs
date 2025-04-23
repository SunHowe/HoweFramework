using System;

namespace HoweFramework
{
    /// <summary>
    /// 异步请求响应基类。
    /// </summary>
    public class ResponseBase : IReference, IDisposable
    {
        /// <summary>
        /// 错误码。0表示成功，其他表示失败。
        /// </summary>
        public int ErrorCode { get; set; }

        public virtual void Clear()
        {
            ErrorCode = 0;
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }
    }
}
