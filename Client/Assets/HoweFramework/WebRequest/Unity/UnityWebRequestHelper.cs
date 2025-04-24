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
        public void Dispose()
        {
        }

        public async UniTask<(int code, byte[] responseBody)> Get(string url, Dictionary<string, string> headers, CancellationToken token = default)
        {
            var request = new UnityWebRequest(url, "GET");
            foreach (var (key, value) in headers)
            {
                request.SetRequestHeader(key, value);
            }

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
            var request = new UnityWebRequest(url, "POST");

            foreach (var (key, value) in headers)
            {
                request.SetRequestHeader(key, value);
            }

            request.uploadHandler = new UploadHandlerRaw(requestBody)
            {
                contentType = contentType
            };

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
