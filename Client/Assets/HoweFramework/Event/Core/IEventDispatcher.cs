using System;

namespace HoweFramework
{
    /// <summary>
    /// 事件调度器接口。
    /// </summary>
    public interface IEventDispatcher : IEventDispatch, IEventSubscribe, IDisposable
    {
        /// <summary>
        /// 设置事件调度器模式。
        /// </summary>
        /// <param name="mode">事件调度器模式。</param>
        void SetMode(EventDispatcherMode mode);

        /// <summary>
        /// 设置默认事件处理函数。
        /// </summary>
        /// <param name="handler">要设置的默认事件处理函数。</param>
        void SetDefaultHandler(GameEventHandlerFunc handler);

        /// <summary>
        /// 检查是否存在事件处理函数。
        /// </summary>
        /// <param name="id">事件类型编号。</param>
        /// <param name="handler">要检查的事件处理函数。</param>
        /// <returns>是否存在事件处理函数。</returns>
        bool Check(int id, GameEventHandler handler);
    }
}
