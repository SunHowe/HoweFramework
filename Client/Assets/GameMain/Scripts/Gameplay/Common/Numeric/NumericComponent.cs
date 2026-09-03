using System.Collections.Generic;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 数值组件。
    /// </summary>
    public sealed class NumericComponent : GameComponentBase, INumeric
    {
        /// <summary>
        /// 数值字典（各子项求和结果与 Final）。
        /// </summary>
        private readonly Dictionary<int, long> m_NumericDict = new();

        /// <summary>
        /// 分来源贡献。key 为 EncodeNumericKey，内层 key 为来源。
        /// </summary>
        private readonly Dictionary<int, ReusableDictionary<object, long>> m_SourceDict = new();

        /// <summary>
        /// 数值变更事件字典。
        /// </summary>
        private readonly Dictionary<int, SimpleEvent<long>> m_NumericChangeEventDict = new();

        /// <summary>
        /// 数值字典。
        /// </summary>
        public IReadOnlyDictionary<int, long> NumericDict => m_NumericDict;

        /// <summary>
        /// 获取最终值。写入必须走带 source 的 <see cref="Set(int, NumericSubType, long, object, bool)"/>。
        /// </summary>
        public long this[int id] => Get(id, NumericSubType.Final);

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
        /// 设置指定来源的属性贡献。同一子项下来源求和后再重算 Final。
        /// </summary>
        /// <param name="id">属性id。</param>
        /// <param name="subType">属性子类型。</param>
        /// <param name="value">该来源的贡献值。</param>
        /// <param name="source">来源。不能为空。</param>
        /// <param name="dispatchEvent">是否派发事件。</param>
        public void Set(int id, NumericSubType subType, long value, object source, bool dispatchEvent = true)
        {
            if (source == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "数值来源不能为空。");
            }

            SetByKeyInternal(NumericHelper.EncodeNumericKey(id, subType), value, source, dispatchEvent);
        }

        /// <summary>
        /// 设置指定来源的属性贡献。
        /// </summary>
        /// <param name="key">属性键值。</param>
        /// <param name="value">该来源的贡献值。</param>
        /// <param name="source">来源。不能为空。</param>
        /// <param name="dispatchEvent">是否派发事件。</param>
        public void SetByKey(int key, long value, object source, bool dispatchEvent = true)
        {
            if (source == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "数值来源不能为空。");
            }

            SetByKeyInternal(key, value, source, dispatchEvent);
        }

        /// <summary>
        /// 移除某个来源在所有子项上的贡献，并重算受影响属性的 Final。
        /// </summary>
        /// <param name="source">来源。</param>
        /// <param name="dispatchEvent">是否派发事件。</param>
        public void RemoveFromSource(object source, bool dispatchEvent = true)
        {
            if (source == null)
            {
                return;
            }

            using var affectedIds = ReusableHashSet<int>.Create();
            using var emptyKeys = ReusableList<int>.Create();

            foreach (var pair in m_SourceDict)
            {
                if (!pair.Value.Remove(source))
                {
                    continue;
                }

                var (id, _) = NumericHelper.DecodeNumericKey(pair.Key);
                affectedIds.Add(id);

                if (pair.Value.Count == 0)
                {
                    emptyKeys.Add(pair.Key);
                }
                else
                {
                    var sum = SumSources(pair.Value);
                    m_NumericDict[pair.Key] = sum;
                    if (dispatchEvent)
                    {
                        DispatchNumericChangeEvent(pair.Key, sum);
                    }
                }
            }

            for (var i = 0; i < emptyKeys.Count; i++)
            {
                var key = emptyKeys[i];
                m_SourceDict[key].Dispose();
                m_SourceDict.Remove(key);
                m_NumericDict.Remove(key);
                if (dispatchEvent)
                {
                    DispatchNumericChangeEvent(key, 0);
                }
            }

            foreach (var id in affectedIds)
            {
                RecalculateFinal(id, dispatchEvent);
            }
        }

        /// <summary>
        /// 修改指定来源的贡献。
        /// </summary>
        public void Modify(int id, NumericSubType subType, long value, object source, bool dispatchEvent = true)
        {
            ModifyByKey(NumericHelper.EncodeNumericKey(id, subType), value, source, dispatchEvent);
        }

        /// <summary>
        /// 修改指定来源的贡献。
        /// </summary>
        /// <param name="key">属性键值。</param>
        /// <param name="value">增量。</param>
        /// <param name="source">来源。不能为空。</param>
        /// <param name="dispatchEvent">是否派发事件。</param>
        public void ModifyByKey(int key, long value, object source, bool dispatchEvent = true)
        {
            if (source == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "数值来源不能为空。");
            }

            SetByKeyInternal(key, GetFromSource(key, source) + value, source, dispatchEvent);
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
        /// 恢复数值快照。来源分解不保留，全部写入 <paramref name="source"/>。
        /// </summary>
        /// <param name="snapshot">数值快照。</param>
        /// <param name="source">恢复后的来源。不能为空。</param>
        public void RestoreSnapshot(INumeric snapshot, object source)
        {
            if (source == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "数值来源不能为空。");
            }

            ClearSources();
            m_NumericDict.Clear();
            m_NumericDict.AddRange(snapshot.NumericDict);

            foreach (var pair in m_NumericDict)
            {
                var (_, subType) = NumericHelper.DecodeNumericKey(pair.Key);
                if (subType == NumericSubType.Final)
                {
                    continue;
                }

                var sources = ReusableDictionary<object, long>.Create();
                sources[source] = pair.Value;
                m_SourceDict[pair.Key] = sources;
            }
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
            ClearSources();
            m_NumericDict.Clear();
        }

        private void SetByKeyInternal(int key, long value, object source, bool dispatchEvent)
        {
            var (id, subType) = NumericHelper.DecodeNumericKey(key);
            if (subType == NumericSubType.Final)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "不允许直接设置最终值。");
            }

            if (!m_SourceDict.TryGetValue(key, out var sources))
            {
                sources = ReusableDictionary<object, long>.Create();
                m_SourceDict[key] = sources;
            }

            sources[source] = value;
            var sum = SumSources(sources);
            m_NumericDict[key] = sum;

            if (dispatchEvent)
            {
                DispatchNumericChangeEvent(key, sum);
            }

            RecalculateFinal(id, dispatchEvent);
        }

        private void RecalculateFinal(int id, bool dispatchEvent)
        {
            var finalKey = NumericHelper.EncodeNumericKey(id, NumericSubType.Final);
            var basicValue = Get(id, NumericSubType.Basic);
            var basicPercent = Get(id, NumericSubType.BasicPercent);
            var basicConstAdd = Get(id, NumericSubType.BasicConstAdd);
            var finalPercent = Get(id, NumericSubType.FinalPercent);
            var finalConstAdd = Get(id, NumericSubType.FinalConstAdd);
            var overrideKey = NumericHelper.EncodeNumericKey(id, NumericSubType.Override);

            var finalValue = m_NumericDict.ContainsKey(overrideKey)
                ? GetByKey(overrideKey)
                : (basicValue * (100 + basicPercent) / 100 + basicConstAdd) * (100 + finalPercent) / 100 + finalConstAdd;

            m_NumericDict[finalKey] = finalValue;

            if (!dispatchEvent)
            {
                return;
            }

            DispatchNumericChangeEvent(finalKey, finalValue);
        }

        private long GetFromSource(int key, object source)
        {
            if (m_SourceDict.TryGetValue(key, out var sources) && sources.TryGetValue(source, out var value))
            {
                return value;
            }

            return 0;
        }

        private static long SumSources(Dictionary<object, long> sources)
        {
            long sum = 0;
            foreach (var pair in sources)
            {
                sum += pair.Value;
            }

            return sum;
        }

        private void ClearSources()
        {
            foreach (var sources in m_SourceDict.Values)
            {
                sources.Dispose();
            }

            m_SourceDict.Clear();
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
            /// <param name="numericComponent">数值组件。</param>
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
