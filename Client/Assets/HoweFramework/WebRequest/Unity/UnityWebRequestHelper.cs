using System.Collections.Generic;
using System.Net;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace HoweFramework
{
    /// <summary>
    /// Unity WebRequest 辅助器。
    /// </summary>
    internal sealed class UnityWebRequestHelper : IWebRequestHelper
    {
        /// <summary>
        /// 默认请求超时时长，以秒为单位。
        /// </summary>
        private const int DefaultTimeoutSeconds = 30;

        public void Dispose()
        {
        }

        public async UniTask<(int code, byte[] responseBody)> Get(string url, Dictionary<string, string> headers, CancellationToken token = default)
        {
            // UnityWebRequest 及 handler 持有原生资源，必须 Dispose（using 覆盖取消路径）。
            using var request = new UnityWebRequest(url, "GET");
            request.timeout = DefaultTimeoutSeconds;
            foreach (var (key, value) in headers)
            {
                request.SetRequestHeader(key, value);
            }

            request.downloadHandler = new DownloadHandlerBuffer();

            var operation = request.SendWebRequest();

            await operation.ToUniTask(cancellationToken: token);

            if (request.result != UnityWebRequest.Result.Success)
            {
                return ((int)request.responseCode, request.downloadHandler.data);
            }

            return ((int)HttpStatusCode.OK, request.downloadHandler.data);
        }

        public async UniTask<(int code, byte[] responseBody)> Post(string url, byte[] requestBody, Dictionary<string, string> headers, string contentType, CancellationToken token = default)
        {
            // UnityWebRequest 及 handler 持有原生资源，必须 Dispose（using 覆盖取消路径）。
            using var request = new UnityWebRequest(url, "POST");
            request.timeout = DefaultTimeoutSeconds;

            foreach (var (key, value) in headers)
            {
                request.SetRequestHeader(key, value);
            }

            request.uploadHandler = new UploadHandlerRaw(requestBody)
            {
                contentType = contentType
            };
            request.downloadHandler = new DownloadHandlerBuffer();

            var operation = request.SendWebRequest();

            await operation.ToUniTask(cancellationToken: token);

            if (request.result != UnityWebRequest.Result.Success)
            {
                return ((int)request.responseCode, request.downloadHandler.data);
            }

            return ((int)HttpStatusCode.OK, request.downloadHandler.data);
        }
    }
}
