using System;

namespace HoweFramework
{
    /// <summary>
    /// 定时器回调。
    /// </summary>
    public delegate void TimerCallback();

    /// <summary>
    /// 计时器调度器接口。
    /// </summary>
    public interface ITimerDispatcher : ITimerCore, IDisposable
    {
        /// <summary>
        /// 每帧更新。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        void Update(float elapseSeconds);
    }
}
