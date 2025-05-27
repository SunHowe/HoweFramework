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
        public Bindable<Guid> UserId { get; } = new Bindable<Guid>(Guid.Empty);

        /// <summary>
        /// 登录。
        /// </summary>
        /// <param name="account">账号。</param>
        /// <param name="password">密码。</param>
        /// <returns>登录结果。</returns>
        public UniTask<int> Login(string account, string password)
        {
            UserId.Value = Guid.NewGuid();
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
