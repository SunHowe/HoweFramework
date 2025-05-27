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
            OnDestroy();
            m_DisposableEventSubscribe?.Dispose();
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
