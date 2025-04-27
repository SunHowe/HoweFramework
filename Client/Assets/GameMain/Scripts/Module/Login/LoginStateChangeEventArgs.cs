using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 登录状态改变事件参数。
    /// </summary>
    public sealed class LoginStateChangeEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(LoginStateChangeEventArgs).GetHashCode();

        public override int Id => EventId;

        public LoginStateType LoginState { get; private set; }
        
        public override void Clear()
        {
            LoginState = LoginStateType.NoLogin;
        }

        public static LoginStateChangeEventArgs Create(LoginStateType loginState)
        {
            var eventArgs = ReferencePool.Acquire<LoginStateChangeEventArgs>();
            eventArgs.LoginState = loginState;
            return eventArgs;
        }
    }
}
