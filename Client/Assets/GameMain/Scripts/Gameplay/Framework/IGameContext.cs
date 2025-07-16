using System;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 玩法上下文接口.
    /// </summary>
    public interface IGameContext : IDisposable
    {
        /// <summary>
        /// 事件调度器。
        /// </summary>
        IEventDispatcher EventDispatcher { get; set; }

        /// <summary>
        /// 游戏对象池。
        /// </summary>
        IGameObjectPool GameObjectPool { get; set; }

        /// <summary>
        /// 资源加载器。
        /// </summary>
        IResLoader ResLoader { get; set; }

        /// <summary>
        /// 游戏运行状态.
        /// </summary>
        GameStatus GameStatus { get; }

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
        /// 获取游戏管理器实例.
        /// </summary>
        IGameManager GetManager(int managerType);
    }
}