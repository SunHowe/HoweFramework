using System;
using Cysharp.Threading.Tasks;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 登录系统接口。
    /// </summary>
    public interface ILoginSystem : ISystem
    {
        /// <summary>
        /// 用户唯一ID。
        /// </summary>
        Bindable<Guid> UserId { get; }

        /// <summary>
        /// 登录。
        /// </summary>
        /// <param name="account">账号。</param>
        /// <param name="password">密码。</param>
        /// <returns>登录结果。</returns>
        UniTask<int> Login(string account, string password);
    }
}
