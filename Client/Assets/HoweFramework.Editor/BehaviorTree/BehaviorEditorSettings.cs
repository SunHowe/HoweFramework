using UnityEditor;
using UnityEngine;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 行为树编辑器设置
    /// </summary>
    public static class BehaviorEditorSettings
    {
        #region EditorPrefs 键值常量
        
        /// <summary>
        /// 默认目录的EditorPrefs键
        /// </summary>
        private const string KEY_DEFAULT_DIRECTORY = "BehaviorEditor.DefaultDirectory";
        
        /// <summary>
        /// 默认文件名的EditorPrefs键
        /// </summary>
        private const string KEY_DEFAULT_FILE_NAME = "BehaviorEditor.DefaultFileName";
        
        #endregion
        
        #region 默认值常量
        
        /// <summary>
        /// 默认的目录
        /// </summary>
        private const string DEFAULT_DIRECTORY = "Assets";
        
        /// <summary>
        /// 默认的文件名
        /// </summary>
        private const string DEFAULT_FILE_NAME = "BehaviorTree";
        
        #endregion
        
        #region 公共属性
        
        /// <summary>
        /// 默认目录（用于打开、保存、另存为）
        /// </summary>
        public static string DefaultDirectory
        {
            get => EditorPrefs.GetString(KEY_DEFAULT_DIRECTORY, DEFAULT_DIRECTORY);
            set => EditorPrefs.SetString(KEY_DEFAULT_DIRECTORY, value);
        }
        
        /// <summary>
        /// 默认文件名
        /// </summary>
        public static string DefaultFileName
        {
            get => EditorPrefs.GetString(KEY_DEFAULT_FILE_NAME, DEFAULT_FILE_NAME);
            set => EditorPrefs.SetString(KEY_DEFAULT_FILE_NAME, value);
        }
        
        #endregion
        
        #region 工具方法
        
        /// <summary>
        /// 更新目录（根据用户最后选择的路径）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static void UpdateDirectory(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;
                
            var directory = System.IO.Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                DefaultDirectory = directory;
            }
        }
        
        /// <summary>
        /// 重置所有设置为默认值
        /// </summary>
        public static void ResetToDefaults()
        {
            DefaultDirectory = DEFAULT_DIRECTORY;
            DefaultFileName = DEFAULT_FILE_NAME;
        }
        
        /// <summary>
        /// 清除所有EditorPrefs设置
        /// </summary>
        public static void ClearAllSettings()
        {
            EditorPrefs.DeleteKey(KEY_DEFAULT_DIRECTORY);
            EditorPrefs.DeleteKey(KEY_DEFAULT_FILE_NAME);
        }
        
        #endregion
    }
} 