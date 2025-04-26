using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 远程请求调度器。
    /// </summary>
    public sealed class RemoteRequestDispatcher : IRemoteRequestDispatcher, IReference
    {
        /// <summary>
        /// 请求字典。
        /// </summary>
        private readonly Dictionary<int, AutoResetUniTaskCompletionSource<ResponseBase>> m_RequestDict = new();

        /// <summary>
        /// 自增长的请求id。
        /// </summary>
        private int m_RequestId;
        
        /// <summary>
        /// 创建一个远程请求实例。
        /// </summary>
        /// <returns>请求id和异步等待对象。</returns>
        public (int requestId, UniTask<ResponseBase> task) CreateRemoteRequest()
        {
            var requestId = ++m_RequestId;
            var tcs = AutoResetUniTaskCompletionSource<ResponseBase>.Create();

            m_RequestDict.Add(requestId, tcs);
            return (requestId, tcs.Task);
        }

        /// <summary>
        /// 设置一个远程请求的响应。
        /// </summary>
        /// <param name="requestId">请求id。</param>
        /// <param name="response">响应。</param>
        public void SetResponse(int requestId, ResponseBase response)
        {
            if (m_RequestDict.Remove(requestId, out var tcs))
            {
                tcs.TrySetResult(response);
            }
            else
            {
                response.Dispose();
            }
        }

        /// <summary>
        /// 中断所有请求。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        public void InterruptAllRequests(int errorCode = ErrorCode.RequestCanceled)
        {
            using var buffer = ReusableList<AutoResetUniTaskCompletionSource<ResponseBase>>.Create();
            foreach (var request in m_RequestDict)
            {
                buffer.Add(request.Value);
            }

            m_RequestDict.Clear();

            foreach (var tcs in buffer)
            {
                tcs.TrySetResult(CommonResponse.Create(errorCode));
            }
        }

        public void Dispose()
        {
            InterruptAllRequests(ErrorCode.RequestDispatcherDisposing);
            ReferencePool.Release(this);
        }

        public void Clear()
        {
        }
    }
}
