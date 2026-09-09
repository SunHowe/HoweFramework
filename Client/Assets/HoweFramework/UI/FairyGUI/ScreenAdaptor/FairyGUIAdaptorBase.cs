using System;
using FairyGUI;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 屏幕适配器基类。
    /// </summary>
    public abstract class FairyGUIAdaptorBase : IDisposable, IReference
    {
        public GComponent ContentPane { get; private set; }

        /// <summary>
        /// 将安全区域（物理像素、左下原点、Y 向上）换算为 FairyGUI 逻辑坐标（左上原点、Y 向下）。
        /// </summary>
        /// <param name="safeArea">Screen.safeArea 安全区域。</param>
        /// <returns>FairyGUI 逻辑坐标下的安全区域。</returns>
        protected static Rect ConvertSafeAreaToUICoordinates(Rect safeArea)
        {
            var uiRoot = GRoot.inst;
            if (uiRoot == null || Screen.width <= 0 || Screen.height <= 0)
            {
                return safeArea;
            }

            var scaleX = uiRoot.width / Screen.width;
            var scaleY = uiRoot.height / Screen.height;

            return new Rect(
                safeArea.x * scaleX,
                (Screen.height - safeArea.yMax) * scaleY,
                safeArea.width * scaleX,
                safeArea.height * scaleY
            );
        }

        /// <summary>
        /// 初始化屏幕适配器。
        /// </summary>
        /// <param name="contentPane">界面内容根节点。</param>
        public void Init(GComponent contentPane)
        {
            ContentPane = contentPane;

            OnInit();
        }

        /// <summary>
        /// 释放屏幕适配器。
        /// </summary>
        public void Dispose()
        {
            OnDispose();

            ContentPane = null;
            ReferencePool.Release(this);
        }

        public void Clear()
        {
        }

        protected abstract void OnInit();
        protected abstract void OnDispose();
    }
}

