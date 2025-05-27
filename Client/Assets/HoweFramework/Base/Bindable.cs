namespace HoweFramework
{
    /// <summary>
    /// 可绑定实例。
    /// </summary>
    public sealed class Bindable<T>
    {
        /// <summary>
        /// 值变化事件。
        /// </summary>
        public delegate void ValueChangedHandler(T value);

        public T Value
        {
            get => m_Value;
            set
            {
                m_Value = value;
                m_OnValueChanged?.Invoke(m_Value);
            }
        }

        private T m_Value;

        /// <summary>
        /// 值变化事件。
        /// </summary>
        private event ValueChangedHandler m_OnValueChanged;

        public Bindable(T value = default)
        {
            m_Value = value;
        }

        /// <summary>
        /// 订阅值变化事件。
        /// </summary>
        /// <param name="handler">值变化事件。</param>
        /// <param name="notifyImmediately">是否立即通知。</param>
        public void Subscribe(ValueChangedHandler handler, bool notifyImmediately = false)
        {
            m_OnValueChanged += handler;
            
            if (notifyImmediately)
            {
                handler?.Invoke(m_Value);
            }
        }

        /// <summary>
        /// 取消订阅值变化事件。
        /// </summary>
        /// <param name="handler">值变化事件。</param>
        public void Unsubscribe(ValueChangedHandler handler)
        {
            m_OnValueChanged -= handler;
        }
    }
}
