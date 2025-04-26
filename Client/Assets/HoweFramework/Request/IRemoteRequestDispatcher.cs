using System;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 远程请求调度器。
    /// </summary>
    public interface IRemoteRequestDispatcher : IDisposable
    {
        /// <summary>
        /// 创建一个远程请求实例。
        /// </summary>
        /// <returns>请求id和异步等待对象。</returns>
        (int requestId, UniTask<ResponseBase> task) CreateRemoteRequest();

        /// <summary>
        /// 设置一个远程请求的响应。
        /// </summary>
        /// <param name="requestId">请求id。</param>
        /// <param name="response">响应。</param>
        void SetResponse(int requestId, ResponseBase response);

        /// <summary>
        /// 中断所有请求。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        void InterruptAllRequests(int errorCode = ErrorCode.RequestCanceled);
    }
}
