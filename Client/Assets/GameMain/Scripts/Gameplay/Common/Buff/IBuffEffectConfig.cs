namespace GameMain
{
    /// <summary>
    /// Buff效果配置接口. 每种效果类型自行实现具体配置.
    /// </summary>
    public interface IBuffEffectConfig
    {
        /// <summary>
        /// 效果类型. 用于在 <see cref="IBuffManager"/> 中查找对应工厂.
        /// </summary>
        BuffEffectType EffectType { get; }
    }
}
