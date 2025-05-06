using HoweFramework;

namespace GameMain.UI.Login
{
    /// <summary>
    /// 登录账号响应。
    /// </summary>
    public sealed class LoginAccountResponse : ResponseBase
    {
        public string Account { get; set; }
        public string Password { get; set; }

        public override void Clear()
        {
            base.Clear();

            Account = null;
            Password = null;
        }

        /// <summary>
        /// 创建登录账号响应。
        /// </summary>
        /// <param name="account">账号。</param>
        /// <param name="password">密码。</param>
        /// <returns>登录响应。</returns>
        public static LoginAccountResponse Create(string account, string password)
        {
            var response = ReferencePool.Acquire<LoginAccountResponse>();
            response.Account = account;
            response.Password = password;
            return response;
        }
    }
}
