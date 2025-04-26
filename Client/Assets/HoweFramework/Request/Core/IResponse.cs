using System;

namespace HoweFramework
{
    /// <summary>
    /// 响应接口。
    /// </summary>
    public interface IResponse : IDisposable, IReference
    {
        /// <summary>
        /// 错误码。0表示成功，其他表示失败。
        /// </summary>
        int ErrorCode { get; set; }
    }
}

