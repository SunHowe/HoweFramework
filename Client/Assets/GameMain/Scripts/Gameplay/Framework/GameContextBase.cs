using System;
using System.Collections.Generic;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 玩法上下文实例抽象类.
    /// </summary>
    public abstract partial class GameContextBase : IGameContext
    {
        public IEventDispatcher EventDispatcher
        {
            get => m_EventDispatcher;
            set
            {
                if (GameStatus != GameStatus.None)
                {
                    throw new ErrorCodeException(ErrorCode.InvalidOperationException, "GameContext already initialized.");
                }

                m_EventDispatcher = value;
            }
        }
        public IGameObjectPool GameObjectPool
        {
            get => m_GameObjectPool;
            set
            {
                if (GameStatus != GameStatus.None)
                {
                    throw new ErrorCodeException(ErrorCode.InvalidOperationException, "GameContext already initialized.");
                }

                m_GameObjectPool = value;
            }
        }
        public IResLoader ResLoader
        {
            get => m_ResLoader;
            set
            {
                if (GameStatus != GameStatus.None)
                {
                    throw new ErrorCodeException(ErrorCode.InvalidOperationException, "GameContext already initialized.");
                }

                m_ResLoader = value;
            }
        }

        public GameStatus GameStatus { get; private set; }

        private readonly Dictionary<int, IGameManager> m_GameManagerDict = new();
        private readonly List<IGameManager> m_GameManagerList = new();

        private IEventDispatcher m_EventDispatcher;
        private IGameObjectPool m_GameObjectPool;
        private IResLoader m_ResLoader;

        private bool m_EventDispatcherManaged = false;
        private bool m_ResLoaderManaged = false;
        private bool m_GameObjectPoolManaged = false;

        public void Awake()
        {
            if (GameStatus != GameStatus.None)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "GameContext already initialized.");
            }

            if (EventDispatcher == null)
            {
                m_EventDispatcherManaged = true;
                m_EventDispatcher = EventModule.Instance.CreateEventDispatcher();
                m_EventDispatcher.SetMode(EventDispatcherMode.AllowNoHandler | EventDispatcherMode.AllowMultiHandler);
            }

            if (ResLoader == null)
            {
                m_ResLoaderManaged = true;
                m_ResLoader = ResModule.Instance.CreateResLoader();
            }

            if (GameObjectPool == null)
            {
                m_GameObjectPoolManaged = true;
                m_GameObjectPool = GameObjectPoolModule.Instance.CreateGameObjectPool(m_ResLoader);
            }

            GameStatus = GameStatus.Initialize;

            OnAwake();

            foreach (var manager in m_GameManagerList)
            {
                manager.Awake(this);
            }

            OnAfterAwake();
        }

        public void Dispose()
        {
            if (GameStatus == GameStatus.None)
            {
                return;
            }

            if (GameStatus >= GameStatus.Running)
            {
                StopGame();
            }

            OnDispose();

            for (var index = m_GameManagerList.Count - 1; index >= 0; index--)
            {
                var manager = m_GameManagerList[index];
                manager.Dispose();
            }

            m_GameManagerDict.Clear();
            m_GameManagerList.Clear();

            if (m_EventDispatcherManaged)
            {
                m_EventDispatcher.Dispose();
                m_EventDispatcher = null;
                m_EventDispatcherManaged = false;
            }

            if (m_GameObjectPoolManaged)
            {
                m_GameObjectPool.Dispose();
                m_GameObjectPool = null;
                m_GameObjectPoolManaged = false;
            }

            if (m_ResLoaderManaged)
            {
                m_ResLoader.Dispose();
                m_ResLoader = null;
                m_ResLoaderManaged = false;
            }

            GameStatus = GameStatus.None;
        }

        public void StartGame()
        {
            if (GameStatus != GameStatus.Initialize)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "GameContext not initialized.");
            }

            GameStatus = GameStatus.Running;

            EventDispatcher.Dispatch(this, GameStartEventArgs.Create());
            EventDispatcher.Dispatch(this, GameStatusChangeEventArgs.Create(GameStatus));
        }

        public void PauseGame()
        {
            if (GameStatus != GameStatus.Running)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "GameContext not running.");
            }

            GameStatus = GameStatus.Pause;
            EventDispatcher.Dispatch(this, GameStatusChangeEventArgs.Create(GameStatus));
        }

        public void ResumeGame()
        {
            if (GameStatus != GameStatus.Pause)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "GameContext not paused.");
            }

            GameStatus = GameStatus.Running;
            EventDispatcher.Dispatch(this, GameStatusChangeEventArgs.Create(GameStatus));
        }

        public void StopGame()
        {
            if (GameStatus < GameStatus.Running)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "GameContext not running.");
            }

            GameStatus = GameStatus.Stopped;
            EventDispatcher.Dispatch(this, GameStopEventArgs.Create());
            EventDispatcher.Dispatch(this, GameStatusChangeEventArgs.Create(GameStatus));
        }

        public IGameManager GetManager(int managerType)
        {
            return m_GameManagerDict.GetValueOrDefault(managerType);
        }

        /// <summary>
        /// 添加管理器实例.
        /// </summary>
        protected void AddManager(IGameManager manager)
        {
            if (!m_GameManagerDict.TryAdd(manager.ManagerType, manager))
            {
                throw new Exception(string.Format("Already exist game manager type '{0}'", manager.ManagerType));
            }

            m_GameManagerList.Add(manager);
        }

        /// <summary>
        /// 添加管理器实例.
        /// </summary>
        protected T AddManager<T>() where T : IGameManager, new()
        {
            var manager = new T();
            AddManager(manager);
            return manager;
        }

        protected abstract void OnAwake();
        protected abstract void OnAfterAwake();
        protected abstract void OnDispose();
    }
}