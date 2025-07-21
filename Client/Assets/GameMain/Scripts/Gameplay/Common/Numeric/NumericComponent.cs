using System.Collections.Generic;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 数值组件。
    /// </summary>
    [GameComponent(GameComponentType.Numeric)]
    public sealed class NumericComponent : GameComponentBase
    {
        /// <summary>
        /// 数值子类型位数。
        /// </summary>
        public const int NUMERIC_SUB_TYPE_BITS = 4;

        /// <summary>
        /// 数值子类型掩码。
        /// </summary>
        public const int NUMERIC_SUB_TYPE_MASK = (1 << NUMERIC_SUB_TYPE_BITS) - 1;

        /// <summary>
        /// 数值id掩码。
        /// </summary>
        public const int NUMERIC_ID_MASK = ~NUMERIC_SUB_TYPE_MASK;

        /// <summary>
        /// 数值字典。
        /// </summary>
        private readonly Dictionary<int, long> m_NumericDict = new();

        /// <summary>
        /// 数值变更事件字典。
        /// </summary>
        private readonly Dictionary<int, SimpleEvent<long>> m_NumericChangeEventDict = new();

        /// <summary>
        /// 获取最终值。
        /// </summary>
        public long this[int id] => GetFinal(id);

        /// <summary>
        /// 获取属性子类型。
        /// </summary>
        /// <param name="id">属性id。</param>
        /// <returns>属性子类型。</returns>
        public static NumericSubType GetSubType(int id)
        {
            return (NumericSubType)(id & NUMERIC_SUB_TYPE_MASK);
        }

        /// <summary>
        /// 获取属性id。
        /// </summary>
        /// <param name="id">属性id。</param>
        /// <param name="numericSubType">属性子类型。</param>
        /// <returns>属性id。</returns>
        public static int GetNumericId(int id, NumericSubType numericSubType)
        {
            return id & NUMERIC_ID_MASK | (int)numericSubType;
        }

        /// <summary>
        /// 获取最终值。
        /// </summary>
        public long GetFinal(int id)
        {
            return Get(GetNumericId(id, NumericSubType.Final));
        }

        /// <summary>
        /// 获取属性值。
        /// </summary>
        public long Get(int id)
        {
            return m_NumericDict.TryGetValue(id, out var value) ? value : 0;
        }
        
        /// <summary>
        /// 设置属性值。
        /// </summary>
        public void Set(int id, long value, bool dispatchEvent = true)
        {
            var subType = GetSubType(id);
            if (subType == NumericSubType.Final)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "不允许直接设置最终值。");
            }

            m_NumericDict[id] = value;

            var finalId = GetNumericId(id, NumericSubType.Final);
            var basicValue = Get(GetNumericId(id, NumericSubType.Basic));
            var basicPercent = Get(GetNumericId(id, NumericSubType.BasicPercent));
            var basicConstAdd = Get(GetNumericId(id, NumericSubType.BasicConstAdd));
            var finalPercent = Get(GetNumericId(id, NumericSubType.FinalPercent));
            var finalConstAdd = Get(GetNumericId(id, NumericSubType.FinalConstAdd));

            var finalValue = (basicValue * (100 + basicPercent) / 100 + basicConstAdd) * (100 + finalPercent) / 100 + finalConstAdd;
            m_NumericDict[finalId] = finalValue;

            if (!dispatchEvent)
            {
                return;
            }

            // 派发子属性变更事件。
            DispatchNumericChangeEvent(id, value);

            // 派发最终值变更事件。
            DispatchNumericChangeEvent(finalId, finalValue);
        }

        /// <summary>
        /// 修改属性值。
        /// </summary>
        /// <param name="id">属性id。</param>
        /// <param name="value">属性值。</param>
        /// <param name="dispatchEvent">是否派发事件。</param>
        public void Modify(int id, long value, bool dispatchEvent = true)
        {
            Set(id, Get(id) + value, dispatchEvent);
        }

        /// <summary>
        /// 订阅数值变更事件。
        /// </summary>
        /// <param name="id">属性id。</param>
        /// <param name="handler">数值变更事件。</param>
        /// <param name="notifyImmediately">是否立即通知。</param>
        public void Subscribe(int id, SimpleEventHandler<long> handler, bool notifyImmediately = false)
        {
            if (!m_NumericChangeEventDict.TryGetValue(id, out var numericChangeEvent))
            {
                numericChangeEvent = SimpleEvent<long>.Create();
                m_NumericChangeEventDict[id] = numericChangeEvent;
            }

            numericChangeEvent.Subscribe(handler);

            if (notifyImmediately)
            {
                handler(Get(id));
            }
        }

        /// <summary>
        /// 取消订阅数值变更事件。
        /// </summary>
        /// <param name="id">属性id。</param>
        /// <param name="handler">数值变更事件。</param>
        public void Unsubscribe(int id, SimpleEventHandler<long> handler)
        {
            if (!m_NumericChangeEventDict.TryGetValue(id, out var numericChangeEvent))
            {
                return;
            }

            numericChangeEvent.Unsubscribe(handler);
        }

        /// <summary>
        /// 派发数值变更事件。
        /// </summary>
        /// <param name="id">属性id。</param>
        /// <param name="value">属性值。</param>
        private void DispatchNumericChangeEvent(int id, long value)
        {
            if (!m_NumericChangeEventDict.TryGetValue(id, out var numericChangeEvent))
            {
                return;
            }

            numericChangeEvent.Dispatch(value);
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
    }
}
