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
        /// 加载模块流程。
        /// </summary>
        LoadModule,

        /// <summary>
        /// 登录流程。
        /// </summary>
        Login,
    }
}
