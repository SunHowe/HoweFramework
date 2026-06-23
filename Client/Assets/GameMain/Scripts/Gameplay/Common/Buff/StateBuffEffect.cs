using System;

namespace GameMain
{
    /// <summary>
    /// 状态 Buff 效果配置. 通过 <see cref="StateComponent"/> 以引用计数方式添加状态.
    /// </summary>
    [Serializable]
    public sealed class StateBuffEffectConfig : IBuffEffectConfig
    {
        /// <summary>
        /// 状态 id.
        /// </summary>
        public int StateId;

        /// <inheritdoc/>
        public BuffEffectType EffectType => BuffEffectType.State;
    }

    /// <summary>
    /// 状态 Buff 效果. 添加时 AddState（以自身为 provider），移除时 RemoveState，
    /// 利用引用计数自动处理同状态叠加.
    /// </summary>
    public sealed class StateBuffEffect : IBuffEffect
    {
        private int m_StateId;
        private bool m_Applied;

        /// <summary>
        /// 初始化效果.
        /// </summary>
        /// <param name="config">效果配置.</param>
        public void Initialize(StateBuffEffectConfig config)
        {
            m_StateId = config.StateId;
        }

        /// <inheritdoc/>
        public void OnAdd(IBuffInstance buff)
        {
            var stateComponent = buff.Entity.GetComponent<StateComponent>();
            if (stateComponent == null)
            {
                return;
            }

            stateComponent.AddState(m_StateId, this);
            m_Applied = true;
        }

        /// <inheritdoc/>
        public void OnRemove(IBuffInstance buff)
        {
            if (!m_Applied)
            {
                return;
            }

            var stateComponent = buff.Entity.GetComponent<StateComponent>();
            if (stateComponent != null)
            {
                stateComponent.RemoveState(m_StateId, this);
            }

            m_Applied = false;
        }

        /// <inheritdoc/>
        public void OnStackChange(IBuffInstance buff, int oldStack, int newStack)
        {
        }

        /// <inheritdoc/>
        public void OnTick(IBuffInstance buff)
        {
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            m_StateId = 0;
            m_Applied = false;
        }
    }

    /// <summary>
    /// 状态 Buff 效果工厂.
    /// </summary>
    public sealed class StateBuffEffectFactory : BuffEffectFactoryBase<StateBuffEffectConfig, StateBuffEffect>
    {
        /// <inheritdoc/>
        public override BuffEffectType EffectType => BuffEffectType.State;

        /// <inheritdoc/>
        protected override void OnCreate(StateBuffEffect effect, StateBuffEffectConfig config)
        {
            effect.Initialize(config);
        }
    }
}
