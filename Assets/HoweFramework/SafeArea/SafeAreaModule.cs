using System;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 安全区域模块。
    /// </summary>
    public sealed class SafeAreaModule : ModuleBase<SafeAreaModule>
    {
        /// <summary>
        /// 安全区域范围。
        /// </summary>
        public Rect SafeArea => m_SafeAreaHelper.SafeArea;

        /// <summary>
        /// 安全区域辅助工具。
        /// </summary>
        private ISafeAreaHelper m_SafeAreaHelper;

        protected override void OnInit()
        {
#if UNITY_EDITOR
            m_SafeAreaHelper = new DebuggableSafeAreaHelper();
#else
            m_SafeAreaHelper = new UnitySafeAreaHelper();
#endif

            m_SafeAreaHelper.OnSafeAreaChange += OnSafeAreaChange;
        }

        private void OnSafeAreaChange(Rect rect)
        {
            // 派发安全区域变化事件.
            EventModule.Instance.Dispatch(this, SafeAreaChangeEventArgs.Create(rect));
        }

        protected override void OnDestroy()
        {
            m_SafeAreaHelper.Dispose();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            m_SafeAreaHelper.OnUpdate();
        }
    }
}
