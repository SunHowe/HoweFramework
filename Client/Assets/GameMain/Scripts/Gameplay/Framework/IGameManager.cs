using System;

namespace GameMain
{
    /// <summary>
    /// 玩法管理器接口.
    /// </summary>
    public interface IGameManager : IDisposable
    {
        /// <summary>
        /// 管理器类型唯一id.
        /// </summary>
        int ManagerType { get; }

        /// <summary>
        /// 所属的上下文实例.
        /// </summary>
        IGameContext Context { get; }
        
        /// <summary>
        /// 初始化回调.
        /// </summary>
        void Awake(IGameContext context);
    }
}