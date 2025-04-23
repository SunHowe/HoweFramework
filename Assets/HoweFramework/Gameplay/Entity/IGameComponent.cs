using System;

namespace HoweFramework
{
    /// <summary>
    /// 游戏实体组件接口.
    /// </summary>
    public interface IGameComponent : IDisposable
    {
        /// <summary>
        /// 组件类型唯一id.
        /// </summary>
        int ComponentType { get; }
        
        /// <summary>
        /// 组件实例唯一id.
        /// </summary>
        int ComponentId { get; }
        
        /// <summary>
        /// 所属的实体.
        /// </summary>
        IGameEntity Entity { get; }
        
        /// <summary>
        /// 所属的上下文实例.
        /// </summary>
        IGameContext Context { get; }
        
        /// <summary>
        /// 初始化回调.
        /// </summary>
        void Awake(int componentId, IGameEntity entity);
    }
}