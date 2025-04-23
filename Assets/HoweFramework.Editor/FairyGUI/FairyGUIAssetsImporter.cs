using System.Collections.Generic;
using FairyGUI.Dynamic.Editor;
using UnityEditor;

namespace HoweFramework.Editor.FairyGUI
{
    /// <summary>
    /// FairyGUI 资源导入处理。
    /// </summary>
    internal class FairyGUIAssetsImporter : AssetPostprocessor
    {
        private static readonly HashSet<FairyGUIResDirectorySetting> s_RemainCheckDirectorySet = new();

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var settings = FairyGUIEditorSettings.Instance;
            s_RemainCheckDirectorySet.Clear();
            foreach (var setting in settings.ResDirectories)
            {
                s_RemainCheckDirectorySet.Add(setting);
            }

            #region [合并所有变动的资源路径到set中]

            var assetsSet = new HashSet<string>(importedAssets);
            foreach (var asset in deletedAssets)
            {
                assetsSet.Add(asset);
            }

            foreach (var asset in movedAssets)
            {
                assetsSet.Add(asset);
            }

            foreach (var asset in movedFromAssetPaths)
            {
                assetsSet.Add(asset);
            }

            #endregion

            foreach (var asset in assetsSet)
            {
                if (!asset.EndsWith(".bytes"))
                    continue;

                foreach (var setting in s_RemainCheckDirectorySet)
                {
                    if (!asset.StartsWith(setting.DirectoryRoot))
                    {
                        continue;
                    }
                    
                    s_RemainCheckDirectorySet.Remove(setting);
                    break;
                }

                if (s_RemainCheckDirectorySet.Count == 0)
                {
                    break;
                }
            }

            foreach (var setting in settings.ResDirectories)
            {
                if (s_RemainCheckDirectorySet.Contains(setting))
                {
                    // 这里包含说明没有相关文件变动 直接跳过处理
                    continue;
                }
                
                // 生成映射文件。
                FairyGUIEditor.GeneratePackageMapping(setting);
                
                // 生成代码。
                FairyGUIEditor.GenerateCode(setting);
            }
        }
    }
}