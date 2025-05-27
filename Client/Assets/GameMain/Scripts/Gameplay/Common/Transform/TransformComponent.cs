using HoweFramework;
using UnityEngine;

namespace GameMain
{
    public delegate void TransformVector3UpdatedDelegate(Vector3 value);

    /// <summary>
    /// Transform组件。
    /// </summary>
    [GameComponent(GameComponentType.Transform)]
    public class TransformComponent : GameComponentBase
    {
        public event TransformVector3UpdatedDelegate OnPositionUpdated;
        public event TransformVector3UpdatedDelegate OnEulerAnglesUpdated;
        public event TransformVector3UpdatedDelegate OnScaleUpdated;

        /// <summary>
        /// 位置。
        /// </summary>
        public Vector3 Position
        {
            get => m_Position;
            set
            {
                if (m_Position == value)
                {
                    return;
                }

                m_Position = value;
                OnPositionUpdated?.Invoke(value);
            }
        }

        /// <summary>
        /// 欧拉角。
        /// </summary>
        public Vector3 EulerAngles
        {
            get => m_EulerAngles;
            set
            {
                if (m_EulerAngles == value)
                {
                    return;
                }

                m_EulerAngles = value;
                OnEulerAnglesUpdated?.Invoke(value);
            }
        }

        /// <summary>
        /// 缩放。
        /// </summary>
        public Vector3 Scale
        {
            get => m_Scale;
            set
            {
                if (m_Scale == value)
                {
                    return;
                }

                m_Scale = value;
                OnScaleUpdated?.Invoke(value);
            }
        }

        private Vector3 m_Position;
        private Vector3 m_EulerAngles;
        private Vector3 m_Scale;

        protected override void OnAwake()
        {
            m_Position = Vector3.zero;
            m_EulerAngles = Vector3.zero;
            m_Scale = Vector3.one;
        }

        protected override void OnDispose()
        {
            OnPositionUpdated = null;
            OnEulerAnglesUpdated = null;
            OnScaleUpdated = null;
            m_Position = Vector3.zero;
            m_EulerAngles = Vector3.zero;
            m_Scale = Vector3.one;
        }
    }
}
