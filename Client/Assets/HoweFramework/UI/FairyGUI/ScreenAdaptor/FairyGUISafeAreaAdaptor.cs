using System;
using FairyGUI;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 安全区域屏幕界面适配器, 界面尺寸始终与安全区域尺寸相同且位置始终与安全区域位置相同。
    /// </summary>
    internal sealed class FairyGUISafeAreaAdaptor : FairyGUIAdaptorBase
    {
        protected override void OnInit()
        {
            EventModule.Instance.Subscribe(SafeAreaChangeEventArgs.EventId, OnSafeAreaChanged);

            UpdateSafeArea(SafeAreaModule.Instance.SafeArea);
        }

        protected override void OnDispose()
        {
            EventModule.Instance.Unsubscribe(SafeAreaChangeEventArgs.EventId, OnSafeAreaChanged);
        }

        private void OnSafeAreaChanged(object sender, GameEventArgs e)
        {
            var eventArgs = (SafeAreaChangeEventArgs)e;
            UpdateSafeArea(eventArgs.SafeArea);
        }

        private void UpdateSafeArea(Rect safeArea)
        {
            ContentPane.SetSize(safeArea.width, safeArea.height);
            ContentPane.SetXY(safeArea.x, safeArea.y);
        }

        public static IDisposable Create(GComponent contentPane)
        {
            var adaptor = ReferencePool.Acquire<FairyGUISafeAreaAdaptor>();
            adaptor.Init(contentPane);
            return adaptor;
        }
    }
}

