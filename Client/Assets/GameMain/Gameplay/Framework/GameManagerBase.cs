namespace GameMain
{
    /// <summary>
    /// 玩法管理器抽象类.
    /// </summary>
    public abstract class GameManagerBase : IGameManager
    {
        public int ManagerType => GameManagerHelper.GetManagerType(GetType());
        
        /// <summary>
        /// 管理器所属的上下文实例.
        /// </summary>
        public IGameContext Context { get; private set; }

        public void Awake(IGameContext context)
        {
            Context = context;
            OnAwake();
        }

        public void Dispose()
        {
            OnDispose();
            Context = null;
        }

        protected abstract void OnAwake();
        public abstract void OnStartGame();
        public abstract void OnStopGame();
        protected abstract void OnDispose();
    }
}