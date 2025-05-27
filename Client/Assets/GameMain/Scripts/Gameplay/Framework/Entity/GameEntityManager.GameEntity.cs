using System;
using System.Collections.Generic;
using HoweFramework;

namespace GameMain
{
    public partial class GameEntityManager
    {
        private sealed class GameEntity : IGameEntity, IReference
        {
            public int EntityId { get; private set; }
            public IGameContext Context { get; private set; }

            private readonly List<GameComponentBase> m_ComponentList = new List<GameComponentBase>();
            private readonly Dictionary<int, GameComponentBase> m_ComponentDict = new Dictionary<int, GameComponentBase>();

            private bool m_IsDisposed = false;
            private IGameEntityManager m_EntityManager = null;

            public void Awake(int entityId, IGameEntityManager entityManager)
            {
                EntityId = entityId;
                Context = entityManager.Context;
                m_EntityManager = entityManager;

                m_IsDisposed = false;
            }

            public void Dispose()
            {
                if (m_EntityManager != null)
                {
                    m_EntityManager.DestroyEntity(EntityId);
                }
                else
                {
                    DisposeFromManager();
                }
            }

            public void DisposeFromManager()
            {
                if (m_IsDisposed)
                {
                    return;
                }

                m_IsDisposed = true;

                for (int i = m_ComponentList.Count - 1; i >= 0; i--)
                {
                    GameComponentBase component = m_ComponentList[i];
                    component.DisposeFromEntity();
                }

                m_ComponentList.Clear();
                m_ComponentDict.Clear();

                EntityId = 0;
                m_EntityManager = null;
                Context = null;
            }

            public void Clear()
            {
                // do nothing.
            }

            public void AddComponent(IGameComponent component)
            {
                if (m_IsDisposed)
                {
                    throw new Exception("实体已销毁");
                }

                if (component is not GameComponentBase gameComponent)
                {
                    throw new Exception($"组件类型 '{component.GetType()}' 不是GameComponentBase的派生类");
                }

                if (!m_ComponentDict.TryAdd(component.ComponentType, gameComponent))
                {
                    throw new Exception($"组件类型 '{component.ComponentType}' 已存在");
                }

                m_ComponentList.Add(gameComponent);
                component.Awake(m_EntityManager.SpawnComponentId(), this);
            }

            public IGameComponent GetComponent(int componentType)
            {
                return m_ComponentDict.GetValueOrDefault(componentType);
            }

            public void RemoveComponent(int componentType)
            {
                if (m_IsDisposed)
                {
                    throw new Exception("实体已销毁");
                }

                if (!m_ComponentDict.Remove(componentType, out var component))
                {
                    return;
                }

                m_ComponentList.Remove(component);
                component.DisposeFromEntity();
            }
        }
    }
}