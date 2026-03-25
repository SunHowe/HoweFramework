using System.Net;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// Web请求模块。
    /// </summary>
    public sealed class WebRequestModule : ModuleBase<WebRequestModule>
    {
        private IWebRequestHelper m_WebRequestHelper;

        /// <summary>
        /// 设置Web请求辅助器。
        /// </summary>
        /// <param name="webRequestHelper">Web请求辅助器。</param>
        public void SetWebRequestHelper(IWebRequestHelper webRequestHelper)
        {
            m_WebRequestHelper = webRequestHelper;
        }

        /// <summary>
        /// 发送POST请求。
        /// </summary>
        /// <param name="request">POST请求。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>POST请求响应。</returns>
        internal async UniTask<IResponse> Post(WebPostRequest request, CancellationToken token)
        {
            if (m_WebRequestHelper == null)
            {
                throw new ErrorCodeException(FrameworkErrorCode.InvalidOperationException, "Web request helper is not set.");
            }

            if (string.IsNullOrEmpty(request.ContentType))
            {
                throw new ErrorCodeException(FrameworkErrorCode.InvalidParam, "Content type is not set.");
            }

            var (statusCode, responseBody) = await m_WebRequestHelper.Post(request.Url, request.RequestBody, request.Headers, request.ContentType, token);
            if (statusCode != (int)HttpStatusCode.OK)
            {
                return WebRequestResponse.Create(GetErrorCode(statusCode), responseBody);
            }

            return WebRequestResponse.Create(statusCode, responseBody);
        }

        /// <summary>
        /// 发送GET请求。
        /// </summary>
        /// <param name="request">GET请求。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>GET请求响应。</returns>
        internal async UniTask<IResponse> Get(WebGetRequest request, CancellationToken token)
        {
            if (m_WebRequestHelper == null)
            {
                throw new ErrorCodeException(FrameworkErrorCode.InvalidOperationException, "Web request helper is not set.");
            }

            var url = request.Url;

            if (request.Parameters.Count > 0)
            {
                using var sb = ReusableStringBuilder.Create();
                sb.AppendFormat("{0}?", url);
                var first = true;
                foreach (var (key, value) in request.Parameters)
                {
                    if (first)
                    {
                        sb.Append("&");
                        first = false;
                    }

                    sb.Append(key);
                    sb.Append("=");
                    sb.Append(value);
                }

                url = sb.ToString();
            }

            var (statusCode, responseBody) = await m_WebRequestHelper.Get(url, request.Headers, token);
            if (statusCode != (int)HttpStatusCode.OK)
            {
                return WebRequestResponse.Create(GetErrorCode(statusCode), responseBody);
            }

            return WebRequestResponse.Create(statusCode, responseBody);
        }

        /// <summary>
        /// 根据HTTP状态码获取错误码。
        /// </summary>
        /// <param name="statusCode">HTTP状态码。</param>
        /// <returns>错误码。</returns>
        private int GetErrorCode(int statusCode)
        {
            return statusCode switch
            {
                (int)HttpStatusCode.BadRequest => FrameworkErrorCode.WebRequestBadRequest,
                (int)HttpStatusCode.Unauthorized => FrameworkErrorCode.WebRequestUnauthorized,
                (int)HttpStatusCode.PaymentRequired => FrameworkErrorCode.WebRequestPaymentRequired,
                (int)HttpStatusCode.Forbidden => FrameworkErrorCode.WebRequestForbidden,
                (int)HttpStatusCode.NotFound => FrameworkErrorCode.WebRequestNotFound,
                (int)HttpStatusCode.MethodNotAllowed => FrameworkErrorCode.WebRequestMethodNotAllowed,
                (int)HttpStatusCode.NotAcceptable => FrameworkErrorCode.WebRequestNotAcceptable,
                (int)HttpStatusCode.ProxyAuthenticationRequired => FrameworkErrorCode.WebRequestProxyAuthenticationRequired,
                (int)HttpStatusCode.RequestTimeout => FrameworkErrorCode.WebRequestRequestTimeout,
                (int)HttpStatusCode.Conflict => FrameworkErrorCode.WebRequestConflict,
                (int)HttpStatusCode.Gone => FrameworkErrorCode.WebRequestGone,
                (int)HttpStatusCode.LengthRequired => FrameworkErrorCode.WebRequestLengthRequired,
                (int)HttpStatusCode.PreconditionFailed => FrameworkErrorCode.WebRequestPreconditionFailed,
                (int)HttpStatusCode.RequestEntityTooLarge => FrameworkErrorCode.WebRequestRequestEntityTooLarge,
                (int)HttpStatusCode.RequestUriTooLong => FrameworkErrorCode.WebRequestRequestUriTooLong,
                (int)HttpStatusCode.UnsupportedMediaType => FrameworkErrorCode.WebRequestUnsupportedMediaType,
                (int)HttpStatusCode.RequestedRangeNotSatisfiable => FrameworkErrorCode.WebRequestRequestedRangeNotSatisfiable,
                (int)HttpStatusCode.ExpectationFailed => FrameworkErrorCode.WebRequestExpectationFailed,
                (int)HttpStatusCode.MisdirectedRequest => FrameworkErrorCode.WebRequestMisdirectedRequest,
                (int)HttpStatusCode.UnprocessableEntity => FrameworkErrorCode.WebRequestUnprocessableEntity,
                (int)HttpStatusCode.Locked => FrameworkErrorCode.WebRequestLocked,
                (int)HttpStatusCode.FailedDependency => FrameworkErrorCode.WebRequestFailedDependency,
                (int)HttpStatusCode.UpgradeRequired => FrameworkErrorCode.WebRequestUpgradeRequired,
                (int)HttpStatusCode.PreconditionRequired => FrameworkErrorCode.WebRequestPreconditionRequired,
                (int)HttpStatusCode.TooManyRequests => FrameworkErrorCode.WebRequestTooManyRequests,
                (int)HttpStatusCode.RequestHeaderFieldsTooLarge => FrameworkErrorCode.WebRequestRequestHeaderFieldsTooLarge,
                (int)HttpStatusCode.UnavailableForLegalReasons => FrameworkErrorCode.WebRequestUnavailableForLegalReasons,
                (int)HttpStatusCode.InternalServerError => FrameworkErrorCode.WebRequestInternalServerError,
                (int)HttpStatusCode.NotImplemented => FrameworkErrorCode.WebRequestNotImplemented,
                (int)HttpStatusCode.BadGateway => FrameworkErrorCode.WebRequestBadGateway,
                (int)HttpStatusCode.ServiceUnavailable => FrameworkErrorCode.WebRequestServiceUnavailable,
                (int)HttpStatusCode.GatewayTimeout => FrameworkErrorCode.WebRequestGatewayTimeout,
                (int)HttpStatusCode.HttpVersionNotSupported => FrameworkErrorCode.WebRequestHttpVersionNotSupported,
                (int)HttpStatusCode.VariantAlsoNegotiates => FrameworkErrorCode.WebRequestVariantAlsoNegotiates,
                (int)HttpStatusCode.InsufficientStorage => FrameworkErrorCode.WebRequestInsufficientStorage,
                (int)HttpStatusCode.LoopDetected => FrameworkErrorCode.WebRequestLoopDetected,
                (int)HttpStatusCode.NotExtended => FrameworkErrorCode.WebRequestNotExtended,
                (int)HttpStatusCode.NetworkAuthenticationRequired => FrameworkErrorCode.WebRequestNetworkAuthenticationRequired,
                _ => FrameworkErrorCode.Unknown,
            };
        }

        protected override void OnInit()
        {
        }

        protected override void OnDestroy()
        {
            m_WebRequestHelper?.Dispose();
            m_WebRequestHelper = null;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}
