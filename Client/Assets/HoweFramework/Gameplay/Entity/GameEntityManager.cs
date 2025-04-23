using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 游戏实体管理器.
    /// </summary>
    public sealed partial class GameEntityManager : GameManagerBase, IGameEntityManager
    {
        private int m_EntityIdIncrease = 0;
        private int m_ComponentIdIncrease = 0;

        private readonly Dictionary<int, GameEntity> m_EntityDict = new Dictionary<int, GameEntity>();

        protected override void OnAwake()
        {
        }

        public override void OnStartGame()
        {
        }

        public override void OnStopGame()
        {
        }

        protected override void OnDispose()
        {
            foreach (var entity in m_EntityDict.Values)
            {
                entity.DisposeFromManager();
                ReferencePool.Release(entity);
            }

            m_EntityDict.Clear();
            m_EntityIdIncrease = 0;
            m_ComponentIdIncrease = 0;
        }

        public IGameEntity CreateEntity()
        {
            var entityId = ++m_EntityIdIncrease;
            var entity = ReferencePool.Acquire<GameEntity>();

            m_EntityDict.Add(entityId, entity);

            entity.Awake(entityId, this);
            return entity;
        }

        public IGameEntity GetEntity(int entityId)
        {
            return m_EntityDict.GetValueOrDefault(entityId);
        }

        public void DestroyEntity(int entityId)
        {
            if (!m_EntityDict.Remove(entityId, out var entity))
            {
                return;
            }

            entity.DisposeFromManager();
            ReferencePool.Release(entity);
        }

        public int SpawnComponentId()
        {
            return ++m_ComponentIdIncrease;
        }
    }
}