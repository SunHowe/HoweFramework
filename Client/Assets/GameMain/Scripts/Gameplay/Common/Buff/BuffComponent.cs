using System.Collections.Generic;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// Buff组件. 为实体提供通用的 Buff 管理能力，
    /// 可结合 <see cref="StateComponent"/> 控制角色状态、
    /// 结合 <see cref="NumericComponent"/> 影响角色属性，
    /// 并支持通过可插拔的 <see cref="IBuffEffect"/> 在 buff 生命周期执行指定行为.
    /// </summary>
    [GameComponent(GameComponentType.Buff)]
    public sealed class BuffComponent : GameComponentBase
    {
        private readonly Dictionary<int, BuffInfo> m_BuffDict = new();
        private readonly List<BuffInfo> m_ExpiredBuffList = new();
        private SimpleEvent<IBuffInstance> m_BuffAddedEvent;
        private SimpleEvent<IBuffInstance> m_BuffRemovedEvent;
        private SimpleEvent<IBuffInstance> m_BuffRefreshedEvent;

        private IBuffManager m_BuffManager;
        private IGameUpdateManager m_GameUpdateManager;

        /// <summary>
        /// 当前 buff 数量.
        /// </summary>
        public int BuffCount => m_BuffDict.Count;

        /// <summary>
        /// 添加 Buff.
        /// </summary>
        /// <param name="buffId">Buff id.</param>
        public void AddBuff(int buffId)
        {
            var config = m_BuffManager?.GetBuffConfig(buffId);
            if (config == null)
            {
                Log.Warning($"未注册 Buff 配置: {buffId}.");
                return;
            }

            if (m_BuffDict.TryGetValue(buffId, out var existing))
            {
                ApplyStackPolicy(existing, config);
                return;
            }

            CreateBuff(buffId, config);
        }

        /// <summary>
        /// 移除 Buff.
        /// </summary>
        /// <param name="buffId">Buff id.</param>
        public void RemoveBuff(int buffId)
        {
            if (!m_BuffDict.TryGetValue(buffId, out var buff))
            {
                return;
            }

            RemoveBuffInternal(buff);
        }

        /// <summary>
        /// 移除所有 Buff.
        /// </summary>
        public void RemoveAllBuffs()
        {
            using var buffList = ReusableList<BuffInfo>.Create();
            buffList.AddRange(m_BuffDict.Values);

            foreach (var buff in buffList)
            {
                RemoveBuffInternal(buff);
            }
        }

        /// <summary>
        /// 检查 Buff 是否存在.
        /// </summary>
        /// <param name="buffId">Buff id.</param>
        /// <returns>是否存在.</returns>
        public bool HasBuff(int buffId)
        {
            return m_BuffDict.ContainsKey(buffId);
        }

        /// <summary>
        /// 获取 Buff 层数.
        /// </summary>
        /// <param name="buffId">Buff id.</param>
        /// <returns>层数，不存在返回 0.</returns>
        public int GetBuffStack(int buffId)
        {
            return m_BuffDict.TryGetValue(buffId, out var buff) ? buff.Stack : 0;
        }

        /// <summary>
        /// 获取 Buff 剩余时间.
        /// </summary>
        /// <param name="buffId">Buff id.</param>
        /// <returns>剩余时间（秒），永久返回 -1，不存在返回 0.</returns>
        public float GetBuffRemainingTime(int buffId)
        {
            return m_BuffDict.TryGetValue(buffId, out var buff) ? buff.RemainingTime : 0;
        }

        /// <summary>
        /// 订阅 Buff 添加事件.
        /// </summary>
        /// <param name="handler">事件处理函数.</param>
        public void SubscribeBuffAdded(SimpleEventHandler<IBuffInstance> handler)
        {
            m_BuffAddedEvent.Subscribe(handler);
        }

        /// <summary>
        /// 取消订阅 Buff 添加事件.
        /// </summary>
        /// <param name="handler">事件处理函数.</param>
        public void UnsubscribeBuffAdded(SimpleEventHandler<IBuffInstance> handler)
        {
            m_BuffAddedEvent.Unsubscribe(handler);
        }

        /// <summary>
        /// 订阅 Buff 移除事件.
        /// </summary>
        /// <param name="handler">事件处理函数.</param>
        public void SubscribeBuffRemoved(SimpleEventHandler<IBuffInstance> handler)
        {
            m_BuffRemovedEvent.Subscribe(handler);
        }

        /// <summary>
        /// 取消订阅 Buff 移除事件.
        /// </summary>
        /// <param name="handler">事件处理函数.</param>
        public void UnsubscribeBuffRemoved(SimpleEventHandler<IBuffInstance> handler)
        {
            m_BuffRemovedEvent.Unsubscribe(handler);
        }

        /// <summary>
        /// 订阅 Buff 刷新事件（再次施加已有 buff 时触发）.
        /// </summary>
        /// <param name="handler">事件处理函数.</param>
        public void SubscribeBuffRefreshed(SimpleEventHandler<IBuffInstance> handler)
        {
            m_BuffRefreshedEvent.Subscribe(handler);
        }

        /// <summary>
        /// 取消订阅 Buff 刷新事件.
        /// </summary>
        /// <param name="handler">事件处理函数.</param>
        public void UnsubscribeBuffRefreshed(SimpleEventHandler<IBuffInstance> handler)
        {
            m_BuffRefreshedEvent.Unsubscribe(handler);
        }

        protected override void OnAwake()
        {
            m_BuffManager = GetManager<IBuffManager>();
            m_GameUpdateManager = GetManager<IGameUpdateManager>();
            m_GameUpdateManager.RegisterFixedUpdate(this, OnUpdate);

            m_BuffAddedEvent = SimpleEvent<IBuffInstance>.Create();
            m_BuffRemovedEvent = SimpleEvent<IBuffInstance>.Create();
            m_BuffRefreshedEvent = SimpleEvent<IBuffInstance>.Create();
        }

        protected override void OnDispose()
        {
            m_GameUpdateManager.UnregisterFixedUpdate(this, OnUpdate);

            using var buffList = ReusableList<BuffInfo>.Create();
            buffList.AddRange(m_BuffDict.Values);

            foreach (var buff in buffList)
            {
                buff.FireRemove();
                buff.Dispose();
            }

            m_BuffDict.Clear();
            m_ExpiredBuffList.Clear();

            m_BuffAddedEvent.Dispose();
            m_BuffRemovedEvent.Dispose();
            m_BuffRefreshedEvent.Dispose();

            m_BuffAddedEvent = null;
            m_BuffRemovedEvent = null;
            m_BuffRefreshedEvent = null;

            m_BuffManager = null;
            m_GameUpdateManager = null;
        }

        private void CreateBuff(int buffId, IBuffConfig config)
        {
            var buff = BuffInfo.Create(buffId, config, this);

            for (var i = 0; i < config.EffectConfigs.Count; i++)
            {
                var effectConfig = config.EffectConfigs[i];
                var effect = m_BuffManager.CreateEffect(effectConfig);
                if (effect != null)
                {
                    buff.AddEffect(effect);
                }
            }

            m_BuffDict[buffId] = buff;

            buff.FireAdd();

            m_BuffAddedEvent.Dispatch(buff);
        }

        private void ApplyStackPolicy(BuffInfo buff, IBuffConfig config)
        {
            switch (config.StackPolicy)
            {
                case BuffStackPolicy.Ignore:
                    return;

                case BuffStackPolicy.RefreshDuration:
                    buff.RefreshDuration();
                    break;

                case BuffStackPolicy.Stack:
                    if (config.MaxStack <= 0 || buff.Stack < config.MaxStack)
                    {
                        buff.AddStack(1);
                    }

                    buff.RefreshDuration();
                    break;

                case BuffStackPolicy.Independent:
                    buff.AddStack(1);
                    break;
            }

            m_BuffRefreshedEvent.Dispatch(buff);
        }

        private void RemoveBuffInternal(BuffInfo buff)
        {
            if (!m_BuffDict.Remove(buff.BuffId))
            {
                return;
            }

            buff.FireRemove();
            m_BuffRemovedEvent.Dispatch(buff);
            buff.Dispose();
        }

        private void OnUpdate(float elapseSeconds)
        {
            m_ExpiredBuffList.Clear();

            foreach (var buff in m_BuffDict.Values)
            {
                buff.UpdateTime(elapseSeconds);

                if (buff.ShouldTick)
                {
                    buff.ConsumeTick();
                    buff.FireTick();
                }

                if (buff.IsExpired)
                {
                    m_ExpiredBuffList.Add(buff);
                }
            }

            foreach (var buff in m_ExpiredBuffList)
            {
                RemoveBuffInternal(buff);
            }

            m_ExpiredBuffList.Clear();
        }

        /// <summary>
        /// Buff运行实例.
        /// </summary>
        private sealed class BuffInfo : IBuffInstance, IReference
        {
            private readonly List<IBuffEffect> m_Effects = new();

            public int BuffId { get; private set; }
            public int Stack { get; private set; }
            public float RemainingTime { get; private set; }
            public IBuffConfig Config { get; private set; }
            public IGameEntity Entity { get; private set; }
            public BuffComponent Component { get; private set; }

            private float m_TickTimer;

            public bool IsExpired => Config.Duration > 0 && RemainingTime <= 0;

            public bool ShouldTick => Config.TickInterval > 0 && m_TickTimer <= 0;

            public void AddEffect(IBuffEffect effect)
            {
                m_Effects.Add(effect);
            }

            public void SetStack(int stack)
            {
                if (Stack == stack)
                {
                    return;
                }

                var oldStack = Stack;
                Stack = stack;

                for (var i = 0; i < m_Effects.Count; i++)
                {
                    m_Effects[i].OnStackChange(this, oldStack, stack);
                }
            }

            public void AddStack(int delta)
            {
                SetStack(Stack + delta);
            }

            public void RefreshDuration()
            {
                if (Config.Duration > 0)
                {
                    RemainingTime = Config.Duration;
                }
            }

            public void UpdateTime(float deltaTime)
            {
                if (RemainingTime > 0)
                {
                    RemainingTime -= deltaTime;
                    if (RemainingTime < 0)
                    {
                        RemainingTime = 0;
                    }
                }

                if (m_TickTimer > 0)
                {
                    m_TickTimer -= deltaTime;
                }
            }

            public void ConsumeTick()
            {
                m_TickTimer += Config.TickInterval;
            }

            public void FireAdd()
            {
                for (var i = 0; i < m_Effects.Count; i++)
                {
                    m_Effects[i].OnAdd(this);
                }
            }

            public void FireRemove()
            {
                for (var i = 0; i < m_Effects.Count; i++)
                {
                    m_Effects[i].OnRemove(this);
                }
            }

            public void FireTick()
            {
                for (var i = 0; i < m_Effects.Count; i++)
                {
                    m_Effects[i].OnTick(this);
                }
            }

            public void Clear()
            {
                for (var i = 0; i < m_Effects.Count; i++)
                {
                    m_Effects[i].Dispose();
                }

                m_Effects.Clear();

                BuffId = 0;
                Stack = 0;
                RemainingTime = 0;
                Config = null;
                Entity = null;
                Component = null;
                m_TickTimer = 0;
            }

            public void Dispose()
            {
                ReferencePool.Release(this);
            }

            public static BuffInfo Create(int buffId, IBuffConfig config, BuffComponent component)
            {
                var info = ReferencePool.Acquire<BuffInfo>();
                info.BuffId = buffId;
                info.Config = config;
                info.Component = component;
                info.Entity = component.Entity;
                info.Stack = 1;
                info.RemainingTime = config.Duration > 0 ? config.Duration : -1;
                info.m_TickTimer = config.TickInterval > 0 ? config.TickInterval : -1;
                return info;
            }
        }
    }
}
