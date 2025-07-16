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
        /// 默认场景根节点名称。
        /// </summary>
        public const string DefaultSceneRootName = "SceneRoot";

        /// <summary>
        /// 默认视图对象根节点名称。
        /// </summary>
        public const string DefaultViewRootName = "ViewRoot";

        /// <inheritdoc/>
        public Transform SceneRoot { get; private set; }

        /// <inheritdoc/>
        public Transform ViewRoot { get; private set; }

        /// <summary>
        /// 场景对象收集器。
        /// </summary>
        private ObjectCollector m_ObjectCollector;

        /// <summary>
        /// 场景根节点名称。
        /// </summary>
        private readonly string m_SceneRootName;

        /// <summary>
        /// 视图对象根节点名称。
        /// </summary>
        private readonly string m_ViewRootName;

        public GameSceneManager() : this(DefaultSceneRootName, DefaultViewRootName)
        {
        }

        public GameSceneManager(string sceneRootName, string viewRootName)
        {
            m_SceneRootName = sceneRootName;
            m_ViewRootName = viewRootName;
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

            SceneRoot = sceneRoot.transform;
            ViewRoot = GetSceneObject<Transform>(m_ViewRootName);
        }

        protected override void OnDispose()
        {
            m_ObjectCollector = null;
        }
    }
}