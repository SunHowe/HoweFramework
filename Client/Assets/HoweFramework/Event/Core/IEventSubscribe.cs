namespace HoweFramework
{
    /// <summary>
    /// 事件订阅接口。
    /// </summary>
    public interface IEventSubscribe
    {
        /// <summary>
        /// 订阅事件。
        /// </summary>
        void Subscribe(int id, GameEventHandler handler);

        /// <summary>
        /// 取消订阅事件。
        /// </summary>
        void Unsubscribe(int id, GameEventHandler handler);
    }
}
