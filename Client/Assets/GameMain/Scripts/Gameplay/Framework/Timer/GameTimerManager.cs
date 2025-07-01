using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 游戏定时器管理器。
    /// </summary>
    public sealed class GameTimerManager : GameManagerBase, IGameTimerManager
    {
        /// <summary>
        /// 定时器调度器。
        /// </summary>
        private ITimerDispatcher m_TimerDispatcher;

        /// <summary>
        /// 游戏更新管理器。
        /// </summary>
        private IGameUpdateManager m_GameUpdateManager;

        /// <summary>
        /// 添加无限重复的帧定时器，会在每帧回调一次。
        /// </summary>
        /// <param name="callback">定时器回调。</param>
        /// <returns>定时器id 用于手动停止定时器。</returns>
        public int AddFrameTimer(TimerCallback callback)
        {
            return m_TimerDispatcher.AddFrameTimer(callback);
        }

        /// <summary>
        /// 添加无限重复的帧定时器，会在每间隔interval帧回调一次。
        /// </summary>
        /// <param name="interval">间隔帧数 最小值为1</param>
        /// <param name="callback">定时器回调。</param>
        /// <returns>定时器id 用于手动停止定时器。</returns>
        public int AddFrameTimer(int interval, TimerCallback callback)
        {
            return m_TimerDispatcher.AddFrameTimer(interval, callback);
        }

        /// <summary>
        /// 添加重复指定次数的帧定时器，会在每间隔interval帧回调一次。
        /// </summary>
        /// <param name="interval">间隔帧数 最小值为1</param>
        /// <param name="repeatTimes">重复次数。</param>
        /// <param name="callback">定时器回调。</param>
        /// <returns>定时器id 用于手动停止定时器。</returns>
        public int AddFrameTimer(int interval, int repeatTimes, TimerCallback callback)
        {
            return m_TimerDispatcher.AddFrameTimer(interval, repeatTimes, callback);
        }

        /// <summary>
        /// 添加无限重复的定时器。
        /// </summary>
        /// <param name="interval">间隔时间。</param>
        /// <param name="callback">定时器回调。</param>
        /// <returns>定时器id 用于手动停止定时器。</returns>
        public int AddTimer(float interval, TimerCallback callback)
        {
            return m_TimerDispatcher.AddTimer(interval, callback);
        }

        /// <summary>
        /// 添加重复指定次数的定时器。
        /// </summary>
        /// <param name="interval">间隔时间。</param>
        /// <param name="repeatTimes">重复次数。</param>
        /// <param name="callback">定时器回调。</param>
        /// <returns>定时器id 用于手动停止定时器。</returns>
        public int AddTimer(float interval, int repeatTimes, TimerCallback callback)
        {
            return m_TimerDispatcher.AddTimer(interval, repeatTimes, callback);
        }

        /// <summary>
        /// 通过定时器id移除定时器。
        /// </summary>
        /// <param name="timerId">定时器id。</param>
        public void RemoveTimer(int timerId)
        {
            m_TimerDispatcher.RemoveTimer(timerId);
        }

        /// <summary>
        /// 更新定时器。
        /// </summary>
        private void OnUpdate(float elapseSeconds)
        {
            m_TimerDispatcher.Update(elapseSeconds);
        }

        protected override void OnAwake()
        {
            m_TimerDispatcher = TimerModule.Instance.CreateTimerDispatcher();

            m_GameUpdateManager = Context.GetManager<IGameUpdateManager>();
            m_GameUpdateManager.RegisterFixedUpdate(this, OnUpdate);
        }

        protected override void OnDispose()
        {
            m_GameUpdateManager.UnregisterFixedUpdate(this, OnUpdate);

            m_TimerDispatcher.Dispose();
            m_TimerDispatcher = null;

            m_GameUpdateManager = null;
        }
    }
}