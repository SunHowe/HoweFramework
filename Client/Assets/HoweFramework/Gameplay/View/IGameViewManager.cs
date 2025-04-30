using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 游戏视图管理器。
    /// </summary>
    [GameManager((int)GameManagerType.View)]
    public interface IGameViewManager : IGameManager
    {
        /// <summary>
        /// 视图根节点。
        /// </summary>
        Transform ViewRoot { get; }

        /// <summary>
        /// 生成视图对象。
        /// </summary>
        IViewObject SpawnViewObject();

        /// <summary>
        /// 销毁视图对象。
        /// </summary>
        void DisposeViewObject(IViewObject viewObject);
    }
}