using HoweFramework;
using UnityEngine;

namespace GameMain
{
    public delegate void ViewComponentLoadedDelegate(ViewComponent viewComponent);
    public delegate void ViewComponentUnloadedDelegate(ViewComponent viewComponent);

    /// <summary>
    /// 视图组件.
    /// </summary>
    [GameComponent(GameComponentType.View)]
    public sealed class ViewComponent : GameComponentBase
    {
        /// <summary>
        /// 视图加载完成事件.
        /// </summary>
        public event ViewComponentLoadedDelegate OnViewLoaded
        {
            add
            {
                m_OnViewLoaded += value;
                if (IsLoaded)
                {
                    value(this);
                }
            }
            remove
            {
                m_OnViewLoaded -= value;
            }
        }

        /// <summary>
        /// 视图卸载完成事件.
        /// </summary>
        public event ViewComponentUnloadedDelegate OnViewUnloaded
        {
            add
            {
                m_OnViewUnloaded += value;
            }
            remove
            {
                m_OnViewUnloaded -= value;
            }
        }

        /// <summary>
        /// 资源键值.
        /// </summary>
        public string ResKey
        {
            get
            {
                return m_ResKey;
            }
            set
            {
                if (m_ResKey == value)
                {
                    return;
                }

                m_ResKey = value;

                if (!m_IsVisible)
                {
                    return;
                }

                if (string.IsNullOrEmpty(value))
                {
                    m_ViewObject.UnloadGameObject();
                    return;
                }

                m_ViewObject.LoadGameObject(value);
            }
        }

        /// <summary>
        /// 视图对象父节点。
        /// </summary>
        public Transform ViewParent
        {
            get
            {
                return m_ViewObject.ParentTransform;
            }
            set
            {
                if (m_ViewObject.ParentTransform == value)
                {
                    return;
                }

                m_ViewObject.ParentTransform = value;
            }
        }

        /// <summary>
        /// GameObject实例。
        /// </summary>
        public GameObject GameObject => m_ViewObject.GameObject;

        /// <summary>
        /// Transform实例。
        /// </summary>
        public Transform Transform => m_ViewObject.Transform;

        /// <summary>
        /// 位置(基于本地坐标系).
        /// </summary>
        public Vector3 Position
        {
            get => m_ViewObject.Position;
            set => m_ViewObject.Position = value;
        }

        /// <summary>
        /// 欧拉角(基于本地坐标系).
        /// </summary>
        public Vector3 EulerAngles
        {
            get => m_ViewObject.EulerAngles;
            set => m_ViewObject.EulerAngles = value;
        }

        /// <summary>
        /// 缩放(基于本地坐标系).
        /// </summary>
        public Vector3 Scale
        {
            get => m_ViewObject.Scale;
            set => m_ViewObject.Scale = value;
        }

        /// <summary>
        /// 是否可见.
        /// </summary>
        public bool IsVisible
        {
            get => m_IsVisible;
            set
            {
                if (m_IsVisible == value)
                {
                    return;
                }

                m_IsVisible = value;

                if (value)
                {
                    if (string.IsNullOrEmpty(m_ResKey))
                    {
                        return;
                    }

                    m_ViewObject.LoadGameObject(m_ResKey);
                }
                else
                {
                    m_ViewObject.UnloadGameObject();
                }
            }
        }

        /// <summary>
        /// 视图对象是否已加载完成。
        /// </summary>
        public bool IsLoaded => m_ViewObject.IsLoaded;

        private string m_ResKey;
        private bool m_IsVisible = false;

        private IViewObject m_ViewObject;
        private ViewComponentLoadedDelegate m_OnViewLoaded;
        private ViewComponentUnloadedDelegate m_OnViewUnloaded;

        protected override void OnAwake()
        {
            m_ViewObject = Context.GetManager<IGameViewManager>().SpawnViewObject();
            m_ViewObject.OnLoaded += OnViewObjectLoaded;
            m_ViewObject.OnUnloaded += OnViewObjectUnloaded;

            m_IsVisible = true;
        }

        protected override void OnDispose()
        {
            m_ViewObject.OnLoaded -= OnViewObjectLoaded;
            m_ViewObject.OnUnloaded -= OnViewObjectUnloaded;
            m_ViewObject.Dispose();
            m_ViewObject = null;
            m_ResKey = null;
            m_IsVisible = false;
            m_OnViewLoaded = null;
            m_OnViewUnloaded = null;
        }

        private void OnViewObjectLoaded(IViewObject viewObject)
        {
            m_OnViewLoaded?.Invoke(this);
        }

        private void OnViewObjectUnloaded(IViewObject viewObject)
        {
            m_OnViewUnloaded?.Invoke(this);
        }
    }
}