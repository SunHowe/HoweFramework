namespace GameMain
{
    /// <summary>
    /// Buff运行实例接口. 提供给 <see cref="IBuffEffect"/> 访问 buff 上下文的能力.
    /// </summary>
    public interface IBuffInstance
    {
        /// <summary>
        /// Buff id.
        /// </summary>
        int BuffId { get; }

        /// <summary>
        /// 当前层数.
        /// </summary>
        int Stack { get; }

        /// <summary>
        /// 剩余时间（秒）.&lt; 0 表示永久.
        /// </summary>
        float RemainingTime { get; }

        /// <summary>
        /// Buff配置.
        /// </summary>
        IBuffConfig Config { get; }

        /// <summary>
        /// 拥有该 buff 的实体.
        /// </summary>
        IGameEntity Entity { get; }

        /// <summary>
        /// 所属的 BuffComponent.
        /// </summary>
        BuffComponent Component { get; }
    }
}
