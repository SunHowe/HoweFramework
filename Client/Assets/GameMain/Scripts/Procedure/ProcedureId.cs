namespace GameMain
{
    /// <summary>
    /// 流程id。
    /// </summary>
    public enum ProcedureId
    {
        /// <summary>
        /// 闪屏流程。
        /// </summary>
        Splash = 1,

        /// <summary>
        /// 加载数据表流程。
        /// </summary>
        LoadDataTable,

        /// <summary>
        /// 加载本地化流程。
        /// </summary>
        LoadLocalization,

        /// <summary>
        /// 初始化游戏系统流程。
        /// </summary>
        InitSystem,

        /// <summary>
        /// 登录流程。
        /// </summary>
        Login,
    }
}
