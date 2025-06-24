namespace HoweFramework
{
    /// <summary>
    /// 流程控制器基类。
    /// </summary>
    public abstract class ProcedureControllerBase : IProcedureController
    {
        /// <summary>
        /// 可销毁对象组。
        /// </summary>
        protected DisposableGroup DisposableGroup => (m_DisposableGroup ??= new());

        /// <summary>
        /// 可销毁对象组。
        /// </summary>
        private DisposableGroup m_DisposableGroup;

        /// <summary>
        /// 帧定时器id。
        /// </summary>
        private int m_FrameTimerId;

        /// <summary>
        /// 初始化。
        /// </summary>
        public void Initialize()
        {
            OnInitialize();
        }

        /// <summary>
        /// 销毁。
        /// </summary>
        public void Dispose()
        {
            OnDispose();
            
            DisableFrameTimer();
            m_DisposableGroup?.Dispose();
            m_DisposableGroup = null;
        }

        /// <summary>
        /// 初始化时调用。
        /// </summary>
        protected abstract void OnInitialize();

        /// <summary>
        /// 销毁时调用。
        /// </summary>
        protected abstract void OnDispose();

        /// <summary>
        /// 帧定时器回调。需要通过EnableFrameTimer启用。
        /// </summary>
        protected virtual void OnUpdate()
        {
        }

        /// <summary>
        /// 启用帧定时器。
        /// </summary>
        protected void EnableFrameTimer()
        {
            if (m_FrameTimerId != 0)
            {
                return;
            }

            m_FrameTimerId = TimerModule.Instance.AddFrameTimer(OnUpdate);
        }

        /// <summary>
        /// 禁用帧定时器。
        /// </summary>
        protected void DisableFrameTimer()
        {
            if (m_FrameTimerId == 0)
            {
                return;
            }

            TimerModule.Instance.RemoveTimer(m_FrameTimerId);
            m_FrameTimerId = 0;
        }
    }
}