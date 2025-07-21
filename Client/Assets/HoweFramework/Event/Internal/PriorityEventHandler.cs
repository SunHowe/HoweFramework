using System;

namespace HoweFramework
{
    /// <summary>
    /// 优先级事件处理器。
    /// </summary>
    internal sealed class PriorityEventHandler : IDisposable, IReferenceWithId, IComparable<PriorityEventHandler>, IEquatable<PriorityEventHandler>
    {
        /// <summary>
        /// 事件处理函数。
        /// </summary>
        public GameEventHandler Handler { get; private set; }

        /// <summary>
        /// 事件订阅优先级。按照优先级从高到低排序，在派发时，优先级高的先派发。
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// 实例编号。优先级相同的事件，按照实例编号排序。
        /// </summary>
        public int InstanceId { get; set; }

        public void Clear()
        {
            Handler = null;
            Priority = 0;
            InstanceId = 0;
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public int CompareTo(PriorityEventHandler other)
        {
            var compare = other.Priority.CompareTo(Priority);
            if (compare != 0)
            {
                return compare;
            }

            return InstanceId.CompareTo(other.InstanceId);
        }

        public bool Equals(PriorityEventHandler other)
        {
            // 只比较事件处理函数，不比较实例编号和优先级。
            return Handler == other.Handler;
        }

        /// <summary>
        /// 创建优先级事件处理器。
        /// </summary>
        /// <param name="handler">事件处理函数。</param>
        /// <param name="priority">事件订阅优先级。按照优先级从高到低排序，在派发时，优先级高的先派发。</param>
        /// <returns>创建的优先级事件处理器。</returns>
        public static PriorityEventHandler Create(GameEventHandler handler, int priority)
        {
            PriorityEventHandler priorityEventHandler = ReferencePool.Acquire<PriorityEventHandler>();
            priorityEventHandler.Handler = handler;
            priorityEventHandler.Priority = priority;
            return priorityEventHandler;
        }
    }
}