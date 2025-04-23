using System;

namespace HoweFramework
{
    /// <summary>
    /// 事件调度器接口。
    /// </summary>
    public interface IEventDispatcher : IDisposable
    {
        /// <summary>
        /// 订阅事件。
        /// </summary>
        /// <param name="id">事件id。</param>
        /// <param name="handler">事件处理器。</param>
        void Subscribe(int id, GameEventHandler handler);

        /// <summary>
        /// 取消订阅事件。
        /// </summary>
        /// <param name="id">事件id。</param>
        /// <param name="handler">事件处理器。</param>
        void Unsubscribe(int id, GameEventHandler handler);

        /// <summary>
        /// 派发事件(线程安全)。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="eventArgs">事件参数。</param>
        void Dispatch(object sender, GameEventArgs eventArgs);

        /// <summary>
        /// 立即派发事件(非线程安全)。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="eventArgs">事件参数。</param>
        void DispatchNow(object sender, GameEventArgs eventArgs);

        /// <summary>
        /// 每帧更新。用于驱动事件调度。
        /// </summary>
        void Update();
    }
}
