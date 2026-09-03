namespace GameMain
{
    /// <summary>
    /// 同一技能定义能否在一个 ASC 上并发激活。
    /// </summary>
    public enum AbilityInstancingPolicy : byte
    {
        /// <summary>
        /// 每个定义同时只能有一份激活实例。
        /// </summary>
        Single = 0,

        /// <summary>
        /// 允许同一定义的多份激活实例。
        /// </summary>
        Multiple = 1,
    }
}
