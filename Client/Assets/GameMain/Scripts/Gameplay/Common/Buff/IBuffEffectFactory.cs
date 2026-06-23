namespace GameMain
{
    /// <summary>
    /// Buff效果工厂接口. 负责根据效果配置创建效果实例.
    /// 注册到 <see cref="IBuffManager"/> 中按 <see cref="EffectType"/> 索引.
    /// </summary>
    public interface IBuffEffectFactory
    {
        /// <summary>
        /// 工厂处理的效果类型.
        /// </summary>
        BuffEffectType EffectType { get; }

        /// <summary>
        /// 创建效果实例.
        /// </summary>
        /// <param name="config">效果配置.</param>
        /// <returns>效果实例.</returns>
        IBuffEffect Create(IBuffEffectConfig config);
    }

    /// <summary>
    /// Buff效果工厂基类. 简化具体工厂的实现.
    /// </summary>
    /// <typeparam name="TConfig">效果配置类型.</typeparam>
    /// <typeparam name="TEffect">效果类型.</typeparam>
    public abstract class BuffEffectFactoryBase<TConfig, TEffect> : IBuffEffectFactory
        where TConfig : IBuffEffectConfig
        where TEffect : IBuffEffect, new()
    {
        /// <inheritdoc/>
        public abstract BuffEffectType EffectType { get; }

        /// <inheritdoc/>
        public IBuffEffect Create(IBuffEffectConfig config)
        {
            var effect = new TEffect();
            OnCreate(effect, (TConfig)config);
            return effect;
        }

        /// <summary>
        /// 创建效果实例时的初始化回调.
        /// </summary>
        /// <param name="effect">效果实例.</param>
        /// <param name="config">效果配置.</param>
        protected abstract void OnCreate(TEffect effect, TConfig config);
    }
}
