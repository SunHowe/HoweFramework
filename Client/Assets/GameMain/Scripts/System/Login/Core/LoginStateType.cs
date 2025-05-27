namespace GameMain
{
    /// <summary>
    /// 登录状态类型。
    /// </summary>
    public enum LoginStateType
    {
        /// <summary>
        /// 未登录。
        /// </summary>
        NoLogin,

        /// <summary>
        /// 连接中。
        /// </summary>
        Connect,

        /// <summary>
        /// 登录请求。
        /// </summary>
        LoginRequest,

        /// <summary>
        /// 进入游戏。
        /// </summary>
        OnGame,

        /// <summary>
        /// 断开连接。
        /// </summary>
        Disconnect,

        /// <summary>
        /// 重连。
        /// </summary>
        Reconnect,
    }
}
