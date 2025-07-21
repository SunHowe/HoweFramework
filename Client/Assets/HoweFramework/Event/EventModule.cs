namespace HoweFramework
{
    /// <summary>
    /// 事件模块。
    /// </summary>
    public sealed class EventModule : ModuleBase<EventModule>
    {
        /// <summary>
        /// 全局事件调度器。
        /// </summary>
        public IEventDispatcher EventDispatcher => m_ThreadSafeEventDispatcher;

        /// <summary>
        /// 线程安全事件调度器。
        /// </summary>
        public IThreadSafeEventDispatcher ThreadSafeEventDispatcher => m_ThreadSafeEventDispatcher;

        /// <summary>
        /// 事件调度器实例。
        /// </summary>
        private IThreadSafeEventDispatcher m_ThreadSafeEventDispatcher;

        /// <summary>
        /// 订阅事件。
        /// </summary>
        /// <param name="id">事件id。</param>
        /// <param name="handler">事件处理器。</param>
        public void Subscribe(int id, GameEventHandler handler)
        {
            m_ThreadSafeEventDispatcher.Subscribe(id, handler);
        }

        /// <summary>
        /// 取消订阅事件。
        /// </summary>
        /// <param name="id">事件id。</param>
        /// <param name="handler">事件处理器。</param>
        public void Unsubscribe(int id, GameEventHandler handler)
        {
            m_ThreadSafeEventDispatcher.Unsubscribe(id, handler);
        }

        /// <summary>   
        /// 派发事件。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="eventArgs">事件参数。</param>
        public void Dispatch(object sender, GameEventArgs eventArgs)
        {
            m_ThreadSafeEventDispatcher.Dispatch(sender, eventArgs);
        }

        /// <summary>
        /// 立即派发事件(线程安全)。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="eventArgs">事件参数。</param>
        public void ThreadSafeDispatch(object sender, GameEventArgs eventArgs)
        {
            m_ThreadSafeEventDispatcher.ThreadSafeDispatch(sender, eventArgs);
        }

        /// <summary>
        /// 创建事件调度器。不需要使用时需要调用Dispose释放。
        /// </summary>
        /// <returns>事件调度器。</returns>
        public IEventDispatcher CreateEventDispatcher()
        {
            return new EventDispatcher();
        }

        /// <summary>
        /// 创建线程安全事件调度器。不需要使用时需要调用Dispose释放。
        /// </summary>
        /// <returns>线程安全事件调度器。</returns>
        public IThreadSafeEventDispatcher CreateThreadSafeEventDispatcher()
        {
            return new ThreadSafeEventDispatcher();
        }

        /// <summary>
        /// 创建优先级事件调度器。不需要使用时需要调用Dispose释放。
        /// </summary>
        /// <returns>优先级事件调度器。</returns>
        public IPriorityEventDispatcher CreatePriorityEventDispatcher()
        {
            return new PriorityEventDispatcher();
        }

        protected override void OnInit()
        {
            m_ThreadSafeEventDispatcher = CreateThreadSafeEventDispatcher();
            m_ThreadSafeEventDispatcher.SetMode(EventDispatcherMode.AllowMultiHandler | EventDispatcherMode.AllowNoHandler);
        }

        protected override void OnDestroy()
        {
            m_ThreadSafeEventDispatcher.Dispose();
            m_ThreadSafeEventDispatcher = null;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            m_ThreadSafeEventDispatcher.UpdateEvents();
        }
    }
}

