using HoweFramework;
using UnityEngine;

namespace GameMain
{
    /// <summary>
    /// 玩法场景管理器。
    /// </summary>
    public sealed class GameSceneManager : GameManagerBase, IGameSceneManager
    {
        /// <summary>
        /// 场景对象收集器。
        /// </summary>
        private ObjectCollector m_ObjectCollector;

        /// <summary>
        /// 场景根节点名称。
        /// </summary>
        private readonly string m_SceneRootName;

        public GameSceneManager() : this("SceneRoot")
        {
        }

        public GameSceneManager(string sceneRootName)
        {
            m_SceneRootName = sceneRootName;
        }

        /// <inheritdoc/>
        public Object GetSceneObject(string objectName)
        {
            return m_ObjectCollector.GetObject(objectName);
        }

        /// <inheritdoc/>
        public T GetSceneObject<T>(string objectName) where T : Object
        {
            return m_ObjectCollector.Get<T>(objectName);
        }

        protected override void OnAwake()
        {
            var sceneRoot = GameObject.Find(m_SceneRootName);
            m_ObjectCollector = sceneRoot.GetComponent<ObjectCollector>();
        }

        protected override void OnDispose()
        {
            m_ObjectCollector = null;
        }
    }
}