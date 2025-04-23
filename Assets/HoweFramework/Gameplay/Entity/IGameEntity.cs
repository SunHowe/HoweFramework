using System;

namespace HoweFramework
{
    /// <summary>
    /// 游戏实体接口.
    /// </summary>
    public interface IGameEntity : IDisposable
    {
        /// <summary>
        /// 实体id.
        /// </summary>
        int EntityId { get; }
        
        /// <summary>
        /// 所属的上下文实例.
        /// </summary>
        IGameContext Context { get; }
        
        /// <summary>
        /// 初始化回调.
        /// </summary>
        void Awake(int entityId, IGameEntityManager entityManager);

        /// <summary>
        /// 添加组件.
        /// </summary>
        void AddComponent(IGameComponent component);

        /// <summary>
        /// 获取指定组件.
        /// </summary>
        IGameComponent GetComponent(int componentType);
        
        /// <summary>
        /// 移除指定组件.
        /// </summary>
        void RemoveComponent(int componentType);
    }
}