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
        public IEventDispatcher EventDispatcher { get; private set; }
        public IGameObjectPool GameObjectPool { get; private set; }
        public IResLoader ResLoader { get; private set; }

        public GameStatus GameStatus { get; private set; }

        private readonly Dictionary<int, IGameManager> m_GameManagerDict = new();
        private readonly List<IGameManager> m_GameManagerList = new();
        private SimpleEvent<GameStatus> m_GameStatusChangeEvent;

        public void Awake()
        {
            EventDispatcher = EventModule.Instance.CreateEventDispatcher();
            EventDispatcher.SetMode(EventDispatcherMode.AllowNoHandler | EventDispatcherMode.AllowMultiHandler);
            ResLoader = ResModule.Instance.CreateResLoader();
            GameObjectPool = GameObjectPoolModule.Instance.CreateGameObjectPool(ResLoader);

            GameStatus = GameStatus.None;
            m_GameStatusChangeEvent = SimpleEvent<GameStatus>.Create();

            OnAwake();

            foreach (var manager in m_GameManagerList)
            {
                manager.Awake(this);
            }

            OnAfterAwake();
        }

        public void Dispose()
        {
            StopGame();
            OnDispose();

            for (var index = m_GameManagerList.Count - 1; index >= 0; index--)
            {
                var manager = m_GameManagerList[index];
                manager.Dispose();
            }

            m_GameManagerDict.Clear();
            m_GameManagerList.Clear();
            
            EventDispatcher.Dispose();
            GameObjectPool.Dispose();
            ResLoader.Dispose();
        }

        public void StartGame()
        {
            if (GameStatus != GameStatus.None)
            {
                return;
            }

            GameStatus = GameStatus.Running;
            m_GameStatusChangeEvent.Dispatch(GameStatus);
        }

        public void PauseGame()
        {
            if (GameStatus != GameStatus.Running)
            {
                return;
            }

            GameStatus = GameStatus.Pause;
            m_GameStatusChangeEvent.Dispatch(GameStatus);
        }

        public void ResumeGame()
        {
            if (GameStatus != GameStatus.Pause)
            {
                return;
            }

            GameStatus = GameStatus.Running;
            m_GameStatusChangeEvent.Dispatch(GameStatus);
        }

        public void StopGame()
        {
            if (GameStatus == GameStatus.Stopped || GameStatus == GameStatus.None)
            {
                return;
            }
            
            GameStatus = GameStatus.Stopped;
            m_GameStatusChangeEvent.Dispatch(GameStatus);
        }

        public IGameManager GetManager(int managerType)
        {
            return m_GameManagerDict.GetValueOrDefault(managerType);
        }

        public void SubscribeGameStatusChange(SimpleEventHandler<GameStatus> handler)
        {
            m_GameStatusChangeEvent.Subscribe(handler);
        }

        public void UnsubscribeGameStatusChange(SimpleEventHandler<GameStatus> handler)
        {
            m_GameStatusChangeEvent.Unsubscribe(handler);
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