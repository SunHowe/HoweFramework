using System;

namespace HoweFramework
{
    /// <summary>
    /// 事件item。
    /// </summary>
    internal sealed class EventItem : IReference
    {
        /// <summary>
        /// 事件id。
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 事件参数。
        /// </summary>
        public GameEventArgs EventArgs { get; set; }

        /// <summary>
        /// 事件发送者。
        /// </summary>
        public object Sender { get; set; }

        public int ReferenceId { get; set; }

        public void Clear()
        {
            Id = 0;
            EventArgs = null;
            Sender = null;
            ReferenceId = 0;
        }

        /// <summary>
        /// 创建事件item。
        /// </summary>
        /// <param name="id">事件id。</param>
        /// <param name="eventArgs">事件参数。</param>
        /// <param name="sender">事件发送者。</param>
        /// <returns>事件item。</returns>
        public static EventItem Create(int id, GameEventArgs eventArgs, object sender)
        {
            EventItem eventItem = ReferencePool.Acquire<EventItem>();
            eventItem.Id = id;
            eventItem.EventArgs = eventArgs;
            eventItem.Sender = sender;
            return eventItem;
        }
    }
}
