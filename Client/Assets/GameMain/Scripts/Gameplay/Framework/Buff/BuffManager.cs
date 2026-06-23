using System.Collections.Generic;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// Buff管理器. 负责 Buff 配置注册与效果工厂管理.
    /// </summary>
    public sealed class BuffManager : GameManagerBase, IBuffManager
    {
        private readonly Dictionary<int, IBuffConfig> m_BuffConfigDict = new();
        private readonly Dictionary<BuffEffectType, IBuffEffectFactory> m_EffectFactoryDict = new();

        /// <inheritdoc/>
        public void RegisterBuffConfig(IBuffConfig config)
        {
            if (config == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "Buff配置不能为空.");
            }

            m_BuffConfigDict[config.BuffId] = config;
        }

        /// <inheritdoc/>
        public void UnregisterBuffConfig(int buffId)
        {
            m_BuffConfigDict.Remove(buffId);
        }

        /// <inheritdoc/>
        public IBuffConfig GetBuffConfig(int buffId)
        {
            return m_BuffConfigDict.TryGetValue(buffId, out var config) ? config : null;
        }

        /// <inheritdoc/>
        public void RegisterEffectFactory(IBuffEffectFactory factory)
        {
            if (factory == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "效果工厂不能为空.");
            }

            m_EffectFactoryDict[factory.EffectType] = factory;
        }

        /// <inheritdoc/>
        public void UnregisterEffectFactory(BuffEffectType effectType)
        {
            m_EffectFactoryDict.Remove(effectType);
        }

        /// <inheritdoc/>
        public IBuffEffect CreateEffect(IBuffEffectConfig config)
        {
            if (config == null)
            {
                return null;
            }

            if (!m_EffectFactoryDict.TryGetValue(config.EffectType, out var factory))
            {
                Log.Warning($"未注册效果类型 {config.EffectType} 的工厂.");
                return null;
            }

            return factory.Create(config);
        }

        protected override void OnAwake()
        {
            RegisterEffectFactory(new NumericBuffEffectFactory());
            RegisterEffectFactory(new StateBuffEffectFactory());
        }

        protected override void OnDispose()
        {
            m_BuffConfigDict.Clear();
            m_EffectFactoryDict.Clear();
        }
    }
}
