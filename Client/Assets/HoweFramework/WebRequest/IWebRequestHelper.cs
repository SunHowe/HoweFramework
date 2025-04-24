using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// Web请求辅助器接口。
    /// </summary>
    public interface IWebRequestHelper : IDisposable
    {
        /// <summary>
        /// 发送POST请求。
        /// </summary>
        /// <param name="url">请求地址。</param>
        /// <param name="requestBody">请求数据。</param>
        /// <param name="headers">请求头。</param>
        /// <param name="contentType">内容类型。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>POST请求响应。</returns>
        UniTask<(int code, byte[] responseBody)> Post(string url, byte[] requestBody, Dictionary<string, string> headers, string contentType, CancellationToken token = default);

        /// <summary>
        /// 发送GET请求。
        /// </summary>
        /// <param name="url">请求地址。</param>
        /// <param name="headers">请求头。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>GET请求响应。</returns>
        UniTask<(int code, byte[] responseBody)> Get(string url, Dictionary<string, string> headers, CancellationToken token = default);
    }
}

