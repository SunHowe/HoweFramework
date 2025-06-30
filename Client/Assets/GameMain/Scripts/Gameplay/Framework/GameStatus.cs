namespace GameMain
{
    /// <summary>
    /// 玩法状态.
    /// </summary>
    public enum GameStatus
    {
        /// <summary>
        /// 未运行。
        /// </summary>
        None,

        /// <summary>
        /// 初始化。
        /// </summary>
        Initialize,

        /// <summary>
        /// 运行中。
        /// </summary>
        Running,

        /// <summary>
        /// 暂停。
        /// </summary>
        Pause,

        /// <summary>
        /// 停止。
        /// </summary>
        Stopped,
    }
}