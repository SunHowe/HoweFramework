namespace GameMain
{
    /// <summary>
    /// 修正写入通道。替代参考实现里一律改 Attribute Base/Current。
    /// </summary>
    public enum GameplayModifierChannel : byte
    {
        /// <summary>
        /// Instant 或 Period&gt;0 时执行到 Resource；否则作为 Numeric GAS 叠加层。
        /// </summary>
        Auto = 0,

        /// <summary>
        /// 以 ASC 为 source 写入 Numeric 的 Basic/Final 百分比与常量层，或 Override。
        /// </summary>
        Overlay = 1,

        /// <summary>
        /// 立即改写 Numeric.Basic。
        /// </summary>
        ExecuteNumeric = 2,

        /// <summary>
        /// 立即改写 Resource 当前值。
        /// </summary>
        ExecuteResource = 3,
    }
}
