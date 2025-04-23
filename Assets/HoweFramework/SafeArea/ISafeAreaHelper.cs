using System;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 安全区域辅助接口。
    /// </summary>
    public interface ISafeAreaHelper : IDisposable
    {
        /// <summary>
        /// 安全区域范围变化事件。
        /// </summary>
        event Action<Rect> OnSafeAreaChange;
        
        /// <summary>
        /// 安全区域范围。
        /// </summary>
        Rect SafeArea { get; }

        /// <summary>
        /// 每帧更新。
        /// </summary>
        void OnUpdate();
    }
}
