using System;

namespace HoweFramework
{
    /// <summary>
    /// 事件模块。
    /// </summary>
    public sealed class EventModule : ModuleBase<EventModule>
    {
        /// <summary>
        /// 事件调度器。
        /// </summary>
        private IEventDispatcher m_EventDispatcher;

        /// <summary>
        /// 订阅事件。
        /// </summary>
        /// <param name="id">事件id。</param>
        /// <param name="handler">事件处理器。</param>
        public void Subscribe(int id, GameEventHandler handler)
        {
            m_EventDispatcher.Subscribe(id, handler);
        }

        /// <summary>
        /// 取消订阅事件。
        /// </summary>
        /// <param name="id">事件id。</param>
        /// <param name="handler">事件处理器。</param>
        public void Unsubscribe(int id, GameEventHandler handler)
        {
            m_EventDispatcher.Unsubscribe(id, handler);
        }

        /// <summary>   
        /// 派发事件(线程安全)。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="eventArgs">事件参数。</param>
        public void Dispatch(object sender, GameEventArgs eventArgs)
        {
            m_EventDispatcher.Dispatch(sender, eventArgs);
        }

        /// <summary>
        /// 立即派发事件(非线程安全)。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="eventArgs">事件参数。</param>
        public void DispatchNow(object sender, GameEventArgs eventArgs)
        {
            m_EventDispatcher.DispatchNow(sender, eventArgs);
        }

        /// <summary>
        /// 创建事件调度器。不需要使用时需要调用Dispose释放。
        /// </summary>
        /// <returns>事件调度器。</returns>
        public IEventDispatcher CreateEventDispatcher()
        {
            return new EventDispatcher();
        }

        protected override void OnInit()
        {
            m_EventDispatcher = CreateEventDispatcher();
            m_EventDispatcher.SetMode(EventDispatcherMode.AllowMultiHandler | EventDispatcherMode.AllowNoHandler);
        }

        protected override void OnDestroy()
        {
            m_EventDispatcher.Dispose();
            m_EventDispatcher = null;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            m_EventDispatcher.Update();
        }
    }
}

