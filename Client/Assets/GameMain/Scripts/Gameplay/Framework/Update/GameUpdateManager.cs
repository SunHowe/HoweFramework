using System;
using System.Collections.Generic;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 游戏更新管理器。
    /// </summary>
    public sealed class GameUpdateManager : GameManagerBase, IGameUpdateManager
    {
        public float GameTime { get; private set; }
        public float GameFixedTime { get; private set; }
        public int GameFrame { get; private set; }
        public float GameFixedDeltaTime { get; private set; }
        public int GameFrameRate { get; }

        private sealed class UpdateContext : IDisposable
        {
            private readonly List<UpdateInfo> m_UpdateInfoList = new();
            private readonly Dictionary<object, UpdateDict> m_UpdateDict = new();

            public void Register(object target, GameUpdateDelegate updateDelegate)
            {
                if (m_UpdateDict.TryGetValue(target, out var updateDict))
                {
                    if (updateDict.Dict.TryGetValue(updateDelegate, out var updateInfo))
                    {
                        updateInfo.IsRemove = false;
                        return;
                    }

                    updateInfo = UpdateInfo.Create(target, updateDelegate, updateDict);
                    m_UpdateInfoList.Add(updateInfo);
                }
                else
                {
                    updateDict = UpdateDict.Create();
                    m_UpdateDict.Add(target, updateDict);

                    var updateInfo = UpdateInfo.Create(target, updateDelegate, updateDict);
                    m_UpdateInfoList.Add(updateInfo);
                }
            }

            public void Unregister(object target, GameUpdateDelegate updateDelegate)
            {
                if (!m_UpdateDict.TryGetValue(target, out var updateDict))
                {
                    return;
                }

                if (!updateDict.Dict.TryGetValue(updateDelegate, out var updateInfo))
                {
                    return;
                }

                updateInfo.IsRemove = true;
            }

            public void UnregisterByTarget(object target)
            {
                if (!m_UpdateDict.Remove(target, out var updateDict))
                {
                    return;
                }

                foreach (var updateInfo in updateDict.Dict.Values)
                {
                    updateInfo.IsRemove = true;
                }

                ReferencePool.Release(updateDict);
            }

            public void Update(float elapsedSeconds, Func<bool> continueFunc)
            {
                for (var index = m_UpdateInfoList.Count - 1; index >= 0; index--)
                {
                    var updateInfo = m_UpdateInfoList[index];
                    if (updateInfo.IsRemove)
                    {
                        m_UpdateInfoList.RemoveAt(index);
                        updateInfo.UpdateDict.Dict.Remove(updateInfo.UpdateDelegate);
                        ReferencePool.Release(updateInfo);
                        continue;
                    }

                    updateInfo.UpdateDelegate(elapsedSeconds);

                    if (!continueFunc())
                    {
                        return;
                    }
                }
            }

            public void Dispose()
            {
                foreach (var updateDict in m_UpdateDict.Values)
                {
                    ReferencePool.Release(updateDict);
                }

                m_UpdateDict.Clear();

                foreach (var updateInfo in m_UpdateInfoList)
                {
                    ReferencePool.Release(updateInfo);
                }

                m_UpdateInfoList.Clear();
            }
        }

        private sealed class UpdateDict : IReference
        {
            public Dictionary<GameUpdateDelegate, UpdateInfo> Dict = new();

            public void Clear()
            {
                Dict.Clear();
            }

            public static UpdateDict Create()
            {
                return ReferencePool.Acquire<UpdateDict>();
            }
        }

        private sealed class UpdateInfo : IReference
        {
            public object Target { get; set; }
            public GameUpdateDelegate UpdateDelegate { get; set; }
            public bool IsRemove { get; set; }
            public UpdateDict UpdateDict { get; set; }

            public void Clear()
            {
                Target = null;
                UpdateDelegate = null;
                IsRemove = false;
                UpdateDict = null;
            }

            public static UpdateInfo Create(object target, GameUpdateDelegate updateDelegate, UpdateDict updateDict)
            {
                var inst = ReferencePool.Acquire<UpdateInfo>();
                inst.Target = target;
                inst.UpdateDelegate = updateDelegate;
                inst.UpdateDict = updateDict;

                updateDict.Dict.Add(updateDelegate, inst);
                return inst;
            }
        }

        private readonly UpdateContext m_UpdateContext = new();
        private readonly UpdateContext m_FixedUpdateContext = new();
        private readonly UpdateContext m_LateUpdateContext = new();
        private readonly UpdateContext m_LateFixedUpdateContext = new();
        private float m_NextFixedUpdateTime;

        public GameUpdateManager(int frameRate = 20)
        {
            GameFrameRate = frameRate;
        }

        protected override void OnAwake()
        {
            GameTime = 0f;
            GameFixedTime = 0f;
            GameFrame = 0;
            GameFixedDeltaTime = 1f / GameFrameRate;
            m_NextFixedUpdateTime = GameFixedDeltaTime;
        }

        protected override void OnDispose()
        {
            m_UpdateContext.Dispose();
            m_FixedUpdateContext.Dispose();
            m_LateUpdateContext.Dispose();
            m_LateFixedUpdateContext.Dispose();
        }

        public void Update(float elapseSeconds)
        {
            if (Context.GameStatus != GameStatus.Running)
            {
                return;
            }

            GameTime += elapseSeconds;

            while (GameTime >= m_NextFixedUpdateTime)
            {
                GameFixedTime = m_NextFixedUpdateTime;
                m_NextFixedUpdateTime += GameFixedDeltaTime;
                ++GameFrame;

                m_FixedUpdateContext.Update(GameFixedDeltaTime, ContinueFunc);
                m_LateFixedUpdateContext.Update(GameFixedDeltaTime, ContinueFunc);
            }
                
            m_UpdateContext.Update(elapseSeconds, ContinueFunc);
            m_LateUpdateContext.Update(elapseSeconds, ContinueFunc);
            
            return;

            bool ContinueFunc() => Context.GameStatus != GameStatus.Stopped;
        }

        public void RegisterUpdate(object target, GameUpdateDelegate updateDelegate)
        {
            m_UpdateContext.Register(target, updateDelegate);
        }

        public void UnregisterUpdate(object target, GameUpdateDelegate updateDelegate)
        {
            m_UpdateContext.Unregister(target, updateDelegate);
        }

        public void RegisterFixedUpdate(object target, GameUpdateDelegate updateDelegate)
        {
            m_FixedUpdateContext.Register(target, updateDelegate);
        }

        public void UnregisterFixedUpdate(object target, GameUpdateDelegate updateDelegate)
        {
            m_FixedUpdateContext.Unregister(target, updateDelegate);
        }

        public void RegisterLateUpdate(object target, GameUpdateDelegate updateDelegate)
        {
            m_LateUpdateContext.Register(target, updateDelegate);
        }

        public void UnregisterLateUpdate(object target, GameUpdateDelegate updateDelegate)
        {
            m_LateUpdateContext.Unregister(target, updateDelegate);
        }

        public void RegisterLateFixedUpdate(object target, GameUpdateDelegate updateDelegate)
        {
            m_LateFixedUpdateContext.Register(target, updateDelegate);
        }

        public void UnregisterLateFixedUpdate(object target, GameUpdateDelegate updateDelegate)
        {
            m_LateFixedUpdateContext.Unregister(target, updateDelegate);
        }

        public void UnregisterByTarget(object target)
        {
            m_UpdateContext.UnregisterByTarget(target);
            m_FixedUpdateContext.UnregisterByTarget(target);
            m_LateUpdateContext.UnregisterByTarget(target);
            m_LateFixedUpdateContext.UnregisterByTarget(target);
        }
    }
}
