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
        private readonly Dictionary<int, AutoResetUniTaskCompletionSource<IResponse>> m_RequestDict = new();

        /// <summary>
        /// 请求创建时间字典（用于超时检测）。
        /// </summary>
        private readonly Dictionary<int, float> m_RequestTimeDict = new();

        /// <summary>
        /// 自增长的请求id。
        /// </summary>
        private int m_RequestId;

        /// <summary>
        /// 请求超时时长，以秒为单位。小于等于 0 表示不启用超时检测。默认 30 秒。
        /// </summary>
        public float RequestTimeout { get; set; } = 30f;

        /// <summary>
        /// 创建一个远程请求实例。
        /// </summary>
        /// <returns>请求id和异步等待对象。</returns>
        public (int requestId, UniTask<IResponse> task) CreateRemoteRequest()
        {
            // 惰性扫描并中断超时请求，避免服务器丢包时请求永久挂起。
            ScanTimeoutRequests();

            var requestId = ++m_RequestId;
            var tcs = AutoResetUniTaskCompletionSource<IResponse>.Create();

            m_RequestDict.Add(requestId, tcs);
            if (RequestTimeout > 0f)
            {
                m_RequestTimeDict.Add(requestId, UnityEngine.Time.realtimeSinceStartup);
            }

            return (requestId, tcs.Task);
        }

        /// <summary>
        /// 设置一个远程请求的响应。
        /// </summary>
        /// <param name="requestId">请求id。</param>
        /// <param name="response">响应。</param>
        public void SetResponse(int requestId, IResponse response)
        {
            m_RequestTimeDict.Remove(requestId);
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
        /// 移除一个远程请求，并以取消结果完成其等待任务。用于发送失败等需要回收注册项的场景。
        /// </summary>
        /// <param name="requestId">请求id。</param>
        public void Remove(int requestId)
        {
            m_RequestTimeDict.Remove(requestId);
            if (m_RequestDict.Remove(requestId, out var tcs))
            {
                tcs.TrySetResult(CommonResponse.Create(FrameworkErrorCode.RequestCanceled));
            }
        }

        /// <summary>
        /// 中断所有请求。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        public void InterruptAllRequests(int errorCode = FrameworkErrorCode.RequestCanceled)
        {
            using var buffer = ReusableList<AutoResetUniTaskCompletionSource<IResponse>>.Create();
            foreach (var request in m_RequestDict)
            {
                buffer.Add(request.Value);
            }

            m_RequestDict.Clear();
            m_RequestTimeDict.Clear();

            foreach (var tcs in buffer)
            {
                tcs.TrySetResult(CommonResponse.Create(errorCode));
            }
        }

        /// <summary>
        /// 扫描并中断超时请求。
        /// </summary>
        private void ScanTimeoutRequests()
        {
            if (RequestTimeout <= 0f || m_RequestTimeDict.Count == 0)
            {
                return;
            }

            var now = UnityEngine.Time.realtimeSinceStartup;
            using var timeoutList = ReusableList<int>.Create();
            foreach (var pair in m_RequestTimeDict)
            {
                if (now - pair.Value >= RequestTimeout)
                {
                    timeoutList.Add(pair.Key);
                }
            }

            foreach (var requestId in timeoutList)
            {
                m_RequestTimeDict.Remove(requestId);
                if (m_RequestDict.Remove(requestId, out var tcs))
                {
                    tcs.TrySetResult(CommonResponse.Create(FrameworkErrorCode.RequestTimeout));
                }
            }
        }

        public void Dispose()
        {
            InterruptAllRequests(FrameworkErrorCode.RequestDispatcherDisposing);
            ReferencePool.Release(this);
        }

        public void Clear()
        {
            RequestTimeout = 30f;
        }
    }
}
