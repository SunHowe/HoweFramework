using System;
using Cysharp.Threading.Tasks;
using Geek.Server.Proto;
using HoweFramework;
using UnityEngine;

namespace GameMain
{
    /// <summary>
    /// GeekServer登录系统。
    /// </summary>
    public sealed class GeekServerLoginSystem : SystemBase, ILoginSystem
    {
        /// <summary>
        /// 用户唯一ID。
        /// </summary>
        public Bindable<long> UserId { get; } = new Bindable<long>(0);

        /// <summary>
        /// 登录状态。
        /// </summary>
        public Bindable<LoginStateType> LoginState { get; } = new Bindable<LoginStateType>(LoginStateType.NoLogin);

        /// <summary>
        /// 请求重试次数。
        /// </summary>
        private const int RequestRetryCount = 3;

        private string m_IpAddress = "127.0.0.1";
        private int m_Port = 8899;

        /// <summary>
        /// 登录。
        /// </summary>
        /// <param name="account">账号。</param>
        /// <param name="password">密码。</param>
        /// <returns>登录结果。</returns>
        public async UniTask<int> Login(string account, string password)
        {
            if (LoginState.Value != LoginStateType.NoLogin)
            {
                return ErrorCode.InvalidOperationException;
            }

            LoginState.Value = LoginStateType.Connect;

            var code = await NetworkModule.Instance.ConnectAsync(m_IpAddress, m_Port).GetErrorCode();
            if (code != 0)
            {
                LoginState.Value = LoginStateType.NoLogin;
                return code;
            }

            LoginState.Value = LoginStateType.LoginRequest;

            var retryCount = 0;
            do
            {
                var req = new LoginReq();
                req.SdkType = 0;
                req.SdkToken = password;
                req.UserName = account;
                req.Device = SystemInfo.deviceUniqueIdentifier;
                req.Platform = "android";

                using var response = await req.SendPacketAsync<LoginResp>();
                if (response.ErrorCode == 0)
                {
                    code = 0;
                    UserId.Value = response.UserInfo.RoleId;
                    break;
                }

                code = response.ErrorCode;
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

        protected override void OnDestroy()
        {
        }

        protected override void OnInit()
        {
        }
    }
}