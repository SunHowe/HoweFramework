using System;
using FairyGUI;

namespace HoweFramework
{
    /// <summary>
    /// 屏幕适配器基类。
    /// </summary>
    public abstract class FairyGUIAdaptorBase : IDisposable, IReference
    {
        public int ReferenceId { get; set; }

        public GComponent ContentPane { get; private set; }

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

