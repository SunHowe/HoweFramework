using System;

namespace HoweFramework
{
    public delegate void UpdateDelegate(float elapseSeconds);
    
    /// <summary>
    /// 玩法上下文接口.
    /// </summary>
    public interface IGameContext : IDisposable
    {
        /// <summary>
        /// 事件调度器。
        /// </summary>
        IEventDispatcher EventDispatcher { get; }

        /// <summary>
        /// 游戏运行状态.
        /// </summary>
        GameStatus GameStatus { get; }
        
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
        /// 初始化回调.
        /// </summary>
        void Awake();

        /// <summary>
        /// 开始游戏.
        /// </summary>
        void StartGame();

        /// <summary>
        /// 暂停游戏.
        /// </summary>
        void PauseGame();

        /// <summary>
        /// 恢复游戏.
        /// </summary>
        void ResumeGame();

        /// <summary>
        /// 结束游戏
        /// </summary>
        void StopGame();
        
        /// <summary>
        /// 帧更新回调.
        /// </summary>
        void Update(float elapseSeconds);

        /// <summary>
        /// 获取游戏管理器实例.
        /// </summary>
        IGameManager GetManager(int managerType);
        
        /// <summary>
        /// 注册帧更新能力.
        /// </summary>
        void RegisterUpdate(object target, UpdateDelegate updateDelegate);

        /// <summary>
        /// 注销帧更新能力.
        /// </summary>
        void UnregisterUpdate(object target, UpdateDelegate updateDelegate);

        /// <summary>
        /// 注册固定帧更新能力.
        /// </summary>
        void RegisterFixedUpdate(object target, UpdateDelegate updateDelegate);
        
        /// <summary>
        /// 注销固定帧更新能力.
        /// </summary>
        void UnregisterFixedUpdate(object target, UpdateDelegate updateDelegate);
        
        /// <summary>
        /// 注册帧后更新能力.
        /// </summary>
        void RegisterLateUpdate(object target, UpdateDelegate updateDelegate);

        /// <summary>
        /// 注销帧后更新能力.
        /// </summary>
        void UnregisterLateUpdate(object target, UpdateDelegate updateDelegate);

        /// <summary>
        /// 注册帧后固定更新能力.
        /// </summary>
        void RegisterLateFixedUpdate(object target, UpdateDelegate updateDelegate);

        /// <summary>
        /// 注销帧后固定更新能力.
        /// </summary>
        void UnregisterLateFixedUpdate(object target, UpdateDelegate updateDelegate);

        /// <summary>
        /// 注销所有更新能力.
        /// </summary>
        void UnregisterByTarget(object target);
    }
}