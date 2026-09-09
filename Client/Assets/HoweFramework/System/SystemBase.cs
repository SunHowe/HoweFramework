namespace HoweFramework
{
    /// <summary>
    /// 系统基类。
    /// </summary>
    public abstract class SystemBase : ISystem, IEventSubscribe
    {
        public void Init()
        {
            OnInit();
        }

        public void Destroy()
        {
            try
            {
                OnDestroy();
            }
            finally
            {
                // 保证事件退订一定执行，避免订阅残留导致事件派发到已销毁对象。
                m_DisposableEventSubscribe?.Dispose();
                m_DisposableEventSubscribe = null;
            }
        }

        protected abstract void OnInit();
        protected abstract void OnDestroy();

        #region [IEventSubscribe]

        private DisposableEventSubscribe m_DisposableEventSubscribe;

        public void Subscribe(int id, GameEventHandler handler)
        {
            m_DisposableEventSubscribe ??= new DisposableEventSubscribe();
            m_DisposableEventSubscribe.Subscribe(id, handler);
        }

        public void Unsubscribe(int id, GameEventHandler handler)
        {
            m_DisposableEventSubscribe?.Unsubscribe(id, handler);
        }

        #endregion
    }
}
