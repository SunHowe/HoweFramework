namespace HoweFramework
{
    /// <summary>
    /// 优先级事件调度器接口。
    /// </summary>
    public interface IPriorityEventDispatcher : IEventDispatcher, IPriorityEventSubscribe
    {
    }
}