using System;
using System.Collections.Generic;
using HoweFramework;

namespace GameMain
{
    public partial class GameContextBase
    {
        private sealed class UpdateContext : IDisposable
        {
            private readonly List<UpdateInfo> m_UpdateInfoList = new();
            private readonly Dictionary<object, UpdateDict> m_UpdateDict = new();

            public void Register(object target, UpdateDelegate updateDelegate)
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

            public void Unregister(object target, UpdateDelegate updateDelegate)
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
            public Dictionary<UpdateDelegate, UpdateInfo> Dict = new();

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
            public UpdateDelegate UpdateDelegate { get; set; }
            public bool IsRemove { get; set; }
            public UpdateDict UpdateDict { get; set; }

            public void Clear()
            {
                Target = null;
                UpdateDelegate = null;
                IsRemove = false;
                UpdateDict = null;
            }

            public static UpdateInfo Create(object target, UpdateDelegate updateDelegate, UpdateDict updateDict)
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

        public void Update(float elapseSeconds)
        {
            if (GameStatus != GameStatus.Running)
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
                
            EventDispatcher.Update();
            m_UpdateContext.Update(elapseSeconds, ContinueFunc);
            m_LateUpdateContext.Update(elapseSeconds, ContinueFunc);
            
            return;

            bool ContinueFunc() => GameStatus != GameStatus.Stopped;
        }

        public void RegisterUpdate(object target, UpdateDelegate updateDelegate)
        {
            m_UpdateContext.Register(target, updateDelegate);
        }

        public void UnregisterUpdate(object target, UpdateDelegate updateDelegate)
        {
            m_UpdateContext.Unregister(target, updateDelegate);
        }

        public void RegisterFixedUpdate(object target, UpdateDelegate updateDelegate)
        {
            m_FixedUpdateContext.Register(target, updateDelegate);
        }

        public void UnregisterFixedUpdate(object target, UpdateDelegate updateDelegate)
        {
            m_FixedUpdateContext.Unregister(target, updateDelegate);
        }

        public void RegisterLateUpdate(object target, UpdateDelegate updateDelegate)
        {
            m_LateUpdateContext.Register(target, updateDelegate);
        }

        public void UnregisterLateUpdate(object target, UpdateDelegate updateDelegate)
        {
            m_LateUpdateContext.Unregister(target, updateDelegate);
        }

        public void RegisterLateFixedUpdate(object target, UpdateDelegate updateDelegate)
        {
            m_LateFixedUpdateContext.Register(target, updateDelegate);
        }

        public void UnregisterLateFixedUpdate(object target, UpdateDelegate updateDelegate)
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