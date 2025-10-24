using System;
using Cysharp.Threading.Tasks;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 离线登录系统。
    /// </summary>
    public sealed class OfflineLoginSystem : SystemBase, ILoginSystem
    {
        /// <summary>
        /// 用户唯一ID。
        /// </summary>
        public Bindable<long> UserId { get; } = new Bindable<long>(0);

        public Bindable<LoginStateType> LoginState { get; } = new Bindable<LoginStateType>(LoginStateType.NoLogin);

        /// <summary>
        /// 登录。
        /// </summary>
        /// <param name="account">账号。</param>
        /// <param name="password">密码。</param>
        /// <returns>登录结果。</returns>
        public UniTask<int> Login(string account, string password)
        {
            if (LoginState.Value != LoginStateType.NoLogin)
            {
                return UniTask.FromResult(ErrorCode.InvalidOperationException);
            }

            UserId.Value = 100000;
            LoginState.Value = LoginStateType.OnGame;
            return UniTask.FromResult(0);
        }

        protected override void OnInit()
        {
        }

        protected override void OnDestroy()
        {
        }
    }
}
