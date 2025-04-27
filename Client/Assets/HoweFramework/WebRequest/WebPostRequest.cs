using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// Web Post请求。
    /// </summary>
    public sealed class WebPostRequest : RequestBase
    {
        /// <summary>
        /// 请求地址。
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 请求数据。
        /// </summary>
        public byte[] RequestBody { get; set; }

        /// <summary>
        /// 请求头。
        /// </summary>
        public Dictionary<string, string> Headers { get; } = new();

        /// <summary>
        /// 内容类型。
        /// </summary>
        public string ContentType { get; set; }

        public override void Clear()
        {
            base.Clear();

            Url = null;
            RequestBody = null;
            Headers.Clear();
            ContentType = null;
        }

        protected override UniTask<IResponse> OnExecute(CancellationToken token)
        {
            return WebRequestModule.Instance.Post(this, token);
        }

        /// <summary>
        /// 创建WebPostRequest。
        /// </summary>
        /// <param name="url">请求地址。</param>
        /// <param name="requestBody">请求数据。</param>
        /// <param name="contentType">内容类型。</param>
        /// <param name="headers">请求头。</param>
        /// <returns>WebPostRequest。</returns>
        public static WebPostRequest Create(string url, byte[] requestBody, string contentType = null, Dictionary<string, string> headers = null)
        {
            var request = new WebPostRequest();
            request.Url = url;
            request.RequestBody = requestBody;
            request.ContentType = contentType;
            
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            return request;
        }

        /// <summary>
        /// 创建JSON 创建WebPostRequest。请求。
        /// </summary>
        /// <param name="url">请求地址。</param>
        /// <param name="requestBody">请求数据。</param>
        /// <param name="headers">请求头。</param>
        /// <returns>WebPostRequest。</returns>
        public static WebPostRequest CreateJsonRequest(string url, object requestBody, Dictionary<string, string> headers = null)
        {
            return Create(url, Encoding.UTF8.GetBytes(JsonUtility.ToJson(requestBody)), "application/json", headers);
        }

        /// <summary>
        /// 创建文本请求。
        /// </summary>
        /// <param name="url">请求地址。</param>
        /// <param name="requestBody">请求数据。</param>
        /// <param name="headers">请求头。</param>
        /// <returns>WebPostRequest。</returns>
        public static WebPostRequest CreateTextRequest(string url, string requestBody, Dictionary<string, string> headers = null)
        {
            return Create(url, Encoding.UTF8.GetBytes(requestBody), "text/plain", headers);
        }
    }
}
