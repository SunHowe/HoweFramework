#if UNITY_EDITOR
using System;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 支持调试的安全区域辅助工具。用于Editor环境使用。
    /// </summary>
    internal sealed class DebuggableSafeAreaHelper : SafeAreaHelperBase
    {
        private readonly DebuggableSafeAreaComponent m_DebuggableSafeAreaComponent;

        public DebuggableSafeAreaHelper()
        {
            var gameObject = new GameObject("Safe Area Debugger");
            m_DebuggableSafeAreaComponent = gameObject.AddComponent<DebuggableSafeAreaComponent>();
            m_DebuggableSafeAreaComponent.OnSafeAreaChanged += SetSafeArea;
        }

        public override void Dispose()
        {
            if (m_DebuggableSafeAreaComponent == null)
            {
                return;
            }
            
            UnityEngine.Object.Destroy(m_DebuggableSafeAreaComponent.gameObject);
        }

        public override void OnUpdate()
        {
        }

        private sealed class DebuggableSafeAreaComponent : MonoBehaviour
        {
            public Action<Rect> OnSafeAreaChanged;

            [SerializeField]
            private int m_OffsetTop;

            [SerializeField]
            private int m_OffsetBottom;

            [SerializeField]
            private int m_OffsetLeft;

            [SerializeField]
            private int m_OffsetRight;

            private int m_LastOffsetTop;
            private int m_LastOffsetBottom;
            private int m_LastOffsetLeft;
            private int m_LastOffsetRight;

            private int m_LastWidth;
            private int m_LastHeight;

            private void Awake()
            {
                m_LastWidth = Screen.width;
                m_LastHeight = Screen.height;

                DontDestroyOnLoad(gameObject);
            }

            private void Update()
            {
                var modify1 = UpdateInputData();
                var modify2 = UpdateScreenSize();

                if (!modify1 && !modify2)
                {
                    return;
                }

                OnSafeAreaChanged?.Invoke(GetSafeArea());
            }

            private bool UpdateInputData()
            {
                if (m_OffsetTop == m_LastOffsetTop &&
                    m_OffsetBottom == m_LastOffsetBottom &&
                    m_OffsetLeft == m_LastOffsetLeft &&
                    m_OffsetRight == m_LastOffsetRight)
                    return false;

                m_LastOffsetTop = m_OffsetTop = Math.Max(0, m_OffsetTop);
                m_LastOffsetBottom = m_OffsetBottom = Math.Max(0, m_OffsetBottom);
                m_LastOffsetLeft = m_OffsetLeft = Math.Max(0, m_OffsetLeft);
                m_LastOffsetRight = m_OffsetRight = Math.Max(0, m_OffsetRight);
                return true;
            }

            public Rect GetSafeArea()
            {
                var x = m_OffsetLeft;
                var y = m_OffsetTop;
                var w = m_LastWidth - m_OffsetRight - x;
                var h = m_LastHeight - m_OffsetBottom - y;
                return new Rect(x, y, w, h);
            }

            private bool UpdateScreenSize()
            {
                var width = Screen.width;
                var height = Screen.height;

                if (width == m_LastWidth && height == m_LastHeight)
                    return false;

                m_LastWidth = width;
                m_LastHeight = height;
                return true;
            }
        }
    }
}

#endif