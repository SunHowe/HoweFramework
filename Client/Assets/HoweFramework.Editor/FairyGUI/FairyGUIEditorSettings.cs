using System.Collections.Generic;

namespace HoweFramework.Editor.FairyGUI
{
    /// <summary>
    /// FairyGUI 编辑器设置。
    /// </summary>
    public class FairyGUIEditorSettings
    {
        public static FairyGUIEditorSettings Instance { get; } = new();
        
        /// <summary>
        /// 资源目录设置 TODO 目前先不做编辑器 直接写死在代码里
        /// </summary>
        public readonly List<FairyGUIResDirectorySetting> ResDirectories = new()
        {
            new FairyGUIResDirectorySetting
            {
                DirectoryRoot = "Assets/GameMain/UI/",
                PackageMappingOutputPath = "Assets/GameMain/UI/MainPackageMapping.asset",
                CodeGenerateSetting = new FairyGUICodeGenerateSetting
                {
                    Namespace = "GameMain.UI",
                    ScribanTemplateDirectory = "Scriban/GameMain/",
                    UIBindingCodeDirectory = "Assets/GameMain/Scripts/UI/",
                    UILogicCodeDirectory = "Assets/GameMain/Scripts/UI/",
                }
            },
        };
    }
}