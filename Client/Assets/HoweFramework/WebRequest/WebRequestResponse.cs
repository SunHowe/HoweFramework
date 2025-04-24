using System.Text;

namespace HoweFramework
{
    /// <summary>
    /// Web请求响应。
    /// </summary>
    public sealed class WebRequestResponse : ResponseBase
    {
        /// <summary>
        /// 原始响应数据。
        /// </summary>
        public byte[] RawResponseBody { get; set; }

        /// <summary>
        /// 响应文本，由原始数据转换而来。
        /// </summary>
        public string ResponseText
        {
            get
            {
                if (string.IsNullOrEmpty(m_ResponseText))
                {
                    m_ResponseText = Encoding.UTF8.GetString(RawResponseBody);
                }

                return m_ResponseText;
            }
        }

        private string m_ResponseText;

        /// <summary>
        /// 获取响应数据。
        /// </summary>
        /// <typeparam name="T">响应数据类型。</typeparam>
        /// <returns>响应数据。</returns>
        public T GetResponseData<T>()
        {
            if (RawResponseBody == null)
            {
                return default;
            }

            return JsonUtility.ToObject<T>(ResponseText);
        }

        public override void Clear()
        {
            base.Clear();
            RawResponseBody = null;
            m_ResponseText = null;
        }

        public static WebRequestResponse Create(int code, byte[] responseBody = null)
        {
            var response = ReferencePool.Acquire<WebRequestResponse>();
            response.RawResponseBody = responseBody;
            response.ErrorCode = code;
            return response;
        }
    }
}
