namespace HoweFramework
{
    /// <summary>
    /// 优先级事件订阅接口。
    /// </summary>
    public interface IPriorityEventSubscribe : IEventSubscribe
    {
        /// <summary>
        /// 按照指定优先级订阅事件。
        /// </summary>
        /// <param name="id">事件类型编号。</param>
        /// <param name="handler">事件处理函数。</param>
        /// <param name="priority">事件订阅优先级。在派发时，优先级高的先派发。</param>
        void Subscribe(int id, GameEventHandler handler, int priority);
    }
}