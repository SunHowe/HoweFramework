namespace HoweFramework
{
    /// <summary>
    /// 游戏实体管理器.
    /// </summary>
    [GameManager((int)GameManagerType.Entity)]
    public interface IGameEntityManager : IGameManager
    {
        /// <summary>
        /// 创建实体实例.
        /// </summary>
        IGameEntity CreateEntity();

        /// <summary>
        /// 获取指定实体实例.
        /// </summary>
        IGameEntity GetEntity(int entityId);

        /// <summary>
        /// 销毁指定实体.
        /// </summary>
        void DestroyEntity(int entityId);

        /// <summary>
        /// 分配组件id。
        /// </summary>
        int SpawnComponentId();
    }
}