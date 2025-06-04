using System;

namespace HoweFramework
{
    /// <summary>
    /// 可释放的事件订阅辅助工具。
    /// </summary>
    public sealed class DisposableEventSubscribe : IEventSubscribe, IDisposable
    {
        /// <summary>
        /// 绑定的事件调度器。
        /// </summary>
        private readonly IEventSubscribe m_EventSubscribe;

        /// <summary>
        /// 事件处理器字典。
        /// </summary>
        private readonly MultiDictionary<int, GameEventHandler> m_EventHandlerDict = new();

        public DisposableEventSubscribe(IEventSubscribe eventDispatcher = null)
        {
            m_EventSubscribe = eventDispatcher ?? EventModule.Instance.EventDispatcher;
        }

        /// <summary>
        /// 订阅事件。
        /// </summary>
        /// <param name="id">事件id。</param>
        /// <param name="handler">事件处理器。</param>
        public void Subscribe(int id, GameEventHandler handler)
        {
            m_EventSubscribe.Subscribe(id, handler);
            m_EventHandlerDict.Add(id, handler);
        }

        /// <summary>
        /// 取消订阅事件。
        /// </summary>
        /// <param name="id">事件id。</param>
        /// <param name="handler">事件处理器。</param>
        public void Unsubscribe(int id, GameEventHandler handler)
        {
            m_EventSubscribe.Unsubscribe(id, handler);
            m_EventHandlerDict.Remove(id, handler);
        }

        /// <summary>
        /// 释放。
        /// </summary>
        public void Dispose()
        {
            foreach (var (id, range) in m_EventHandlerDict)
            {
                foreach (var handler in range)
                {
                    m_EventSubscribe.Unsubscribe(id, handler);
                }
            }

            m_EventHandlerDict.Clear();
        }
    }
}
