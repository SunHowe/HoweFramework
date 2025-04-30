namespace GameMain
{
    /// <summary>
    /// 游戏实体引用.
    /// </summary>
    public readonly struct GameEntityRef
    {
        private readonly IGameEntity m_GameEntity;
        private readonly int m_GameEntityId;

        public GameEntityRef(IGameEntity gameEntity)
        {
            m_GameEntity = gameEntity;
            m_GameEntityId = gameEntity?.EntityId ?? 0;
        }

        /// <summary>
        /// 获取有效的实体引用.
        /// </summary>
        public IGameEntity GameEntity => m_GameEntity?.EntityId != m_GameEntityId ? null : m_GameEntity;
    }
}