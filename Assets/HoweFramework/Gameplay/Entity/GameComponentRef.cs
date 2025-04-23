namespace HoweFramework
{
    /// <summary>
    /// 游戏组件引用.
    /// </summary>
    public readonly struct GameComponentRef<T> where T : class, IGameComponent
    {
        private readonly T m_GameComponent;
        private readonly int m_GameComponentId;

        public GameComponentRef(T gameComponent)
        {
            m_GameComponent = gameComponent;
            m_GameComponentId = gameComponent?.ComponentId ?? 0;
        }
        
        /// <summary>
        /// 获取有效的组件引用.
        /// </summary>
        public T GameComponent => m_GameComponent?.ComponentId != m_GameComponentId ? null : m_GameComponent;

        /// <summary>
        /// 隐式转换为组件.
        /// </summary>
        /// <param name="gameComponentRef">游戏组件引用.</param>
        public static implicit operator T(GameComponentRef<T> gameComponentRef)
        {
            return gameComponentRef.GameComponent;
        }

        /// <summary>
        /// 隐式转换为游戏组件引用.
        /// </summary>
        /// <param name="gameComponent">游戏组件.</param>
        public static implicit operator GameComponentRef<T>(T gameComponent)
        {
            return new GameComponentRef<T>(gameComponent);
        }
    }
}