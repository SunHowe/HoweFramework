using System;
using FairyGUI;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 全屏界面适配器, 界面尺寸随屏幕尺寸变化, 提供安全区域适配功能 节点名固定为safeArea。
    /// </summary>
    internal sealed class FairyGUIFullScreenAdaptor : FairyGUIAdaptorBase
    {
        private GObject m_SafeAreaObject;

        protected override void OnInit()
        {
            var uiRoot = GRoot.inst;
            m_SafeAreaObject = ContentPane.GetChild("safeArea");

            ContentPane.size = uiRoot.size;
            ContentPane.xy = Vector2.zero;
            ContentPane.AddRelation(uiRoot, RelationType.Size);

            if (m_SafeAreaObject == null)
            {
                return;
            }

            m_SafeAreaObject.relations.ClearAll(); // 清除所有关联 由框架根据安全区域重新设置
            EventModule.Instance.Subscribe(SafeAreaChangeEventArgs.EventId, OnSafeAreaChanged);

            UpdateSafeArea(SafeAreaModule.Instance.SafeArea);
        }

        protected override void OnDispose()
        {
            ContentPane.RemoveRelation(GRoot.inst, RelationType.Size);

            if (m_SafeAreaObject != null)
            {
                EventModule.Instance.Unsubscribe(SafeAreaChangeEventArgs.EventId, OnSafeAreaChanged);
            }
        }

        private void OnSafeAreaChanged(object sender, GameEventArgs e)
        {
            var eventArgs = (SafeAreaChangeEventArgs)e;
            UpdateSafeArea(eventArgs.SafeArea);
        }

        private void UpdateSafeArea(Rect safeArea)
        {
            if (m_SafeAreaObject == null)
                return;

            m_SafeAreaObject.SetSize(safeArea.width, safeArea.height);
            m_SafeAreaObject.SetXY(safeArea.x, safeArea.y);
        }

        public static IDisposable Create(GComponent contentPane)
        {
            var adaptor = ReferencePool.Acquire<FairyGUIFullScreenAdaptor>();
            adaptor.Init(contentPane);
            return adaptor;
        }
    }
}

