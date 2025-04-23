using System;
using FairyGUI;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 固定尺寸界面适配器, 界面尺寸不随屏幕尺寸变化, 提供位置适配功能。
    /// </summary>
    internal sealed class FairyGUIConstantAdaptor : FairyGUIAdaptorBase
    {
        public bool IsHorizontalCenter { get; private set; }
        public bool IsVerticalCenter { get; private set; }

        protected override void OnInit()
        {
            var uiRoot = GRoot.inst;

            var xy = Vector2.zero;

            if (IsHorizontalCenter)
            {
                xy.x = (uiRoot.width - ContentPane.width) / 2;
            }

            if (IsVerticalCenter)
            {
                xy.y = (uiRoot.height - ContentPane.height) / 2;
            }

            ContentPane.SetXY(xy.x, xy.y, true);

            if (IsHorizontalCenter)
            {
                ContentPane.AddRelation(uiRoot, RelationType.Center_Center);
            }

            if (IsVerticalCenter)
            {
                ContentPane.AddRelation(uiRoot, RelationType.Middle_Middle);
            }
        }

        protected override void OnDispose()
        {
            var uiRoot = GRoot.inst;

            if (IsHorizontalCenter)
            {
                ContentPane.RemoveRelation(uiRoot, RelationType.Center_Center);
                IsHorizontalCenter = false;
            }

            if (IsVerticalCenter)
            {
                ContentPane.RemoveRelation(uiRoot, RelationType.Middle_Middle);
                IsVerticalCenter = false;
            }
        }

        public static IDisposable Create(GComponent contentPane, bool isHorizontalCenter = false, bool isVerticalCenter = false)
        {
            var adaptor = ReferencePool.Acquire<FairyGUIConstantAdaptor>();
            adaptor.IsHorizontalCenter = isHorizontalCenter;
            adaptor.IsVerticalCenter = isVerticalCenter;

            adaptor.Init(contentPane);
            return adaptor;
        }
    }
}

