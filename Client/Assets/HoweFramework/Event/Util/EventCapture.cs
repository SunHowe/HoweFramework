using System;

namespace HoweFramework
{
    /// <summary>
    /// 事件捕获器。用于临时的事件捕获，不需要使用时需要调用 Dispose 释放。
    /// </summary>
    public sealed class EventCapture : IDisposable, IReference
    {
        /// <summary>
        /// 事件Id。
        /// </summary>
        public int EventId { get; private set; }

        /// <summary>
        /// 事件处理函数。
        /// </summary>
        public GameEventHandler Handler { get; private set; }

        private bool m_Referenced = false;
        private IEventSubscribe m_EventSubscribe = null;

        public void Clear()
        {
            EventId = 0;
            Handler = null;
            m_Referenced = false;
            m_EventSubscribe = null;
        }

        public void Dispose()
        {
            if (!m_Referenced)
            {
                return;
            }

            if (m_EventSubscribe != null)
            {
                m_EventSubscribe.Unsubscribe(EventId, Handler);
            }

            ReferencePool.Release(this);
        }

        /// <summary>
        /// 创建事件捕获器。
        /// </summary>
        /// <param name="eventId">事件Id。</param>
        /// <param name="handler">事件处理函数。</param>
        /// <param name="eventSubscribe">事件订阅器。若未指定则使用默认事件订阅器。</param>
        /// <returns>事件捕获器。</returns>
        public static EventCapture Create(int eventId, GameEventHandler handler, IEventSubscribe eventSubscribe = null)
        {
            eventSubscribe ??= EventModule.Instance.EventDispatcher;
            
            EventCapture eventCapture = ReferencePool.Acquire<EventCapture>();
            eventCapture.EventId = eventId;
            eventCapture.Handler = handler;
            eventCapture.m_Referenced = true;
            eventCapture.m_EventSubscribe = eventSubscribe;

            eventSubscribe.Subscribe(eventId, handler);

            return eventCapture;
        }
    }
}
