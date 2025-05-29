namespace GameMain
{
    /// <summary>
    /// 数值子类型。
    /// </summary>
    public enum NumericSubType : byte
    {
        /// <summary>
        /// 最终值。
        /// 最终值 = (基础值 * (1 + 基础百分比) + 基础常量) * (1 + 最终百分比) + 最终常量。
        /// </summary>
        Final,

        /// <summary>
        /// 基础值。
        /// </summary>
        Basic,

        /// <summary>
        /// 基础百分比。
        /// </summary>
        BasicPercent,

        /// <summary>
        /// 基础常量。
        /// </summary>
        BasicConstAdd,

        /// <summary>
        /// 最终百分比。
        /// </summary>
        FinalPercent,

        /// <summary>
        /// 最终常量。
        /// </summary>
        FinalConstAdd,
    }
}
