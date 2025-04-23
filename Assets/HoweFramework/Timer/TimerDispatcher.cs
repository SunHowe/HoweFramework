using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 计时器调度器。
    /// </summary>
    internal sealed class TimerDispatcher : ITimerDispatcher, IReference
    {
        private readonly Dictionary<int, TimerInfo> m_TimerInfoDict = new Dictionary<int, TimerInfo>();
        private readonly List<TimerInfo> m_TimerInfoList = new List<TimerInfo>();
        private readonly Queue<TimerInfo> m_TimerInfoQueue = new Queue<TimerInfo>();

        /// <summary>
        /// 自增长的定时器id。
        /// </summary>
        private int m_IncrementTimerId;

        /// <summary>
        /// 模块总流逝时间。
        /// </summary>
        private float m_ElapsedTime;

        /// <summary>
        /// 已执行Update的次数。
        /// </summary>
        private int m_UpdateTimes;

        public int ReferenceId { get; set; }

        public void Clear()
        {
            m_TimerInfoDict.Clear();
            m_TimerInfoList.Clear();
            m_TimerInfoQueue.Clear();
            m_IncrementTimerId = 0;
            m_ElapsedTime = 0f;
            m_UpdateTimes = 0;
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public void Update(float elapseSeconds)
        {
            m_ElapsedTime += elapseSeconds;
            m_UpdateTimes += 1;

            // 将等待队列中的定时器添加到定时器列表中.
            while (m_TimerInfoQueue.Count > 0)
            {
                m_TimerInfoList.Add(m_TimerInfoQueue.Dequeue());
            }

            // 遍历定时器列表，更新定时器状态.
            for (var index = 0; index < m_TimerInfoList.Count; ++index)
            {
                var info = m_TimerInfoList[index];
                var willRemove = false;
                if (info.IsCancel)
                {
                    // 已经被取消的定时器. 从列表中移除.
                    willRemove = true;
                }
                else if (info.IsArrived(m_ElapsedTime, m_UpdateTimes))
                {
                    // 到达触发时间, 执行回调函数.
                    // 令次数+1
                    ++info.InvokeTimes;
                    try
                    {
                        info.TimerCallback();
                    }
                    catch (Exception e)
                    {
                        Log.Fatal($"Timer callback error: {e.Message}\n{e.StackTrace}");
                    }

                    if (info.TimerRepeatTimes <= 0 || info.InvokeTimes < info.TimerRepeatTimes)
                    {
                        // 未达到指定次数上限. 更新下次触发时间.
                        info.PreviousInvokeTime = m_ElapsedTime;
                        info.NextInvokeTime += info.TimerInterval;
                        info.NextInvokeUpdateTimes += info.TimerUpdateInterval;
                    }
                    else
                    {
                        // 达到指定次数上限, 标记为需要移除
                        willRemove = true;
                    }
                }

                if (!willRemove)
                {
                    continue;
                }

                // 从列表中移除
                ReferencePool.Release(info);

                // 将列表中最后一个定时器移到当前位置
                var lastIndex = m_TimerInfoList.Count - 1;
                if (index < lastIndex)
                {
                    m_TimerInfoList[index] = m_TimerInfoList[lastIndex];
                    m_TimerInfoList.RemoveAt(lastIndex);
                    index -= 1; // 需要减1防止这个定时器未执行到
                }
                else
                {
                    // 已经是最后一个定时器, 直接移除
                    m_TimerInfoList.RemoveAt(index);
                }
            }
        }

        public int AddFrameTimer(TimerCallback callback)
        {
            return AddFrameTimer(1, 0, callback, null);
        }

        public int AddFrameTimer(TimerCallback callback, object userData)
        {
            return AddFrameTimer(1, 0, callback, userData);
        }

        public int AddFrameTimer(int interval, TimerCallback callback)
        {
            return AddFrameTimer(interval, 0, callback, null);
        }

        public int AddFrameTimer(int interval, TimerCallback callback, object userData)
        {
            return AddFrameTimer(interval, 0, callback, userData);
        }

        public int AddFrameTimer(int interval, int repeatTimes, TimerCallback callback)
        {
            return AddFrameTimer(interval, repeatTimes, callback, null);
        }

        public int AddFrameTimer(int interval, int repeatTimes, TimerCallback callback, object userData)
        {
            if (m_IncrementTimerId >= int.MaxValue)
                m_IncrementTimerId = 0;

            var timerId = ++m_IncrementTimerId;
            var info = TimerInfo.CreateFrame(timerId, interval, repeatTimes, callback, userData);
            info.PreviousInvokeTime = m_ElapsedTime;
            info.NextInvokeUpdateTimes = m_UpdateTimes + interval;

            m_TimerInfoQueue.Enqueue(info);
            m_TimerInfoDict.Add(timerId, info);
            return timerId;
        }

        public int AddTimer(float interval, TimerCallback callback)
        {
            return AddTimer(interval, 0, callback, null);
        }

        public int AddTimer(float interval, TimerCallback callback, object userData)
        {
            return AddTimer(interval, 0, callback, userData);
        }

        public int AddTimer(float interval, int repeatTimes, TimerCallback callback)
        {
            return AddTimer(interval, repeatTimes, callback, null);
        }

        public int AddTimer(float interval, int repeatTimes, TimerCallback callback, object userData)
        {
            if (m_IncrementTimerId >= int.MaxValue)
                m_IncrementTimerId = 0;

            var timerId = ++m_IncrementTimerId;
            var info = TimerInfo.Create(timerId, interval, repeatTimes, callback, userData);
            info.PreviousInvokeTime = m_ElapsedTime;
            info.NextInvokeTime = m_ElapsedTime + interval;

            m_TimerInfoQueue.Enqueue(info);
            m_TimerInfoDict.Add(timerId, info);

            return timerId;
        }

        public void RemoveTimer(int timerId)
        {
            if (!m_TimerInfoDict.TryGetValue(timerId, out var info))
            {
                return;
            }

            info.IsCancel = true;
        }
    }
}
