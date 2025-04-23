namespace HoweFramework
{
    /// <summary>
    /// 通用响应包实例。
    /// </summary>
    public sealed class CommonResponse : ResponseBase
    {
        /// <summary>
        /// 业务透传数据。
        /// </summary>
        public object UserData { get; set; }

        public static CommonResponse Create(int errorCode, object userData = null)
        {
            var response = ReferencePool.Acquire<CommonResponse>();
            response.ErrorCode = errorCode;
            response.UserData = userData;
            return response;
        }

        public override void Clear()
        {
            base.Clear();

            if (UserData is IReference reference)
            {
                ReferencePool.Release(reference);
            }

            UserData = null;
        }
    }
}
