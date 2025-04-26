using System;

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
        public IEventDispatcher EventDispatcher { get; private set; }

        /// <summary>
        /// 订阅事件。
        /// </summary>
        /// <param name="id">事件id。</param>
        /// <param name="handler">事件处理器。</param>
        public void Subscribe(int id, GameEventHandler handler)
        {
            EventDispatcher.Subscribe(id, handler);
        }

        /// <summary>
        /// 取消订阅事件。
        /// </summary>
        /// <param name="id">事件id。</param>
        /// <param name="handler">事件处理器。</param>
        public void Unsubscribe(int id, GameEventHandler handler)
        {
            EventDispatcher.Unsubscribe(id, handler);
        }

        /// <summary>   
        /// 派发事件(线程安全)。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="eventArgs">事件参数。</param>
        public void Dispatch(object sender, GameEventArgs eventArgs)
        {
            EventDispatcher.Dispatch(sender, eventArgs);
        }

        /// <summary>
        /// 立即派发事件(非线程安全)。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="eventArgs">事件参数。</param>
        public void DispatchNow(object sender, GameEventArgs eventArgs)
        {
            EventDispatcher.DispatchNow(sender, eventArgs);
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
            EventDispatcher = CreateEventDispatcher();
            EventDispatcher.SetMode(EventDispatcherMode.AllowMultiHandler | EventDispatcherMode.AllowNoHandler);
        }

        protected override void OnDestroy()
        {
            EventDispatcher.Dispose();
            EventDispatcher = null;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            EventDispatcher.Update();
        }
    }
}

