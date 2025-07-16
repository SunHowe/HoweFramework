using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HoweFramework;
using UnityEngine;

namespace GameMain
{
    public sealed partial class GameViewManager
    {
        /// <summary>
        /// 视图对象.
        /// </summary>
        private sealed class ViewObject : IReference, IViewObject
        {
            /// <summary>
            /// 视图对象加载完成事件。
            /// </summary>
            public event ViewObjectLoadedDelegate OnLoaded
            {
                add
                {
                    m_OnLoaded += value;
                    if (IsLoaded)
                    {
                        value(this);
                    }
                }
                remove
                {
                    m_OnLoaded -= value;
                }
            }

            /// <summary>
            /// 视图对象卸载完成事件。
            /// </summary>
            public event ViewObjectUnloadedDelegate OnUnloaded
            {
                add
                {
                    m_OnUnloaded += value;
                }
                remove
                {
                    m_OnUnloaded -= value;
                }
            }

            /// <summary>
            /// 实例id。
            /// </summary>
            public int Id { get; private set; }

            /// <summary>
            /// 资源键值.
            /// </summary>
            public string ResKey { get; private set; }

            /// <summary>
            /// GameObject实例.
            /// </summary>
            public GameObject GameObject { get; private set; }

            /// <summary>
            /// Transform实例.
            /// </summary>
            public Transform Transform { get; private set; }

            /// <summary>
            /// 获取/设置父节点。
            /// </summary>
            public Transform ParentTransform
            {
                get => m_ParentTransform;
                set
                {
                    if (m_ParentTransform == value)
                    {
                        return;
                    }

                    m_ParentTransform = value;
                    if (Transform != null)
                    {
                        Transform.SetParent(value);
                    }
                }
            }

            /// <summary>
            /// 获取/设置位置(基于本地坐标系)
            /// </summary>
            public Vector3 Position
            {
                get => m_LocalPosition;
                set
                {
                    m_LocalPosition = value;
                    if (Transform != null)
                    {
                        Transform.localPosition = value;
                    }
                }
            }

            /// <summary>
            /// 获取/设置欧拉角(基于本地坐标系)
            /// </summary>
            public Vector3 EulerAngles
            {
                get => m_LocalEulerAngles;
                set
                {
                    m_LocalEulerAngles = value;
                    if (Transform != null)
                    {
                        Transform.localEulerAngles = value;
                    }
                }
            }

            /// <summary>
            /// 获取/设置缩放值(基于本地坐标系)
            /// </summary>
            public Vector3 Scale
            {
                get => m_LocalScale;
                set
                {
                    m_LocalScale = value;
                    if (Transform != null)
                    {
                        Transform.localScale = value;
                    }
                }
            }

            /// <summary>
            /// 获取/设置视图对象是否可见.
            /// </summary>
            public bool IsVisible
            {
                get => m_IsVisible;
                set
                {
                    m_IsVisible = value;
                    if (GameObject != null)
                    {
                        GameObject.SetActive(value);
                    }
                }
            }

            /// <summary>
            /// 是否已完成加载。
            /// </summary>
            public bool IsLoaded { get; private set; }

            private Transform m_ParentTransform;
            private Vector3 m_LocalPosition = Vector3.zero;
            private Vector3 m_LocalEulerAngles = Vector3.zero;
            private Vector3 m_LocalScale = Vector3.one;
            private bool m_IsVisible = true;

            private CancellationTokenSource m_CancellationTokenSource;
            private IGameViewManager m_GameViewManager;

            private ViewObjectLoadedDelegate m_OnLoaded;
            private ViewObjectUnloadedDelegate m_OnUnloaded;

            /// <summary>
            /// 加载视图对象.
            /// </summary>
            public void LoadGameObject(string resKey)
            {
                LoadGameObjectAsync(resKey).Forget();
            }

            /// <summary>
            /// 加载视图对象.
            /// </summary>
            private async UniTask LoadGameObjectAsync(string resKey)
            {
                if (resKey == ResKey)
                {
                    // 资源键值相同，不重复加载。
                    if (string.IsNullOrEmpty(resKey))
                    {
                        // 要防止直接设置GameObject后，再调用LoadGameObject传入空字符串，导致旧对象未卸载的问题。
                        UnloadGameObject();
                    }
                    
                    return;
                }

                if (string.IsNullOrEmpty(resKey))
                {
                    // 资源键值为空，卸载视图对象。
                    UnloadGameObject();
                    return;
                }

                var cancellationToken = (m_CancellationTokenSource ??= new CancellationTokenSource()).Token;

                var gameObject = await m_GameViewManager.Context.GameObjectPool.InstantiateAsync(resKey);
                if (cancellationToken.IsCancellationRequested)
                {
                    // 取消加载任务。
                    if (gameObject != null)
                    {
                        // 归还实例。
                        gameObject.Destroy();
                    }
                    return;
                }

                // 卸载旧的视图对象。
                UnloadGameObject();

                // 设置资源键值。
                ResKey = resKey;

                if (gameObject == null)
                {
                    // 加载失败。
                    return;
                }

                IsLoaded = true;
                GameObject = gameObject;
                Transform = gameObject.transform;
                if (m_ParentTransform != null)
                {
                    Transform.SetParent(m_ParentTransform);
                }

                Transform.localPosition = m_LocalPosition;
                Transform.localEulerAngles = m_LocalEulerAngles;
                Transform.localScale = m_LocalScale;

                gameObject.SetActive(m_IsVisible);

                m_OnLoaded?.Invoke(this);
            }

            /// <summary>
            /// 设置GameObject.
            /// </summary>
            /// <param name="gameObject">GameObject。</param>
            public void SetGameObject(GameObject gameObject)
            {
                // 卸载旧的视图对象。
                UnloadGameObject();

                if (gameObject == null)
                {
                    return;
                }

                IsLoaded = true;
                GameObject = gameObject;
                Transform = gameObject.transform;
                if (m_ParentTransform != null)
                {
                    Transform.SetParent(m_ParentTransform);
                }

                Transform.localPosition = m_LocalPosition;
                Transform.localEulerAngles = m_LocalEulerAngles;
                Transform.localScale = m_LocalScale;

                gameObject.SetActive(m_IsVisible);

                m_OnLoaded?.Invoke(this);
            }

            /// <summary>
            /// 卸载资源对象.
            /// </summary>
            public void UnloadGameObject()
            {
                if (!IsLoaded && string.IsNullOrEmpty(ResKey))
                {
                    return;
                }

                IsLoaded = false;

                if (GameObject != null)
                {
                    m_OnUnloaded?.Invoke(this);

                    GameObject = null;
                    Transform = null;
                    GameObject.Destroy();
                }
                else if (m_CancellationTokenSource != null)
                {
                    m_CancellationTokenSource.Cancel();
                    m_CancellationTokenSource.Dispose();
                    m_CancellationTokenSource = null;
                }

                ResKey = null;
            }

            public void Clear()
            {
                // Do nothing.
            }

            public void Dispose()
            {
                if (m_GameViewManager == null)
                {
                    return;
                }

                m_GameViewManager.DisposeViewObject(this);
            }

            public void DisposeFromManager()
            {
                UnloadGameObject();

                if (m_CancellationTokenSource != null)
                {
                    m_CancellationTokenSource.Cancel();
                    m_CancellationTokenSource.Dispose();
                    m_CancellationTokenSource = null;
                }

                m_LocalPosition = Vector3.zero;
                m_LocalEulerAngles = Vector3.zero;
                m_LocalScale = Vector3.one;
                m_IsVisible = true;
                m_GameViewManager = null;
                Id = 0;
                ResKey = null;
                IsLoaded = false;
                m_ParentTransform = null;
                m_OnLoaded = null;
                m_OnUnloaded = null;

                ReferencePool.Release(this);
            }

            public static ViewObject Create(IGameViewManager gameViewManager, int id)
            {
                var viewObject = ReferencePool.Acquire<ViewObject>();
                viewObject.m_GameViewManager = gameViewManager;
                viewObject.Id = id;
                return viewObject;
            }
        }
    }
}