namespace GameMain
{
    public delegate void GameUpdateDelegate(float elapseSeconds);

    /// <summary>
    /// 游戏更新管理器。
    /// </summary>
    [GameManager(GameManagerType.Update)]
    public interface IGameUpdateManager : IGameManager
    {
        /// <summary>
        /// 当前游戏时间.
        /// </summary>
        float GameTime { get; }

        /// <summary>
        /// 当前游戏逻辑帧时间.
        /// </summary>
        float GameFixedTime { get; }

        /// <summary>
        /// 当前游戏逻辑帧序号.
        /// </summary>
        int GameFrame { get; }

        /// <summary>
        /// 当前游戏逻辑帧帧率.
        /// </summary>
        int GameFrameRate { get; }

        /// <summary>
        /// 游戏逻辑帧deltaTime.
        /// </summary>
        float GameFixedDeltaTime { get; }

        /// <summary>
        /// 提供给外部驱动游戏更新的接口.
        /// </summary>
        void Update(float elapseSeconds);

        /// <summary>
        /// 注册帧更新能力.
        /// </summary>
        void RegisterUpdate(object target, GameUpdateDelegate updateDelegate);

        /// <summary>
        /// 注销帧更新能力.
        /// </summary>
        void UnregisterUpdate(object target, GameUpdateDelegate updateDelegate);

        /// <summary>
        /// 注册固定帧更新能力.
        /// </summary>
        void RegisterFixedUpdate(object target, GameUpdateDelegate updateDelegate);

        /// <summary>
        /// 注销固定帧更新能力.
        /// </summary>
        void UnregisterFixedUpdate(object target, GameUpdateDelegate updateDelegate);

        /// <summary>
        /// 注册帧后更新能力.
        /// </summary>
        void RegisterLateUpdate(object target, GameUpdateDelegate updateDelegate);

        /// <summary>
        /// 注销帧后更新能力.
        /// </summary>
        void UnregisterLateUpdate(object target, GameUpdateDelegate updateDelegate);

        /// <summary>
        /// 注册帧后固定更新能力.
        /// </summary>
        void RegisterLateFixedUpdate(object target, GameUpdateDelegate updateDelegate);

        /// <summary>
        /// 注销帧后固定更新能力.
        /// </summary>
        void UnregisterLateFixedUpdate(object target, GameUpdateDelegate updateDelegate);

        /// <summary>
        /// 注销所有更新能力.
        /// </summary>
        void UnregisterByTarget(object target);
    }
}
