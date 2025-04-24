using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// Web请求模块扩展。
    /// </summary>
    public static class WebRequestModuleExtensions
    {
        /// <summary>
        /// 使用UnityWebRequest的Web请求辅助器。
        /// </summary>
        /// <param name="module">Web请求模块。</param>
        /// <returns>Web请求模块。</returns>
        public static WebRequestModule UseUnityWebRequest(this WebRequestModule module)
        {
            module.SetWebRequestHelper(new UnityWebRequestHelper());
            return module;
        }

        /// <summary>
        /// 发送GET请求。
        /// </summary>
        /// <param name="module">Web请求模块。</param>
        /// <param name="url">请求地址。</param>
        /// <param name="headers">请求头。</param>
        /// <param name="parameters">请求参数。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>GET请求响应。</returns>
        public static UniTask<WebRequestResponse> Get(this WebRequestModule module, string url, Dictionary<string, string> headers = null, Dictionary<string, string> parameters = null, CancellationToken token = default)
        {
            return WebGetRequest.Create(url, headers, parameters).Execute(token).As<WebRequestResponse>();
        }

        /// <summary>
        /// 发送POST请求。
        /// </summary>
        /// <param name="module">Web请求模块。</param>
        /// <param name="url">请求地址。</param>
        /// <param name="requestBody">请求数据。</param>
        /// <param name="contentType">内容类型。</param>
        /// <param name="headers">请求头。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>POST请求响应。</returns>
        public static UniTask<WebRequestResponse> Post(this WebRequestModule module, string url, byte[] requestBody, string contentType = null, Dictionary<string, string> headers = null, CancellationToken token = default)
        {
            return WebPostRequest.Create(url, requestBody, contentType, headers).Execute(token).As<WebRequestResponse>();
        }

        /// <summary>
        /// 发送POST请求。
        /// </summary>
        /// <param name="module">Web请求模块。</param>
        /// <param name="url">请求地址。</param>
        /// <param name="requestBody">请求数据。</param>
        /// <param name="headers">请求头。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>POST请求响应。</returns>
        public static UniTask<WebRequestResponse> PostJsonObject(this WebRequestModule module, string url, object requestBody, Dictionary<string, string> headers = null, CancellationToken token = default)
        {
            return WebPostRequest.CreateJsonRequest(url, requestBody, headers).Execute(token).As<WebRequestResponse>();
        }

        /// <summary>
        /// 发送POST请求。
        /// </summary>
        /// <param name="module">Web请求模块。</param>
        /// <param name="url">请求地址。</param>
        /// <param name="requestBody">请求数据。</param>
        /// <param name="headers">请求头。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>POST请求响应。</returns>
        public static UniTask<WebRequestResponse> PostText(this WebRequestModule module, string url, string requestBody, Dictionary<string, string> headers = null, CancellationToken token = default)
        {
            return WebPostRequest.CreateTextRequest(url, requestBody, headers).Execute(token).As<WebRequestResponse>();
        }
        
    }
}
