using UnityEngine;

namespace GameMain
{
    /// <summary>
    /// 玩法场景管理器。
    /// </summary>
    [GameManager(GameManagerType.Scene)]
    public interface IGameSceneManager : IGameManager
    {
        /// <summary>
        /// 场景根节点。
        /// </summary>
        Transform SceneRoot { get; }

        /// <summary>
        /// 视图对象根节点。
        /// </summary>
        Transform ViewRoot { get; }

        /// <summary>
        /// 获取指定名字的场景对象。
        /// </summary>
        Object GetSceneObject(string objectName);

        /// <summary>
        /// 获取指定名字指定类型的场景对象。
        /// </summary>
        T GetSceneObject<T>(string objectName) where T : Object;
    }
}