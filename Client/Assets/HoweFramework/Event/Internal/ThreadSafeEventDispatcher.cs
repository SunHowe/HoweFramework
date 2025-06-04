using System.Collections.Concurrent;

namespace HoweFramework
{
    /// <summary>
    /// 线程安全事件派发器。
    /// </summary>
    internal sealed class ThreadSafeEventDispatcher : EventDispatcherBase, IThreadSafeEventDispatcher
    {
        /// <summary>
        /// 待处理的事件数量。
        /// </summary>
        public int EventCount => m_EventItemQueue.Count;

        /// <summary>
        /// 事件队列。
        /// </summary>
        private readonly ConcurrentQueue<EventItem> m_EventItemQueue = new();

        /// <summary>
        /// 线程安全派发事件。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="eventArgs">事件参数。</param>
        public void ThreadSafeDispatch(object sender, GameEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "Event args is invalid.");
            }

            var eventItem = EventItem.Create(eventArgs.Id, eventArgs, sender);
            m_EventItemQueue.Enqueue(eventItem);
        }

        /// <summary>
        /// 更新事件。
        /// </summary>
        public void UpdateEvents()
        {
            while (m_EventItemQueue.TryDequeue(out var eventItem))
            {
                HandleEvent(eventItem.Sender, eventItem.EventArgs);
                ReferencePool.Release(eventItem);
            }
        }

        /// <summary>
        /// 清空事件。
        /// </summary>
        public void ClearEvents()
        {
            m_EventItemQueue.Clear();
        }

        public override void Dispose()
        {
            m_EventItemQueue.Clear();
            base.Dispose();
        }
    }
}
