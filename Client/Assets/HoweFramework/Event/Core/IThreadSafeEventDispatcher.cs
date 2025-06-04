namespace HoweFramework
{
    /// <summary>
    /// 线程安全事件调度器。
    /// </summary>
    public interface IThreadSafeEventDispatcher : IEventDispatcher, IThreadSafeEventDispatch
    {
        /// <summary>
        /// 待处理的事件数量。
        /// </summary>
        int EventCount { get; }

        /// <summary>
        /// 更新缓存的事件。
        /// </summary>
        void UpdateEvents();

        /// <summary>
        /// 清空缓存的事件。
        /// </summary>
        void ClearEvents();
    }
}
