using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 玩法上下文实例抽象类.
    /// </summary>
    public abstract partial class GameContextBase : IGameContext
    {
        public GameStatus GameStatus { get; private set; }
        public float GameTime { get; private set; }
        public float GameFixedTime { get; private set; }
        public int GameFrame { get; private set; }
        public float GameFixedDeltaTime { get; private set; }
        public virtual int GameFrameRate => 20;

        private readonly Dictionary<int, IGameManager> m_GameManagerDict = new();
        private readonly List<IGameManager> m_GameManagerList = new();
        private float m_NextFixedUpdateTime;

        public void Awake()
        {
            GameStatus = GameStatus.None;
            GameTime = 0f;
            GameFixedTime = 0f;
            GameFrame = 0;
            GameFixedDeltaTime = 1f / GameFrameRate;
            m_NextFixedUpdateTime = GameFixedDeltaTime;

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

            m_UpdateContext.Dispose();
            m_FixedUpdateContext.Dispose();
            m_LateUpdateContext.Dispose();
            m_LateFixedUpdateContext.Dispose();
        }

        public void StartGame()
        {
            if (GameStatus != GameStatus.None)
            {
                return;
            }

            GameStatus = GameStatus.Running;

            foreach (var manager in m_GameManagerList)
            {
                manager.OnStartGame();
            }
        }

        public void PauseGame()
        {
            if (GameStatus != GameStatus.Running)
            {
                return;
            }

            GameStatus = GameStatus.Pause;
        }

        public void ResumeGame()
        {
            if (GameStatus != GameStatus.Pause)
            {
                return;
            }

            GameStatus = GameStatus.Running;
        }

        public void StopGame()
        {
            if (GameStatus == GameStatus.Stopped || GameStatus == GameStatus.None)
            {
                return;
            }
            
            GameStatus = GameStatus.Stopped;

            for (var index = m_GameManagerList.Count - 1; index >= 0; index--)
            {
                var manager = m_GameManagerList[index];
                manager.OnStopGame();
            }
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