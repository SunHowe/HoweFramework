using FairyGUI;

namespace HoweFramework
{
    /// <summary>
    /// FairyGUI设计分辨率与缩放模式配置。
    /// </summary>
    public struct FairyGUIContentScaleFactor
    {
        public static readonly FairyGUIContentScaleFactor Default = new FairyGUIContentScaleFactor()
        {
            DesignResolutionX = 1280,
            DesignResolutionY = 720,
            ScreenMatchMode = UIContentScaler.ScreenMatchMode.MatchWidthOrHeight,
        };

        /// <summary>
        /// 设计分辨率X。
        /// </summary>
        public int DesignResolutionX { get; set; }

        /// <summary>
        /// 设计分辨率Y。
        /// </summary>
        public int DesignResolutionY { get; set; }

        /// <summary>
        /// 缩放模式。
        /// </summary>
        public UIContentScaler.ScreenMatchMode ScreenMatchMode { get; set; }
    }
}
