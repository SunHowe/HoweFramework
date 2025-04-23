using System;

namespace HoweFramework.Editor.FairyGUI
{
    /// <summary>
    /// FairyGUI代码生成配置。
    /// </summary>
    [Serializable]
    public class FairyGUICodeGenerateSetting
    {
        /// <summary>
        /// Scriban模板文件目录。
        /// </summary>
        public string ScribanTemplateDirectory;

        /// <summary>
        /// UI绑定代码生成目录。
        /// </summary>
        public string UIBindingCodeDirectory;

        /// <summary>
        /// UI逻辑代码生成目录。
        /// </summary>
        public string UILogicCodeDirectory;

        /// <summary>
        /// 命名空间。
        /// </summary>
        public string Namespace;
    }
}