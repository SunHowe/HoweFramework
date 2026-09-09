using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 行为树根节点。
    /// </summary>
    public sealed class BehaviorRoot : BehaviorDecorNodeBase, IBehaviorContext
    {
        /// <summary>
        /// 值字典。
        /// </summary>
        private readonly Dictionary<string, object> m_Values = new();

        private int m_LastResult = FrameworkErrorCode.Success;

        /// <summary>
        /// 执行。
        /// </summary>
        /// <returns>返回执行结果。</returns>
        public override int Execute()
        {
            if (m_LastResult != FrameworkErrorCode.BehaviorRunningState)
            {
                ResetState();
            }

            m_LastResult = ExecuteChild();
            return m_LastResult;
        }

        /// <summary>
        /// 清理。
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            m_Values.Clear();
            m_LastResult = FrameworkErrorCode.Success;
        }

        public override void ResetState()
        {
            m_LastResult = FrameworkErrorCode.Success;
            base.ResetState();
        }

        /// <summary>
        /// 设置值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        public void SetValue<T>(string key, T value)
        {
            m_Values[key] = value;
        }

        /// <summary>
        /// 获取值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>返回值。</returns>
        public T GetValue<T>(string key, T defaultValue = default)
        {
            if (m_Values.TryGetValue(key, out var value) && value is T distValue)
            {
                return distValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// 从行为树上下文获取值的字符串。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>返回值。</returns>
        public string GetValueString(string key)
        {
            if (m_Values.TryGetValue(key, out var value) && value != null)
            {
                return value.ToString();
            }

            return null;
        }

        /// <summary>
        /// 移除值。
        /// </summary>
        /// <param name="key">键。</param>
        public void RemoveValue(string key)
        {
            m_Values.Remove(key);
        }

        /// <summary>
        /// 创建行为树根节点。
        /// </summary>
        /// <returns>返回行为树根节点。</returns>
        public static BehaviorRoot Create()
        {
            var root = ReferencePool.Acquire<BehaviorRoot>();
            root.SetContext(root);
            return root;
        }
    }
}