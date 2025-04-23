using System;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 安全区域辅助工具基类。
    /// </summary>
    public abstract class SafeAreaHelperBase : ISafeAreaHelper
    {
        /// <summary>
        /// 安全区域范围变化事件。
        /// </summary>
        public event Action<Rect> OnSafeAreaChange;

        /// <summary>
        /// 安全区域范围。
        /// </summary>
        public Rect SafeArea { get; private set; }

        protected void SetSafeArea(Rect rect)
        {
            if (SafeArea == rect)
            {
                return;
            }

            SafeArea = rect;

            if (OnSafeAreaChange == null)
            {
                return;
            }

            OnSafeAreaChange.Invoke(rect);
        }

        public abstract void Dispose();

        public abstract void OnUpdate();
    }
}