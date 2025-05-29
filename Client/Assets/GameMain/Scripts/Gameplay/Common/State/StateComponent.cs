using System.Collections.Generic;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 状态组件。
    /// </summary>
    [GameComponent(GameComponentType.State)]
    public sealed class StateComponent : GameComponentBase
    {
        /// <summary>
        /// 状态字典。
        /// </summary>
        private readonly Dictionary<int, ReusableHashSet<object>> m_StateDict = new();

        /// <summary>
        /// 状态事件字典。
        /// </summary>
        private readonly Dictionary<int, SimpleEvent<bool>> m_StateEventDict = new();

        /// <summary>
        /// 检查状态。
        /// </summary>
        public bool CheckState(int state)
        {
            return m_StateDict.ContainsKey(state);
        }

        /// <summary>
        /// 添加状态。
        /// </summary>
        /// <param name="state">状态。</param>
        /// <param name="provider">提供者。</param>
        public void AddState(int state, object provider)
        {
            if (!m_StateDict.TryGetValue(state, out var set))
            {
                set = ReusableHashSet<object>.Create();
                m_StateDict[state] = set;
                set.Add(provider);

                DispatchStateEvent(state, true);
            }
            else
            {
                set.Add(provider);
            }
        }

        /// <summary>
        /// 移除状态。
        /// </summary>
        /// <param name="state">状态。</param>
        /// <param name="provider">提供者。</param>
        public void RemoveState(int state, object provider)
        {
            if (!m_StateDict.TryGetValue(state, out var set))
            {
                return;
            }

            set.Remove(provider);

            if (set.Count > 0)
            {
                return;
            }

            m_StateDict.Remove(state);
            set.Dispose();

            DispatchStateEvent(state, false);
        }

        /// <summary>
        /// 订阅状态事件。
        /// </summary>
        /// <param name="state">状态。</param>
        /// <param name="handler">状态事件。</param>
        /// <param name="notifyImmediately">是否立即通知。</param>
        public void Subscribe(int state, SimpleEventHandler<bool> handler, bool notifyImmediately = false)
        {
            if (!m_StateEventDict.TryGetValue(state, out var eventObj))
            {
                eventObj = SimpleEvent<bool>.Create();
                m_StateEventDict[state] = eventObj;
            }

            eventObj.Subscribe(handler);

            if (notifyImmediately)
            {
                handler(CheckState(state));
            }
        }

        /// <summary>
        /// 取消订阅状态事件。
        /// </summary>
        /// <param name="state">状态。</param>
        /// <param name="handler">状态事件。</param>
        public void Unsubscribe(int state, SimpleEventHandler<bool> handler)
        {
            if (!m_StateEventDict.TryGetValue(state, out var eventObj))
            {
                return;
            }

            eventObj.Unsubscribe(handler);
        }

        /// <summary>
        /// 派发状态事件。
        /// </summary>
        /// <param name="state">状态。</param>
        /// <param name="exists">是否存在。</param>
        private void DispatchStateEvent(int state, bool exists)
        {
            if (!m_StateEventDict.TryGetValue(state, out var eventObj))
            {
                return;
            }

            eventObj.Dispatch(exists);
        }

        protected override void OnAwake()
        {
        }

        protected override void OnDispose()
        {
            foreach (var eventObj in m_StateEventDict.Values)
            {
                eventObj.Dispose();
            }

            m_StateEventDict.Clear();

            foreach (var set in m_StateDict.Values)
            {
                set.Dispose();
            }

            m_StateDict.Clear();
        }
    }
}
