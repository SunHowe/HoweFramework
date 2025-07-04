using System.Collections.Generic;
using UnityEngine;

namespace GameMain
{
    /// <summary>
    /// 游戏视图管理器。
    /// </summary>
    public sealed partial class GameViewManager : GameManagerBase, IGameViewManager
    {
        public Transform ViewRoot { get; private set; }

        private readonly Dictionary<int, ViewObject> m_ViewObjectDict = new();
        private int m_IdIncrease;
        private bool m_ViewRootManaged = false;

        public IViewObject SpawnViewObject()
        {
            var id = ++m_IdIncrease;
            var viewObject = ViewObject.Create(this, id);
            viewObject.ParentTransform = ViewRoot;

            m_ViewObjectDict.Add(id, viewObject);
            return viewObject;
        }

        public void DisposeViewObject(IViewObject viewObject)
        {
            var vo = (ViewObject)viewObject;
            if (m_ViewObjectDict.Remove(vo.Id))
            {
                vo.DisposeFromManager();
            }
        }

        protected override void OnAwake()
        {
            var gameSceneManager = Context.GetManager<IGameSceneManager>();
            ViewRoot = gameSceneManager?.ViewRoot;

            if (ViewRoot == null)
            {
                ViewRoot = new GameObject("ViewRoot").transform;
                m_ViewRootManaged = true;
            }
        }

        protected override void OnDispose()
        {
            foreach (var viewObject in m_ViewObjectDict.Values)
            {
                viewObject.DisposeFromManager();
            }

            m_ViewObjectDict.Clear();
            m_IdIncrease = 0;

            if (m_ViewRootManaged)
            {
                Object.Destroy(ViewRoot.gameObject);
                m_ViewRootManaged = false;
            }

            ViewRoot = null;
        }
    }
}
