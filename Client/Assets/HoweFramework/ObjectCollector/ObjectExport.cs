using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 对象导出组件。挂载该组件的节点可以被挂件收集器面板的按钮自动收集，并允许指定名字和组件类型。
    /// </summary>
    public sealed class ObjectExport : MonoBehaviour
    {
        /// <summary>
        /// 导出名字。作为收集器中存放的键值。
        /// </summary>
        public string ExportName;

        /// <summary>
        /// 是否使用导出类型名作为后缀。若开启则不支持手动设置名字，直接使用GameObject的名字加类型名后缀。
        /// </summary>
        public bool ExportWithTypeNameSuffix;
        
        /// <summary>
        /// 导出类型名。供收集器收集指定类型组件使用。
        /// </summary>
        public string ExportTypeName;
    }
}