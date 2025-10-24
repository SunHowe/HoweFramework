using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// GeekServer响应协议包。
    /// </summary>
    public sealed class GeekServerResponsePacket : GeekServerPacket, IResponse, IRemoteRequest
    {
        /// <summary>
        /// 请求id。
        /// </summary>
        public int RequestId
        {
            get => Message?.UniId ?? 0;
            set
            {
                if (Message != null)
                {
                    Message.UniId = value;
                }
            }
        }
        
        /// <summary>
        /// 错误码。
        /// </summary>
        public int ErrorCode
        {
            get
            {
                if (Message is ResponseMessage responseMessage)
                {
                    return responseMessage.ErrorCode;
                }
                return 0;
            }
            set
            {
                if (Message is ResponseMessage responseMessage)
                {
                    responseMessage.ErrorCode = value;
                }
            }
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }
    }
}