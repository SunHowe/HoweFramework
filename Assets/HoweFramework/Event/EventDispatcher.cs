using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 事件调度器。
    /// </summary>
    internal sealed class EventDispatcher : IEventDispatcher
    {
        /// <summary>
        /// 事件处理器字典。
        /// </summary>
        private readonly Dictionary<int, GameEventHandler> m_EventHandlerDict = new();

        /// <summary>
        /// 事件队列。
        /// </summary>
        private readonly ConcurrentQueue<EventItem> m_EventItemQueue = new();

        /// <summary>
        /// 订阅事件。
        /// </summary>
        /// <param name="id">事件id。</param>
        /// <param name="handler">事件处理器。</param>
        public void Subscribe(int id, GameEventHandler handler)
        {
            if (m_EventHandlerDict.TryGetValue(id, out var eventHandler))
            {
                eventHandler += handler;
                m_EventHandlerDict[id] = eventHandler;
            }
            else
            {
                m_EventHandlerDict[id] = handler;
            }
        }

        /// <summary>
        /// 取消订阅事件。
        /// </summary>
        /// <param name="id">事件id。</param>
        /// <param name="handler">事件处理器。</param>
        public void Unsubscribe(int id, GameEventHandler handler)
        {
            if (!m_EventHandlerDict.TryGetValue(id, out var eventHandler))
            {
                return;
            }

            eventHandler -= handler;
            m_EventHandlerDict[id] = eventHandler;
        }

        /// <summary>
        /// 派发事件(线程安全)。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="eventArgs">事件参数。</param>
        public void Dispatch(object sender, GameEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                throw new ArgumentNullException(nameof(eventArgs));
            }

            var eventItem = EventItem.Create(eventArgs.Id, eventArgs, sender);
            m_EventItemQueue.Enqueue(eventItem);
        }

        /// <summary>
        /// 立即派发事件(非线程安全)。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="eventArgs">事件参数。</param>
        public void DispatchNow(object sender, GameEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                throw new ArgumentNullException(nameof(eventArgs));
            }

            if (m_EventHandlerDict.TryGetValue(eventArgs.Id, out var eventHandler))
            {
                eventHandler.Invoke(sender, eventArgs);
            }

            if (eventArgs.IsReleaseAfterFire)
            {
                ReferencePool.Release(eventArgs);
            }
        }

        /// <summary>
        /// 每帧更新。
        /// </summary>
        public void Update()
        {
            while (m_EventItemQueue.TryDequeue(out var eventItem))
            {
                try
                {
                    m_EventHandlerDict[eventItem.Id]?.Invoke(eventItem.Sender, eventItem.EventArgs);
                }
                catch (Exception e)
                {
                    Log.Error($"Dispatch event {eventItem.Id} failed, {e.Message}");
                }
                finally
                {
                    if (eventItem.EventArgs.IsReleaseAfterFire)
                    {
                        ReferencePool.Release(eventItem.EventArgs);
                    }

                    ReferencePool.Release(eventItem);
                }
            }
        }

        public void Dispose()
        {
            m_EventHandlerDict.Clear();
            m_EventItemQueue.Clear();
        }
    }
}

