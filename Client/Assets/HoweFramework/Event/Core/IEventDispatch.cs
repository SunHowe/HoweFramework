namespace HoweFramework
{
    /// <summary>
    /// 事件派发接口。
    /// </summary>
    public interface IEventDispatch
    {
        /// <summary>
        /// 派发事件。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="eventArgs">事件参数。</param>
        void Dispatch(object sender, GameEventArgs eventArgs);
    }
}