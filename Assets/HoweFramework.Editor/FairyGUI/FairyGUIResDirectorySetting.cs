using System;
using UnityEngine.Serialization;

namespace HoweFramework.Editor.FairyGUI
{
    /// <summary>
    /// FairyGUI资源目录设置。
    /// </summary>
    [Serializable]
    public class FairyGUIResDirectorySetting
    {
        /// <summary>
        /// 资源目录。
        /// </summary>
        public string DirectoryRoot;

        /// <summary>
        /// FairyGUI包映射文件输出路径。
        /// </summary>
        public string PackageMappingOutputPath;

        /// <summary>
        /// 代码生成配置。
        /// </summary>
        public FairyGUICodeGenerateSetting CodeGenerateSetting;
    }
}