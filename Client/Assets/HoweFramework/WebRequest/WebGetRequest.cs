using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// Web Get请求。
    /// </summary>
    public sealed class WebGetRequest : RequestBase
    {
        /// <summary>
        /// 请求地址。
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 请求头。
        /// </summary>
        public Dictionary<string, string> Headers { get; } = new();

        /// <summary>
        /// 请求参数。
        /// </summary>
        public Dictionary<string, string> Parameters { get; } = new();

        public override void Clear()
        {
            base.Clear();
            Url = null;
        }

        protected override UniTask<ResponseBase> OnExecute(CancellationToken token)
        {
            return WebRequestModule.Instance.Get(this, token);
        }

        /// <summary>
        /// 创建WebGetRequest。
        /// </summary>
        /// <param name="url">请求地址。</param>
        /// <param name="headers">请求头。</param>
        /// <param name="parameters">请求参数。</param>
        /// <returns>WebGetRequest。</returns>
        public static WebGetRequest Create(string url, Dictionary<string, string> headers = null, Dictionary<string, string> parameters = null)
        {
            var request = new WebGetRequest();
            request.Url = url;
            
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    request.Parameters.Add(parameter.Key, parameter.Value);
                }
            }

            return request;
        }
    }
}

