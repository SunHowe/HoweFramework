using FairyGUI.Dynamic;

namespace HoweFramework
{
    /// <summary>
    /// FairyGUI设置。
    /// </summary>
    public class FairyGUISettings
    {
        /// <summary>
        /// 设计分辨率与缩放模式配置。
        /// </summary>
        public FairyGUIContentScaleFactor ContentScaleFactor { get; set; } = FairyGUIContentScaleFactor.Default;

        /// <summary>
        /// 是否立即卸载未使用的UI包。
        /// </summary>
        public bool UnloadUnusedUIPackageImmediately { get; set; }

        /// <summary>
        /// UI包路径字符串格式化。
        /// </summary>
        public string UIPackagePathFormat { get; set; } = "Assets/GameMain/UI/{0}_fui.bytes";

        /// <summary>
        /// UI资源路径字符串格式化。
        /// </summary>
        public string UIAssetPathFormat { get; set; } = "Assets/GameMain/UI/{0}_{1}{2}";
    }
}
