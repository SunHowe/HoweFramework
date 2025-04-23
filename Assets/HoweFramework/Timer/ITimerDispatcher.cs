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
    public interface ITimerDispatcher : IDisposable
    {
        /// <summary>
        /// 添加无限重复的帧定时器，会在每帧回调一次。
        /// </summary>
        /// <param name="callback">定时器回调。</param>
        /// <returns>定时器id 用于手动停止定时器。</returns>
        int AddFrameTimer(TimerCallback callback);

        /// <summary>
        /// 添加无限重复的帧定时器，会在每间隔interval帧回调一次。
        /// </summary>
        /// <param name="interval">间隔帧数 最小值为1</param>
        /// <param name="callback">定时器回调。</param>
        /// <returns>定时器id 用于手动停止定时器。</returns>
        int AddFrameTimer(int interval, TimerCallback callback);

        /// <summary>
        /// 添加重复指定次数的帧定时器，会在每间隔interval帧回调一次。
        /// </summary>
        /// <param name="interval">间隔帧数 最小值为1</param>
        /// <param name="repeatTimes">重复次数。</param>
        /// <param name="callback">定时器回调。</param>
        /// <returns>定时器id 用于手动停止定时器。</returns>
        int AddFrameTimer(int interval, int repeatTimes, TimerCallback callback);
        
        /// <summary>
        /// 添加无限重复的定时器。
        /// </summary>
        /// <param name="interval">间隔时间。</param>
        /// <param name="callback">定时器回调。</param>
        /// <returns>定时器id 用于手动停止定时器。</returns>
        int AddTimer(float interval, TimerCallback callback);

        /// <summary>
        /// 添加重复指定次数的定时器。
        /// </summary>
        /// <param name="interval">间隔时间。</param>
        /// <param name="repeatTimes">重复次数。</param>
        /// <param name="callback">定时器回调。</param>
        /// <returns>定时器id 用于手动停止定时器。</returns>
        int AddTimer(float interval, int repeatTimes, TimerCallback callback);
        
        /// <summary>
        /// 通过定时器id移除定时器。
        /// </summary>
        /// <param name="timerId">定时器id。</param>
        void RemoveTimer(int timerId);

        /// <summary>
        /// 每帧更新。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        void Update(float elapseSeconds);
    }
}
