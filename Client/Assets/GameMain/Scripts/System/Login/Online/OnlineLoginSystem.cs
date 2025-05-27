using System;
using Cysharp.Threading.Tasks;
using HoweFramework;
using Protocol;

namespace GameMain
{
    /// <summary>
    /// 在线登录系统。
    /// </summary>
    public sealed class OnlineLoginSystem : SystemBase, ILoginSystem
    {
        /// <summary>
        /// 用户唯一ID。
        /// </summary>
        public Bindable<Guid> UserId { get; } = new Bindable<Guid>(Guid.Empty);

        /// <summary>
        /// 登录状态。
        /// </summary>
        public Bindable<LoginStateType> LoginState { get; } = new Bindable<LoginStateType>(LoginStateType.NoLogin);

        /// <summary>
        /// 请求重试次数。
        /// </summary>
        private const int RequestRetryCount = 3;

        public async UniTask<int> Login(string ipAddress, int port, string account, string password)
        {
            if (LoginState.Value != LoginStateType.NoLogin)
            {
                return ErrorCode.InvalidOperationException;
            }

            LoginState.Value = LoginStateType.Connect;

            var code = await NetworkModule.Instance.ConnectAsync(ipAddress, port).GetErrorCode();
            if (code != 0)
            {
                LoginState.Value = LoginStateType.NoLogin;
                return code;
            }

            LoginState.Value = LoginStateType.LoginRequest;

            var retryCount = 0;
            do
            {
                using var response = await LoginRequest.Create(account, password).SendPacketAsync().As<LoginResponse>();
                if (response.ErrorCode == 0)
                {
                    code = 0;
                    UserId.Value = response.UserId;
                    break;
                }
                
                ++retryCount;
            }
            while (retryCount < RequestRetryCount);

            if (code != 0)
            {
                LoginState.Value = LoginStateType.NoLogin;
                NetworkModule.Instance.Disconnect();
                return code;
            }

            LoginState.Value = LoginStateType.OnGame;
            Log.Info("登录成功");

            return ErrorCode.Success;
        }

        protected override void OnInit()
        {
        }

        protected override void OnDestroy()
        {
        }
    }
}
