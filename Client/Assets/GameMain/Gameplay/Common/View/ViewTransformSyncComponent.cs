using HoweFramework;
using UnityEngine;

namespace GameMain
{
    /// <summary>
    /// 视图Transform同步组件。
    /// </summary>
    [GameComponent(GameComponentType.ViewTransformSync)]
    public sealed class ViewTransformSyncComponent : GameComponentBase
    {
        private TransformComponent m_TransformComponent;
        private ViewComponent m_ViewComponent;

        /// <summary>
        /// 是否暂停同步。
        /// </summary>
        public bool IsPause
        {
            get => m_IsPause;
            set
            {
                if (m_IsPause == value)
                {
                    return;
                }

                m_IsPause = value;

                if (value)
                {
                    if (m_IsOnLerpPosition)
                    {
                        Context.UnregisterLateUpdate(this, OnUpdateLerpPosition);
                    }

                    if (m_IsOnLerpEulerAngles)
                    {
                        Context.UnregisterLateUpdate(this, OnUpdateLerpEulerAngles);
                    }

                    if (m_IsOnLerpScale)
                    {
                        Context.UnregisterLateUpdate(this, OnUpdateLerpScale);
                    }
                }
                else
                {
                    if (m_IsOnLerpPosition)
                    {
                        // 重新计算位置。
                        OnTransformPositionUpdated(m_TransformComponent.Position);
                    }

                    if (m_IsOnLerpEulerAngles)
                    {
                        // 重新计算欧拉角。
                        OnTransformEulerAnglesUpdated(m_TransformComponent.EulerAngles);
                    }

                    if (m_IsOnLerpScale)
                    {
                        // 重新计算缩放。
                        OnTransformScaleUpdated(m_TransformComponent.Scale);
                    }
                }
            }
        }

        /// <summary>
        /// 是否开启位置插值。
        /// </summary>
        public bool IsLerpPosition
        {
            get => m_IsLerpPosition;
            set
            {
                if (m_IsLerpPosition == value)
                {
                    return;
                }

                m_IsLerpPosition = value;

                if (m_IsPause)
                {
                    return;
                }
                
                if (!m_IsOnLerpPosition)
                {
                    return;
                }

                // 如果正在插值，则取消插值更新，并立即同步位置。
                Context.UnregisterLateUpdate(this, OnUpdateLerpPosition);
                m_ViewComponent.Position = m_TransformComponent.Position;
                m_IsOnLerpPosition = false;
            }
        }

        /// <summary>
        /// 是否开启欧拉角插值。
        /// </summary>
        public bool IsLerpEulerAngles
        {
            get => m_IsLerpEulerAngles;
            set
            {
                if (m_IsLerpEulerAngles == value)
                {
                    return;
                }

                m_IsLerpEulerAngles = value;

                if (m_IsPause)
                {
                    return;
                }

                if (!m_IsOnLerpEulerAngles)
                {
                    return;
                }

                // 如果正在插值，则取消插值更新，并立即同步欧拉角。
                Context.UnregisterLateUpdate(this, OnUpdateLerpEulerAngles);
                m_ViewComponent.EulerAngles = m_TransformComponent.EulerAngles;
                m_IsOnLerpEulerAngles = false;
            }
        }

        /// <summary>
        /// 是否开启缩放插值。
        /// </summary>
        public bool IsLerpScale
        {
            get => m_IsLerpScale;
            set
            {
                if (m_IsLerpScale == value)
                {
                    return;
                }

                m_IsLerpScale = value;

                if (m_IsPause)
                {
                    return;
                }

                if (!m_IsOnLerpScale)
                {
                    return;
                }

                // 如果正在插值，则取消插值更新，并立即同步缩放。
                Context.UnregisterLateUpdate(this, OnUpdateLerpScale);
                m_ViewComponent.Scale = m_TransformComponent.Scale;
                m_IsOnLerpScale = false;
            }
        }

        private bool m_IsPause;
        private bool m_IsLerpPosition;
        private bool m_IsLerpEulerAngles;
        private bool m_IsLerpScale;

        private float m_LerpPositionTime;
        private float m_LerpEulerAnglesTime;
        private float m_LerpScaleTime;

        private float m_LerpDuration;
        private int m_FrameRate;

        private bool m_IsOnLerpPosition;
        private bool m_IsOnLerpEulerAngles;
        private bool m_IsOnLerpScale;

        /// <summary>
        /// 强制同步。
        /// </summary>
        public void ForceSync()
        {
            m_ViewComponent.Position = m_TransformComponent.Position;
            m_ViewComponent.EulerAngles = m_TransformComponent.EulerAngles;
            m_ViewComponent.Scale = m_TransformComponent.Scale;

            m_IsOnLerpPosition = false;
            m_IsOnLerpEulerAngles = false;
            m_IsOnLerpScale = false;
        }

        protected override void OnAwake()
        {
            m_LerpDuration = Context.GameFixedDeltaTime;
            m_FrameRate = Context.GameFrameRate;

            m_TransformComponent = Entity.GetComponent<TransformComponent>();
            m_ViewComponent = Entity.GetComponent<ViewComponent>();

            m_TransformComponent.OnPositionUpdated += OnTransformPositionUpdated;
            m_TransformComponent.OnEulerAnglesUpdated += OnTransformEulerAnglesUpdated;
            m_TransformComponent.OnScaleUpdated += OnTransformScaleUpdated;

            // 初始化位置。
            m_ViewComponent.Position = m_TransformComponent.Position;
            // 初始化欧拉角。
            m_ViewComponent.EulerAngles = m_TransformComponent.EulerAngles;
            // 初始化缩放。
            m_ViewComponent.Scale = m_TransformComponent.Scale;
        }

        protected override void OnDispose()
        {
            Context.UnregisterLateUpdate(this, OnUpdateLerpPosition);
            Context.UnregisterLateUpdate(this, OnUpdateLerpEulerAngles);
            Context.UnregisterLateUpdate(this, OnUpdateLerpScale);

            m_TransformComponent.OnPositionUpdated -= OnTransformPositionUpdated;
            m_TransformComponent.OnEulerAnglesUpdated -= OnTransformEulerAnglesUpdated;
            m_TransformComponent.OnScaleUpdated -= OnTransformScaleUpdated;

            m_TransformComponent = null;
            m_ViewComponent = null;

            m_IsPause = false;

            m_IsLerpPosition = false;
            m_IsLerpEulerAngles = false;
            m_IsLerpScale = false;

            m_IsOnLerpPosition = false;
            m_IsOnLerpEulerAngles = false;
            m_IsOnLerpScale = false;

            m_LerpPositionTime = 0f;
            m_LerpEulerAnglesTime = 0f;
            m_LerpScaleTime = 0f;

            m_LerpDuration = 0f;
            m_FrameRate = 0;
        }

        private void OnTransformPositionUpdated(Vector3 position)
        {
            if (m_IsPause)
            {
                // 暂停同步。
                return;
            }

            if (!m_IsLerpPosition)
            {
                // 直接赋值。
                m_ViewComponent.Position = position;
                return;
            }

            // 清零插值时间。   
            m_LerpPositionTime = 0;
            
            if (m_IsOnLerpPosition)
            {
                return;
            }

            m_IsOnLerpPosition = true;
            Context.RegisterLateUpdate(this, OnUpdateLerpPosition);
        }

        private void OnTransformEulerAnglesUpdated(Vector3 eulerAngles)
        {
            if (m_IsPause)
            {
                // 暂停同步。
                return;
            }

            if (!m_IsLerpEulerAngles)
            {
                // 直接赋值。
                m_ViewComponent.EulerAngles = eulerAngles;
                return;
            }

            // 清零插值时间。   
            m_LerpEulerAnglesTime = 0;
            
            if (m_IsOnLerpEulerAngles)
            {
                return;
            }

            m_IsOnLerpEulerAngles = true;
            Context.RegisterLateUpdate(this, OnUpdateLerpEulerAngles);
        }

        private void OnTransformScaleUpdated(Vector3 scale)
        {
            if (m_IsPause)
            {
                // 暂停同步。
                return;
            }

            if (!m_IsLerpScale)
            {
                // 直接赋值。
                m_ViewComponent.Scale = scale;
                return;
            }

            // 清零插值时间。   
            m_LerpScaleTime = 0;
            
            if (m_IsOnLerpScale)
            {
                return;
            }

            m_IsOnLerpScale = true;
            Context.RegisterLateUpdate(this, OnUpdateLerpScale);
        }

        private void OnUpdateLerpPosition(float elapseSeconds)
        {
            m_LerpPositionTime += elapseSeconds;
            if (m_LerpPositionTime >= m_LerpDuration)
            {
                // 插值结束。
                m_IsOnLerpPosition = false;
                m_ViewComponent.Position = m_TransformComponent.Position;
                Context.UnregisterLateUpdate(this, OnUpdateLerpPosition);
                return;
            }
            
            m_ViewComponent.Position = Vector3.Lerp(m_ViewComponent.Position, m_TransformComponent.Position, m_LerpPositionTime * m_FrameRate);
        }

        private void OnUpdateLerpEulerAngles(float elapseSeconds)
        {
            m_LerpEulerAnglesTime += elapseSeconds;
            if (m_LerpEulerAnglesTime >= m_LerpDuration)
            {
                // 插值结束。
                m_IsOnLerpEulerAngles = false;
                m_ViewComponent.EulerAngles = m_TransformComponent.EulerAngles;
                Context.UnregisterLateUpdate(this, OnUpdateLerpEulerAngles);
                return;
            }

            m_ViewComponent.EulerAngles = Vector3.Lerp(m_ViewComponent.EulerAngles, m_TransformComponent.EulerAngles, m_LerpEulerAnglesTime * m_FrameRate);
        }

        private void OnUpdateLerpScale(float elapseSeconds)
        {
            m_LerpScaleTime += elapseSeconds;
            if (m_LerpScaleTime >= m_LerpDuration)
            {
                // 插值结束。
                m_IsOnLerpScale = false;
                m_ViewComponent.Scale = m_TransformComponent.Scale;
                Context.UnregisterLateUpdate(this, OnUpdateLerpScale);
                return;
            }

            m_ViewComponent.Scale = Vector3.Lerp(m_ViewComponent.Scale, m_TransformComponent.Scale, m_LerpScaleTime * m_FrameRate);
        }
    }
}
