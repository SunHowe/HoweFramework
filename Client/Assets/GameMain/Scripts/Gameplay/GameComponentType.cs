namespace GameMain
{
    /// <summary>
    /// 游戏组件类型。
    /// </summary>
    public enum GameComponentType
    {
        #region [通用组件 1-1000]

        /// <summary>
        /// Transform组件。
        /// </summary>
        Transform = 1,

        /// <summary>
        /// 视图组件。
        /// </summary>
        View = 2,

        /// <summary>
        /// 视图Transform同步组件。
        /// </summary>
        ViewTransformSync = 3,

        /// <summary>
        /// 数值组件。
        /// </summary>
        Numeric = 4,

        /// <summary>
        /// 状态组件。
        /// </summary>
        State = 5,

        /// <summary>
        /// 资源组件。
        /// </summary>
        Resource = 6,

        #endregion

        Snake = 1000,
        SnakeNode,
    }
}
