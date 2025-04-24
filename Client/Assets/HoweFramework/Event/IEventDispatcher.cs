using System;

namespace HoweFramework
{
    /// <summary>
    /// 事件调度器接口。
    /// </summary>
    public interface IEventDispatcher : IDisposable
    {
        /// <summary>
        /// 待处理的事件数量。
        /// </summary>
        int EventCount { get; }

        /// <summary>
        /// 设置事件调度器模式。
        /// </summary>
        /// <param name="mode">事件调度器模式。</param>
        void SetMode(EventDispatcherMode mode);

        /// <summary>
        /// 设置默认事件处理函数。
        /// </summary>
        /// <param name="handler">要设置的默认事件处理函数。</param>
        void SetDefaultHandler(GameEventHandler handler);

        /// <summary>
        /// 检查是否存在事件处理函数。
        /// </summary>
        /// <param name="id">事件类型编号。</param>
        /// <param name="handler">要检查的事件处理函数。</param>
        /// <returns>是否存在事件处理函数。</returns>
        bool Check(int id, GameEventHandler handler);

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
