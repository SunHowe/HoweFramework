using System.Collections.Generic;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 数值组件。
    /// </summary>
    [GameComponent(GameComponentType.Numeric)]
    public sealed class NumericComponent : GameComponentBase, INumeric
    {
        /// <summary>
        /// 数值字典。
        /// </summary>
        private readonly Dictionary<int, long> m_NumericDict = new();

        /// <summary>
        /// 数值变更事件字典。
        /// </summary>
        private readonly Dictionary<int, SimpleEvent<long>> m_NumericChangeEventDict = new();

        /// <summary>
        /// 数值字典。
        /// </summary>
        public IReadOnlyDictionary<int, long> NumericDict => m_NumericDict;

        /// <summary>
        /// 获取最终值。
        /// </summary>
        public long this[int id]
        {
            get => Get(id, NumericSubType.Final);
            set => Set(id, NumericSubType.Final, value, true);
        }

        /// <summary>
        /// 获取属性值。
        /// </summary>
        public long Get(int id, NumericSubType subType = NumericSubType.Final)
        {
            return GetByKey(NumericHelper.EncodeNumericKey(id, subType));
        }

        /// <summary>
        /// 获取属性值。
        /// </summary>
        /// <param name="key">属性键值。</param>
        /// <returns>属性值。</returns>
        public long GetByKey(int key)
        {
            return m_NumericDict.TryGetValue(key, out var value) ? value : 0;
        }

        /// <summary>
        /// 设置属性值。
        /// </summary>
        public void Set(int id, NumericSubType subType, long value, bool dispatchEvent = true)
        {
            
        }

        /// <summary>
        /// 设置属性值。
        /// </summary>
        /// <param name="key">属性键值。</param>
        /// <param name="value">属性值。</param>
        /// <param name="dispatchEvent">是否派发事件。</param>
        public void SetByKey(int key, long value, bool dispatchEvent = true)
        {
            var (id, subType) = NumericHelper.DecodeNumericKey(key);
            if (subType == NumericSubType.Final)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "不允许直接设置最终值。");
            }

            var finalKey = NumericHelper.EncodeNumericKey(id, NumericSubType.Final);

            m_NumericDict[key] = value;

            var basicValue = Get(id, NumericSubType.Basic);
            var basicPercent = Get(id, NumericSubType.BasicPercent);
            var basicConstAdd = Get(id, NumericSubType.BasicConstAdd);
            var finalPercent = Get(id, NumericSubType.FinalPercent);
            var finalConstAdd = Get(id, NumericSubType.FinalConstAdd);

            var finalValue = (basicValue * (100 + basicPercent) / 100 + basicConstAdd) * (100 + finalPercent) / 100 + finalConstAdd;
            m_NumericDict[finalKey] = finalValue;

            if (!dispatchEvent)
            {
                return;
            }

            // 派发子属性变更事件。
            DispatchNumericChangeEvent(key, value);

            // 派发最终值变更事件。
            DispatchNumericChangeEvent(finalKey, finalValue);
        }

        /// <summary>
        /// 修改属性值。
        /// </summary>
        /// <param name="id">属性id。</param>
        /// <param name="subType">属性子类型。</param>
        /// <param name="value">属性值。</param>
        /// <param name="dispatchEvent">是否派发事件。</param>
        public void Modify(int id, NumericSubType subType, long value, bool dispatchEvent = true)
        {
            ModifyByKey(NumericHelper.EncodeNumericKey(id, subType), value, dispatchEvent);
        }

        /// <summary>
        /// 修改属性值。
        /// </summary>
        /// <param name="key">属性键值。</param>
        /// <param name="value">属性值。</param>
        /// <param name="dispatchEvent">是否派发事件。</param>
        public void ModifyByKey(int key, long value, bool dispatchEvent = true)
        {
            SetByKey(key, GetByKey(key) + value, dispatchEvent);
        }

        /// <summary>
        /// 订阅数值变更事件。
        /// </summary>
        /// <param name="id">属性id。</param>
        /// <param name="subType">属性子类型。</param>
        /// <param name="handler">数值变更事件。</param>
        /// <param name="notifyImmediately">是否立即通知。</param>
        public void Subscribe(int id, NumericSubType subType, SimpleEventHandler<long> handler, bool notifyImmediately = false)
        {
            SubscribeByKey(NumericHelper.EncodeNumericKey(id, subType), handler, notifyImmediately);
        }

        /// <summary>
        /// 订阅数值变更事件。
        /// </summary>
        /// <param name="key">属性键值。</param>
        /// <param name="handler">数值变更事件。</param>
        /// <param name="notifyImmediately">是否立即通知。</param>
        public void SubscribeByKey(int key, SimpleEventHandler<long> handler, bool notifyImmediately = false)
        {
            if (!m_NumericChangeEventDict.TryGetValue(key, out var numericChangeEvent))
            {
                numericChangeEvent = SimpleEvent<long>.Create();
                m_NumericChangeEventDict[key] = numericChangeEvent;
            }

            numericChangeEvent.Subscribe(handler);

            if (notifyImmediately)
            {
                handler(GetByKey(key));
            }
        }

        /// <summary>
        /// 取消订阅数值变更事件。
        /// </summary>
        /// <param name="id">属性id。</param>
        /// <param name="subType">属性子类型。</param>
        /// <param name="handler">数值变更事件。</param>
        public void Unsubscribe(int id, NumericSubType subType, SimpleEventHandler<long> handler)
        {
            UnsubscribeByKey(NumericHelper.EncodeNumericKey(id, subType), handler);
        }

        /// <summary>
        /// 取消订阅数值变更事件。
        /// </summary>
        /// <param name="key">属性键值。</param>
        /// <param name="handler">数值变更事件。</param>
        public void UnsubscribeByKey(int key, SimpleEventHandler<long> handler)
        {
            if (!m_NumericChangeEventDict.TryGetValue(key, out var numericChangeEvent))
            {
                return;
            }

            numericChangeEvent.Unsubscribe(handler);
        }

        /// <summary>
        /// 创建数值快照。
        /// </summary>
        /// <returns>数值快照。</returns>
        public INumeric TakeSnapshot()
        {
            return NumericSnapshot.Create(this);
        }

        /// <summary>
        /// 恢复数值快照。
        /// </summary>
        /// <param name="snapshot">数值快照。</param>
        public void RestoreSnapshot(INumeric snapshot)
        {
            m_NumericDict.Clear();
            m_NumericDict.AddRange(snapshot.NumericDict);
        }

        /// <summary>
        /// 派发数值变更事件。
        /// </summary>
        /// <param name="key">属性键值。</param>
        /// <param name="value">属性值。</param>
        private void DispatchNumericChangeEvent(int key, long value)
        {
            if (!m_NumericChangeEventDict.TryGetValue(key, out var numericChangeEvent))
            {
                return;
            }

            numericChangeEvent.Dispatch(value);
        }

        /// <summary>
        /// 克隆数值。
        /// </summary>
        public INumeric Clone()
        {
            return TakeSnapshot();
        }

        protected override void OnAwake()
        {
        }

        protected override void OnDispose()
        {
            foreach (var numericChangeEvent in m_NumericChangeEventDict.Values)
            {
                numericChangeEvent.Dispose();
            }

            m_NumericChangeEventDict.Clear();
            m_NumericDict.Clear();
        }

        /// <summary>
        /// 数值快照。
        /// </summary>
        private sealed class NumericSnapshot : INumeric, IReference
        {
            /// <summary>
            /// 数值字典。
            /// </summary>
            public IReadOnlyDictionary<int, long> NumericDict => m_NumericDict;

            /// <summary>
            /// 数值字典。
            /// </summary>
            private readonly Dictionary<int, long> m_NumericDict = new();

            /// <summary>
            /// 获取属性值。
            /// </summary>
            public long Get(int id, NumericSubType subType = NumericSubType.Final)
            {
                return GetByKey(NumericHelper.EncodeNumericKey(id, subType));
            }

            /// <summary>
            /// 获取属性值。
            /// </summary>
            /// <param name="key">属性键值。</param>
            /// <returns>属性值。</returns>
            public long GetByKey(int key)
            {
                return m_NumericDict.TryGetValue(key, out var value) ? value : 0;
            }

            public void Clear()
            {
                m_NumericDict.Clear();
            }

            public void Dispose()
            {
                ReferencePool.Release(this);
            }

            /// <summary>
            /// 创建数值快照。
            /// </summary>
            /// <param name="numericDict">数值字典。</param>
            /// <returns>数值快照。</returns>
            public static NumericSnapshot Create(NumericComponent numericComponent)
            {
                var snapshot = ReferencePool.Acquire<NumericSnapshot>();
                snapshot.m_NumericDict.AddRange(numericComponent.m_NumericDict);
                return snapshot;
            }

            /// <summary>
            /// 克隆数值快照。
            /// </summary>
            /// <returns>克隆后的数值快照。</returns>
            public INumeric Clone()
            {
                var snapshot = ReferencePool.Acquire<NumericSnapshot>();
                snapshot.m_NumericDict.AddRange(m_NumericDict);
                return snapshot;
            }
        }
    }
}
