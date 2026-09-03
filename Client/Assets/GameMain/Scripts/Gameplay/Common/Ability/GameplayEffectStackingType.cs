namespace GameMain
{
    /// <summary>
    /// 同一效果定义再次施加时如何与已有实例交互。仅对 Duration / Infinite 有意义。
    /// </summary>
    public enum GameplayEffectStackingType : byte
    {
        /// <summary>
        /// 每个定义一份实例，再次施加刷新剩余时间。
        /// </summary>
        RefreshDuration = 0,

        /// <summary>
        /// 每个定义一份实例，已存在则忽略再次施加。
        /// </summary>
        None = 1,

        /// <summary>
        /// 每次施加都是独立实例，不共享层数上限。
        /// </summary>
        AggregateByTarget = 2,

        /// <summary>
        /// 最多 <see cref="GameplayEffectDef.MaxStackCount"/> 层，满层后刷新最早到期的实例。
        /// </summary>
        StackCount = 3,
    }
}
