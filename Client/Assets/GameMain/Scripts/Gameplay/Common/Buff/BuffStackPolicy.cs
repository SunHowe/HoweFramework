namespace GameMain
{
    /// <summary>
    /// Buff叠加策略.
    /// </summary>
    public enum BuffStackPolicy
    {
        /// <summary>
        /// 独立共存：同一buffId的实例各自独立计时与生效.
        /// </summary>
        Independent,

        /// <summary>
        /// 刷新时长：重置剩余时间为配置时长，不增加层数.
        /// </summary>
        RefreshDuration,

        /// <summary>
        /// 叠加层数：层数+1（受MaxStack限制），刷新时长.
        /// </summary>
        Stack,

        /// <summary>
        /// 忽略：若buff已存在则不施加.
        /// </summary>
        Ignore,
    }
}
