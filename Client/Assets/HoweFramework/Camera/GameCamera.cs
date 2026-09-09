using System;
using UnityEngine;

namespace HoweFramework
{
    [RequireComponent(typeof(Camera))]
    public sealed class GameCamera : MonoBehaviour, IComparable<GameCamera>
    {
        public Camera Camera { get; private set; }

        public int Priority => m_Priority;

        [SerializeField]
        private int m_Priority = 0;

        private void Awake()
        {
            Camera = GetComponent<Camera>();
        }

        private void OnEnable()
        {
            // 模块销毁后（如退出播放模式时场景组件回调晚于模块销毁）Instance 为 null，需要判空。
            CameraModule.Instance?.RegisterCamera(this);
        }

        private void OnDisable()
        {
            CameraModule.Instance?.UnregisterCamera(this);
        }

        public int CompareTo(GameCamera other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            var compare = Priority.CompareTo(other.Priority);
            if (compare != 0)
            {
                return compare;
            }

            return GetInstanceID().CompareTo(other.GetInstanceID());
        }
    }
}