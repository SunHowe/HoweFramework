using System;

namespace GameMain
{
    /// <summary>
    /// 数值 Buff 效果配置. 通过 <see cref="NumericComponent.Modify"/> 修改实体属性，
    /// 按当前层数缩放.
    /// </summary>
    [Serializable]
    public sealed class NumericBuffEffectConfig : IBuffEffectConfig
    {
        /// <summary>
        /// 属性 id.
        /// </summary>
        public int NumericId;

        /// <summary>
        /// 属性子类型.
        /// </summary>
        public NumericSubType SubType;

        /// <summary>
        /// 每层修改值. 正数增加，负数减少.
        /// </summary>
        public long Value;

        /// <inheritdoc/>
        public BuffEffectType EffectType => BuffEffectType.Numeric;
    }

    /// <summary>
    /// 数值 Buff 效果. 添加时按层数 Modify，移除时反向 Modify，层数变化时调整差值.
    /// </summary>
    public sealed class NumericBuffEffect : IBuffEffect
    {
        private int m_NumericId;
        private NumericSubType m_SubType;
        private long m_ValuePerStack;
        private long m_AppliedValue;

        /// <summary>
        /// 初始化效果.
        /// </summary>
        /// <param name="config">效果配置.</param>
        public void Initialize(NumericBuffEffectConfig config)
        {
            m_NumericId = config.NumericId;
            m_SubType = config.SubType;
            m_ValuePerStack = config.Value;
        }

        /// <inheritdoc/>
        public void OnAdd(IBuffInstance buff)
        {
            Apply(buff, buff.Stack);
        }

        /// <inheritdoc/>
        public void OnRemove(IBuffInstance buff)
        {
            Apply(buff, 0);
        }

        /// <inheritdoc/>
        public void OnStackChange(IBuffInstance buff, int oldStack, int newStack)
        {
            Apply(buff, newStack);
        }

        /// <inheritdoc/>
        public void OnTick(IBuffInstance buff)
        {
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            m_NumericId = 0;
            m_SubType = default;
            m_ValuePerStack = 0;
            m_AppliedValue = 0;
        }

        private void Apply(IBuffInstance buff, int targetStack)
        {
            var numericComponent = buff.Entity.GetComponent<NumericComponent>();
            if (numericComponent == null)
            {
                return;
            }

            var targetValue = m_ValuePerStack * targetStack;
            var diff = targetValue - m_AppliedValue;
            if (diff != 0)
            {
                numericComponent.Modify(m_NumericId, m_SubType, diff);
            }

            m_AppliedValue = targetValue;
        }
    }

    /// <summary>
    /// 数值 Buff 效果工厂.
    /// </summary>
    public sealed class NumericBuffEffectFactory : BuffEffectFactoryBase<NumericBuffEffectConfig, NumericBuffEffect>
    {
        /// <inheritdoc/>
        public override BuffEffectType EffectType => BuffEffectType.Numeric;

        /// <inheritdoc/>
        protected override void OnCreate(NumericBuffEffect effect, NumericBuffEffectConfig config)
        {
            effect.Initialize(config);
        }
    }
}
