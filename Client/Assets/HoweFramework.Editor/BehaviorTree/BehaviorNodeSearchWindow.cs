using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 行为树节点搜索窗口
    /// </summary>
    public class BehaviorNodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        /// <summary>
        /// 图视图引用
        /// </summary>
        private BehaviorGraphView m_GraphView;

        /// <summary>
        /// 模板管理器
        /// </summary>
        private BehaviorNodeTemplateManager m_TemplateManager;

        /// <summary>
        /// 鼠标位置
        /// </summary>
        private Vector2 m_MousePosition;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="graphView">图视图</param>
        /// <param name="templateManager">模板管理器</param>
        public void Initialize(BehaviorGraphView graphView, BehaviorNodeTemplateManager templateManager)
        {
            m_GraphView = graphView;
            m_TemplateManager = templateManager;
        }

        /// <summary>
        /// 创建搜索树
        /// </summary>
        /// <param name="context">搜索上下文</param>
        /// <returns>搜索树</returns>
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("创建节点"), 0)
            };

            // 按节点类型分组添加
            AddNodesByType(tree, BehaviorNodeType.Action, "行为节点");
            AddNodesByType(tree, BehaviorNodeType.Composite, "复合节点");
            AddNodesByType(tree, BehaviorNodeType.Decor, "装饰节点");

            return tree;
        }

        /// <summary>
        /// 按类型添加节点
        /// </summary>
        /// <param name="tree">搜索树</param>
        /// <param name="nodeType">节点类型</param>
        /// <param name="typeName">类型名称</param>
        private void AddNodesByType(List<SearchTreeEntry> tree, BehaviorNodeType nodeType, string typeName)
        {
            var templates = m_TemplateManager.GetTemplatesByType(nodeType);
            if (templates.Count == 0)
                return;

            // 添加分组
            tree.Add(new SearchTreeGroupEntry(new GUIContent(typeName), 1));

            // 添加模板
            foreach (var template in templates)
            {
                var content = new GUIContent(template.NodeName, template.NodeDescription);
                tree.Add(new SearchTreeEntry(content)
                {
                    level = 2,
                    userData = template
                });
            }
        }

        /// <summary>
        /// 选择条目时的处理
        /// </summary>
        /// <param name="searchTreeEntry">选中的条目</param>
        /// <param name="context">搜索上下文</param>
        /// <returns>是否处理成功</returns>
        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            if (searchTreeEntry.userData is BehaviorNodeTemplate template)
            {
                // 创建节点（使用右键菜单打开时记录的位置）
                m_GraphView.CreateNodeFromTemplate(template);
                return true;
            }

            return false;
        }
    }
} 