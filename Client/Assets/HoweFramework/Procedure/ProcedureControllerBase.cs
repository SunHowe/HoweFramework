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
    }
}