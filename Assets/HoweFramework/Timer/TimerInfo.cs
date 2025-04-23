using System;
using System.Runtime.CompilerServices;

namespace HoweFramework
{
    /// <summary>
    /// 计时器信息。
    /// </summary>
    internal sealed class TimerInfo : IReference
    {
        /// <summary>
        /// 定时器的唯一id。
        /// </summary>
        public int TimerId { get; private set; }

        /// <summary>
        /// 定时器回调周期。单位：秒。时间定时器使用。
        /// </summary>
        public float TimerInterval { get; private set; }

        /// <summary>
        /// 定时器回调周期。单位: 帧。帧定时器使用。
        /// </summary>
        public int TimerUpdateInterval { get; private set; }

        /// <summary>
        /// 定时器回调重复次数, 0代表不限制次数。
        /// </summary>
        public int TimerRepeatTimes { get; private set; }

        /// <summary>
        /// 定时器回调函数。
        /// </summary>
        public TimerCallback TimerCallback { get; private set; }

        /// <summary>
        /// 定时器用户数据。
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 已触发回调的次数。
        /// </summary>
        public int InvokeTimes { get; set; }

        /// <summary>
        /// 前一次触发回调的时间。
        /// </summary>
        public float PreviousInvokeTime { get; set; }

        /// <summary>
        /// 下一次触发回调的时间。时间定时器使用。
        /// </summary>
        public float NextInvokeTime { get; set; }

        /// <summary>
        /// 下一次触发回调的帧更新次数。帧定时器使用。
        /// </summary>
        public int NextInvokeUpdateTimes { get; set; }

        /// <summary>
        /// 是否已经被取消。
        /// </summary>
        public bool IsCancel { get; set; }

        /// <summary>
        /// 是否是帧定时器。
        /// </summary>
        public bool IsFrameTimer { get; set; }

        /// <summary>
        /// 用于判断是否已经达到目标时间的函数。
        /// </summary>
        private Func<float, int, bool> m_ArriveFunc;

        public void Clear()
        {
            TimerId = 0;
            TimerInterval = 0f;
            TimerRepeatTimes = 0;
            TimerCallback = null;
            UserData = null;
            InvokeTimes = 0;
            PreviousInvokeTime = 0f;
            NextInvokeTime = 0f;
            NextInvokeUpdateTimes = 0;
            IsCancel = false;
            IsFrameTimer = false;
            m_ArriveFunc = null;
        }

        /// <summary>
        /// 创建时间定时器。
        /// </summary>
        public static TimerInfo Create(int timerId, float timerInterval, int timerRepeatTimes, TimerCallback timerCallback, object userData)
        {
            var timerInfo = ReferencePool.Acquire<TimerInfo>();
            timerInfo.TimerId = timerId;
            timerInfo.TimerInterval = timerInterval;
            timerInfo.TimerRepeatTimes = timerRepeatTimes;
            timerInfo.TimerCallback = timerCallback;
            timerInfo.UserData = userData;
            timerInfo.m_ArriveFunc = timerInfo.IsArrivedByTime;
            return timerInfo;
        }

        /// <summary>
        /// 创建帧定时器。
        /// </summary>
        public static TimerInfo CreateFrame(int timerId, int timerUpdateInterval, int timerRepeatTimes, TimerCallback timerCallback, object userData)
        {
            var timerInfo = ReferencePool.Acquire<TimerInfo>();
            timerInfo.TimerId = timerId;
            timerInfo.TimerUpdateInterval = timerUpdateInterval;
            timerInfo.TimerRepeatTimes = timerRepeatTimes;
            timerInfo.TimerCallback = timerCallback;
            timerInfo.UserData = userData;
            timerInfo.IsFrameTimer = true;
            timerInfo.m_ArriveFunc = timerInfo.IsArrivedByFrame;
            return timerInfo;
        }

        /// <summary>
        /// 是否已经到定时器触发时机。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsArrived(float elapsedTime, int updateTimes)
        {
            return m_ArriveFunc(elapsedTime, updateTimes);
        }

        private bool IsArrivedByTime(float elapsedTime, int _)
        {
            return NextInvokeTime <= elapsedTime;
        }

        private bool IsArrivedByFrame(float _, int updateTimes)
        {
            return NextInvokeUpdateTimes <= updateTimes;
        }
    }
}
