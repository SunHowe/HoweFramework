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
            CameraModule.Instance.RegisterCamera(this);
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