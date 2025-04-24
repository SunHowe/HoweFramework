using System.Net;
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
        /// <returns>POST请求响应。</returns>
        internal async UniTask<WebRequestResponse> Post(WebPostRequest request)
        {
            if (m_WebRequestHelper == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "Web request helper is not set.");
            }

            if (string.IsNullOrEmpty(request.ContentType))
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "Content type is not set.");
            }

            var (statusCode, responseBody) = await m_WebRequestHelper.Post(request.Url, request.RequestBody, request.Headers, request.ContentType);
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
        /// <returns>GET请求响应。</returns>
        internal async UniTask<WebRequestResponse> Get(WebGetRequest request)
        {
            if (m_WebRequestHelper == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "Web request helper is not set.");
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

            var (statusCode, responseBody) = await m_WebRequestHelper.Get(url, request.Headers);
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
                (int)HttpStatusCode.BadRequest => ErrorCode.WebRequestBadRequest,
                (int)HttpStatusCode.Unauthorized => ErrorCode.WebRequestUnauthorized,
                (int)HttpStatusCode.PaymentRequired => ErrorCode.WebRequestPaymentRequired,
                (int)HttpStatusCode.Forbidden => ErrorCode.WebRequestForbidden,
                (int)HttpStatusCode.NotFound => ErrorCode.WebRequestNotFound,
                (int)HttpStatusCode.MethodNotAllowed => ErrorCode.WebRequestMethodNotAllowed,
                (int)HttpStatusCode.NotAcceptable => ErrorCode.WebRequestNotAcceptable,
                (int)HttpStatusCode.ProxyAuthenticationRequired => ErrorCode.WebRequestProxyAuthenticationRequired,
                (int)HttpStatusCode.RequestTimeout => ErrorCode.WebRequestRequestTimeout,
                (int)HttpStatusCode.Conflict => ErrorCode.WebRequestConflict,
                (int)HttpStatusCode.Gone => ErrorCode.WebRequestGone,
                (int)HttpStatusCode.LengthRequired => ErrorCode.WebRequestLengthRequired,
                (int)HttpStatusCode.PreconditionFailed => ErrorCode.WebRequestPreconditionFailed,
                (int)HttpStatusCode.RequestEntityTooLarge => ErrorCode.WebRequestRequestEntityTooLarge,
                (int)HttpStatusCode.RequestUriTooLong => ErrorCode.WebRequestRequestUriTooLong,
                (int)HttpStatusCode.UnsupportedMediaType => ErrorCode.WebRequestUnsupportedMediaType,
                (int)HttpStatusCode.RequestedRangeNotSatisfiable => ErrorCode.WebRequestRequestedRangeNotSatisfiable,
                (int)HttpStatusCode.ExpectationFailed => ErrorCode.WebRequestExpectationFailed,
                (int)HttpStatusCode.MisdirectedRequest => ErrorCode.WebRequestMisdirectedRequest,
                (int)HttpStatusCode.UnprocessableEntity => ErrorCode.WebRequestUnprocessableEntity,
                (int)HttpStatusCode.Locked => ErrorCode.WebRequestLocked,
                (int)HttpStatusCode.FailedDependency => ErrorCode.WebRequestFailedDependency,
                (int)HttpStatusCode.UpgradeRequired => ErrorCode.WebRequestUpgradeRequired,
                (int)HttpStatusCode.PreconditionRequired => ErrorCode.WebRequestPreconditionRequired,
                (int)HttpStatusCode.TooManyRequests => ErrorCode.WebRequestTooManyRequests,
                (int)HttpStatusCode.RequestHeaderFieldsTooLarge => ErrorCode.WebRequestRequestHeaderFieldsTooLarge,
                (int)HttpStatusCode.UnavailableForLegalReasons => ErrorCode.WebRequestUnavailableForLegalReasons,
                (int)HttpStatusCode.InternalServerError => ErrorCode.WebRequestInternalServerError,
                (int)HttpStatusCode.NotImplemented => ErrorCode.WebRequestNotImplemented,
                (int)HttpStatusCode.BadGateway => ErrorCode.WebRequestBadGateway,
                (int)HttpStatusCode.ServiceUnavailable => ErrorCode.WebRequestServiceUnavailable,
                (int)HttpStatusCode.GatewayTimeout => ErrorCode.WebRequestGatewayTimeout,
                (int)HttpStatusCode.HttpVersionNotSupported => ErrorCode.WebRequestHttpVersionNotSupported,
                (int)HttpStatusCode.VariantAlsoNegotiates => ErrorCode.WebRequestVariantAlsoNegotiates,
                (int)HttpStatusCode.InsufficientStorage => ErrorCode.WebRequestInsufficientStorage,
                (int)HttpStatusCode.LoopDetected => ErrorCode.WebRequestLoopDetected,
                (int)HttpStatusCode.NotExtended => ErrorCode.WebRequestNotExtended,
                (int)HttpStatusCode.NetworkAuthenticationRequired => ErrorCode.WebRequestNetworkAuthenticationRequired,
                _ => ErrorCode.Unknown,
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
