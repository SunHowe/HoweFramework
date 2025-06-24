using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 行为树节点模板管理器
    /// </summary>
    public class BehaviorNodeTemplateManager
    {
        /// <summary>
        /// 所有节点模板
        /// </summary>
        private List<BehaviorNodeTemplate> m_Templates = new List<BehaviorNodeTemplate>();

        /// <summary>
        /// 按类型分组的模板
        /// </summary>
        private Dictionary<BehaviorNodeType, List<BehaviorNodeTemplate>> m_TemplatesByType = new Dictionary<BehaviorNodeType, List<BehaviorNodeTemplate>>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BehaviorNodeTemplateManager()
        {
            LoadTemplates();
        }

        /// <summary>
        /// 加载模板
        /// </summary>
        private void LoadTemplates()
        {
            // 从程序集中查找所有节点模板类型
            var templateTypes = GetTemplateTypes();
            
            foreach (var type in templateTypes)
            {
                try
                {
                    var template = Activator.CreateInstance(type) as BehaviorNodeTemplate;
                    if (template != null)
                    {
                        // 过滤掉Root节点模板，不允许用户手动创建
                        if (template.NodeType != BehaviorNodeType.Root)
                        {
                            m_Templates.Add(template);
                        }
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError($"Failed to create template instance for type {type.Name}: {e.Message}");
                }
            }

            // 按类型分组
            GroupTemplatesByType();
        }

        /// <summary>
        /// 获取所有模板类型
        /// </summary>
        /// <returns>模板类型列表</returns>
        private IEnumerable<Type> GetTemplateTypes()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var templateTypes = new List<Type>();
            
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes()
                        .Where(t => typeof(BehaviorNodeTemplate).IsAssignableFrom(t) && 
                                   !t.IsAbstract && 
                                   t.IsClass)
                        .ToArray();
                    
                    templateTypes.AddRange(types);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogWarning($"Failed to load types from assembly {assembly.FullName}: {e.Message}");
                }
            }

            return templateTypes;
        }

        /// <summary>
        /// 按类型分组模板
        /// </summary>
        private void GroupTemplatesByType()
        {
            m_TemplatesByType.Clear();
            
            foreach (var template in m_Templates)
            {
                if (!m_TemplatesByType.ContainsKey(template.NodeType))
                {
                    m_TemplatesByType[template.NodeType] = new List<BehaviorNodeTemplate>();
                }
                
                m_TemplatesByType[template.NodeType].Add(template);
            }
        }

        /// <summary>
        /// 获取所有模板
        /// </summary>
        /// <returns>模板列表</returns>
        public IReadOnlyList<BehaviorNodeTemplate> GetAllTemplates()
        {
            return m_Templates.AsReadOnly();
        }

        /// <summary>
        /// 按类型获取模板
        /// </summary>
        /// <param name="nodeType">节点类型</param>
        /// <returns>模板列表</returns>
        public IReadOnlyList<BehaviorNodeTemplate> GetTemplatesByType(BehaviorNodeType nodeType)
        {
            if (m_TemplatesByType.TryGetValue(nodeType, out var templates))
            {
                return templates.AsReadOnly();
            }
            
            return new List<BehaviorNodeTemplate>().AsReadOnly();
        }

        /// <summary>
        /// 搜索模板
        /// </summary>
        /// <param name="searchText">搜索文本</param>
        /// <returns>匹配的模板列表</returns>
        public IReadOnlyList<BehaviorNodeTemplate> SearchTemplates(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return GetAllTemplates();

            var searchLower = searchText.ToLower();
            
            return m_Templates
                .Where(t => t.NodeName.ToLower().Contains(searchLower) ||
                           t.NodeDescription.ToLower().Contains(searchLower) ||
                           t.RuntimeTypeName.ToLower().Contains(searchLower))
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// 基于RuntimeTypeName获取模板实例
        /// </summary>
        /// <param name="typeName">运行时类型名</param>
        /// <returns>模板实例或null</returns>
        public BehaviorNodeTemplate GetTemplateByRuntimeTypeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return null;
            return m_Templates.FirstOrDefault(t => t.RuntimeTypeName == typeName);
        }
    }
} 