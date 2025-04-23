using FairyGUI.Dynamic.Editor;
using UnityEditor;

namespace HoweFramework.Editor.FairyGUI
{
    /// <summary>
    /// FairyGUI 编辑器集合。
    /// </summary>
    public static class FairyGUIEditor
    {
        private const string MENU_ITEM_ROOT = "Game Framework/FairyGUI/";

        /// <summary>
        /// 生成所有目录的包映射文件。
        /// </summary>
        [MenuItem(MENU_ITEM_ROOT + "Generate Package Mapping")]
        public static void GeneratePackageMapping()
        {
            foreach (var setting in FairyGUIEditorSettings.Instance.ResDirectories)
            {
                GeneratePackageMapping(setting);
            }
        }

        /// <summary>
        /// 生成所有目录的代码。
        /// </summary>
        [MenuItem(MENU_ITEM_ROOT + "Generate Code")]
        public static void GenerateCode()
        {
            foreach (var setting in FairyGUIEditorSettings.Instance.ResDirectories)
            {
                GenerateCode(setting);
            }
        }

        /// <summary>
        /// 生成指定目录的包映射文件。
        /// </summary>
        public static void GeneratePackageMapping(FairyGUIResDirectorySetting setting)
        {
            UIPackageMappingUtility.GenerateMappingFile(setting.DirectoryRoot, setting.PackageMappingOutputPath);
        }

        /// <summary>
        /// 生成指定目录的代码
        /// </summary>
        public static void GenerateCode(FairyGUIResDirectorySetting setting)
        {
            FairyGUICodeGenerator.GenerateCode(setting.DirectoryRoot, setting.CodeGenerateSetting);
        }
    }
}