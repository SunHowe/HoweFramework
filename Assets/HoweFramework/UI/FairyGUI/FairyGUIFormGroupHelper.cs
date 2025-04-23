using System.Collections.Generic;
using FairyGUI;

namespace HoweFramework
{
    /// <summary>
    /// FairyGUI界面分组辅助器。
    /// </summary>
    public sealed class FairyGUIFormGroupHelper : IUIFormGroupHelper
    {
        private readonly List<GComponent> m_UIFormGroupList = new();

        /// <summary>
        /// 创建UI界面分组实例。
        /// </summary>
        /// <param name="name">分组名称。</param>
        /// <returns>UI界面分组实例。</returns>
        public object CreateUIFormGroupInstance(string name)
        {
            var node = new GComponent();
            node.name = name;
#if UNITY_EDITOR
            node.gameObjectName = name;
#endif
            var gRoot = GRoot.inst;
            node.SetSize(gRoot.width, gRoot.height);
            gRoot.AddChild(node);

            node.Center(true);
            node.AddRelation(gRoot, RelationType.Size);

            m_UIFormGroupList.Add(node);

            return node;
        }

        public void Dispose()
        {
            foreach (var uiFormGroup in m_UIFormGroupList)
            {
                uiFormGroup.Dispose();
            }

            m_UIFormGroupList.Clear();
        }
    }
}
