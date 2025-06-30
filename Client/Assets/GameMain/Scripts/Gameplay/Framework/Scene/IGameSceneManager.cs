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
        /// 获取指定名字的场景对象。
        /// </summary>
        Object GetSceneObject(string objectName);

        /// <summary>
        /// 获取指定名字指定类型的场景对象。
        /// </summary>
        T GetSceneObject<T>(string objectName) where T : Object;
    }
}