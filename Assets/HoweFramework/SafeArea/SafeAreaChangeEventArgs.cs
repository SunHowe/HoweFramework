using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 事件。
    /// </summary>
    public sealed class SafeAreaChangeEventArgs : GameEventArgs
    {
        /// <summary>
        /// 事件编号。
        /// </summary>
        public static readonly int EventId = typeof(SafeAreaChangeEventArgs).GetHashCode();

        /// <summary>
        /// 获取事件编号。
        /// </summary>
        public override int Id => EventId;
        
        /// <summary>
        /// 屏幕安全区域范围。
        /// </summary>
        public Rect SafeArea { get; set; }

        /// <summary>
        /// 初始化事件新实例。
        /// </summary>
        public SafeAreaChangeEventArgs()
        {
            SafeArea = Rect.zero;
        }

        /// <summary>
        /// 创建事件。
        /// </summary>
        public static SafeAreaChangeEventArgs Create(Rect safeArea)
        {
            var eventArgs = ReferencePool.Acquire<SafeAreaChangeEventArgs>();
            eventArgs.SafeArea = safeArea;
            return eventArgs;
        }

        /// <summary>
        /// 清理事件。
        /// </summary>
        public override void Clear()
        {
            SafeArea = Rect.zero;
        }
    }
}