namespace HoweFramework
{
    /// <summary>
    /// 事件派发接口(线程安全)。
    /// </summary>
    public interface IThreadSafeEventDispatch
    {
        /// <summary>
        /// 派发事件(线程安全)。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="eventArgs">事件参数。</param>
        void ThreadSafeDispatch(object sender, GameEventArgs eventArgs);
    }
}
