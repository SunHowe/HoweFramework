using System.Net;
using Cysharp.Threading.Tasks;
using HoweFramework;
using Protocol;

namespace GameMain
{
    /// <summary>
    /// 登录模块。
    /// </summary>
    public sealed class LoginModule : ModuleBase<LoginModule>
    {
        /// <summary>
        /// 请求重试次数。
        /// </summary>
        private const int RequestRetryCount = 3;

        /// <summary>
        /// 登录状态。
        /// </summary>
        public LoginStateType LoginState { get; private set; }

        /// <summary>
        /// 登录。
        /// </summary>
        /// <param name="ipAddress">IP地址。</param>
        /// <param name="port">端口。</param>
        /// <param name="account">账号。</param>
        /// <param name="password">密码。</param>
        public async UniTask<int> Login(string ipAddress, int port, string account, string password)
        {
            if (LoginState != LoginStateType.NoLogin)
            {
                return ErrorCode.InvalidOperationException;
            }

            ChangeLoginState(LoginStateType.Connect);

            var code = await NetworkModule.Instance.ConnectAsync(ipAddress, port).GetErrorCode();
            if (code != 0)
            {
                ChangeLoginState(LoginStateType.NoLogin);
                return code;
            }

            ChangeLoginState(LoginStateType.LoginRequest);

            var retryCount = 0;
            do
            {
                code = await LoginRequest.Create(account, password).SendPacketAsync().GetErrorCode();
                ++retryCount;
            }
            while (code != 0 && retryCount < RequestRetryCount);

            if (code != 0)
            {
                ChangeLoginState(LoginStateType.NoLogin);
                NetworkModule.Instance.Disconnect();
                return code;
            }

            ChangeLoginState(LoginStateType.OnGame);
            Log.Info("登录成功");

            return ErrorCode.Success;
        }

        protected override void OnInit()
        {
        }

        protected override void OnDestroy()
        {
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }

        private void ChangeLoginState(LoginStateType loginState)
        {
            LoginState = loginState;
            EventModule.Instance.Dispatch(this, LoginStateChangeEventArgs.Create(loginState));
        }
    }
}
