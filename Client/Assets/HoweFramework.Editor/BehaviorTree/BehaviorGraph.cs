using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 行为树图数据
    /// </summary>
    [Serializable]
    public class BehaviorGraph : ScriptableObject
    {
        [SerializeField] private string m_GraphId;
        [SerializeField] private string m_GraphName;
        [SerializeField] private string m_Description;
        [SerializeField] private List<BehaviorNode> m_Nodes;
        [SerializeField] private string m_RootNodeId;
        [SerializeField] private Vector2 m_GraphOffset;
        [SerializeField] private Vector3 m_GraphScale = Vector3.one;

        /// <summary>
        /// 图ID
        /// </summary>
        public string GraphId
        {
            get => string.IsNullOrEmpty(m_GraphId) ? (m_GraphId = Guid.NewGuid().ToString()) : m_GraphId;
            set => m_GraphId = value;
        }

        /// <summary>
        /// 图名称
        /// </summary>
        public string GraphName
        {
            get => m_GraphName;
            set => m_GraphName = value;
        }

        /// <summary>
        /// 图描述
        /// </summary>
        public string Description
        {
            get => m_Description;
            set => m_Description = value;
        }

        /// <summary>
        /// 节点列表
        /// </summary>
        public List<BehaviorNode> Nodes
        {
            get => m_Nodes ??= new List<BehaviorNode>();
            set => m_Nodes = value;
        }

        /// <summary>
        /// 根节点ID
        /// </summary>
        public string RootNodeId
        {
            get => m_RootNodeId;
            set => m_RootNodeId = value;
        }

        /// <summary>
        /// 图偏移
        /// </summary>
        public Vector2 GraphOffset
        {
            get => m_GraphOffset;
            set => m_GraphOffset = value;
        }

        /// <summary>
        /// 图缩放
        /// </summary>
        public Vector3 GraphScale
        {
            get => m_GraphScale;
            set => m_GraphScale = value;
        }

        /// <summary>
        /// 根节点
        /// </summary>
        public BehaviorNode RootNode => GetNode(RootNodeId);

        /// <summary>
        /// 节点数量
        /// </summary>
        public int NodeCount => Nodes.Count;

        private void OnEnable()
        {
            if (m_Nodes == null)
                m_Nodes = new List<BehaviorNode>();
                
            // 确保有根节点
            EnsureRootNode();
        }
        
        /// <summary>
        /// 确保存在根节点
        /// </summary>
        private void EnsureRootNode()
        {
            // 如果已经有根节点，直接返回
            if (!string.IsNullOrEmpty(m_RootNodeId) && GetNode(m_RootNodeId) != null)
                return;
                
            // 查找是否已经存在Root类型的节点
            var existingRoot = Nodes.FirstOrDefault(n => n.NodeType == BehaviorNodeType.Root);
            if (existingRoot != null)
            {
                m_RootNodeId = existingRoot.Id;
                return;
            }
            
            // 创建新的根节点
            var rootTemplate = new RootNodeTemplate();
            var rootNode = rootTemplate.CreateNode();
            rootNode.GraphPosition = new Vector2(0, 0); // 根节点位于中心
            
            AddNode(rootNode);
            m_RootNodeId = rootNode.Id;
        }

        /// <summary>
        /// 获取节点
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        /// <returns>节点</returns>
        public BehaviorNode GetNode(string nodeId)
        {
            return Nodes.FirstOrDefault(n => n.Id == nodeId);
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>是否添加成功</returns>
        public bool AddNode(BehaviorNode node)
        {
            if (node == null || string.IsNullOrEmpty(node.Id))
                return false;

            if (GetNode(node.Id) != null)
                return false; // 节点已存在

            Nodes.Add(node);
            return true;
        }

        /// <summary>
        /// 移除节点
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveNode(string nodeId)
        {
            var node = GetNode(nodeId);
            if (node == null)
                return false;

            // 不允许删除根节点
            if (IsRootNode(nodeId))
                return false;

            // 移除所有父子关系
            DisconnectNode(nodeId);

            return Nodes.Remove(node);
        }
        
        /// <summary>
        /// 检查是否为根节点
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        /// <returns>是否为根节点</returns>
        public bool IsRootNode(string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId))
                return false;
                
            var node = GetNode(nodeId);
            return node != null && node.NodeType == BehaviorNodeType.Root;
        }

        /// <summary>
        /// 连接节点（父子关系）
        /// </summary>
        /// <param name="parentId">父节点ID</param>
        /// <param name="childId">子节点ID</param>
        /// <returns>是否连接成功</returns>
        public bool ConnectNodes(string parentId, string childId)
        {
            var parent = GetNode(parentId);
            var child = GetNode(childId);

            if (parent == null || child == null)
                return false;

            if (!parent.CanAddChild())
                return false;

            // Root节点不允许有父节点
            if (IsRootNode(childId))
                return false;

            if (child.ParentId == parentId)
                return true; // 已经连接

            // 如果子节点已有父节点，先断开
            if (!string.IsNullOrEmpty(child.ParentId))
            {
                DisconnectNodes(child.ParentId, childId);
            }

            // 检查是否会形成循环
            if (WouldCreateCycle(parentId, childId))
                return false;

            // 建立连接
            parent.AddChildId(childId);
            child.ParentId = parentId;

            return true;
        }

        /// <summary>
        /// 断开节点连接
        /// </summary>
        /// <param name="parentId">父节点ID</param>
        /// <param name="childId">子节点ID</param>
        /// <returns>是否断开成功</returns>
        public bool DisconnectNodes(string parentId, string childId)
        {
            var parent = GetNode(parentId);
            var child = GetNode(childId);

            if (parent == null || child == null)
                return false;

            parent.RemoveChildId(childId);
            child.ParentId = null;

            return true;
        }

        /// <summary>
        /// 断开节点的所有连接
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        public void DisconnectNode(string nodeId)
        {
            var node = GetNode(nodeId);
            if (node == null)
                return;

            // 断开与父节点的连接
            if (!string.IsNullOrEmpty(node.ParentId))
            {
                DisconnectNodes(node.ParentId, nodeId);
            }

            // 断开与所有子节点的连接
            var childrenIds = new List<string>(node.ChildrenIds);
            foreach (var childId in childrenIds)
            {
                DisconnectNodes(nodeId, childId);
            }
        }

        /// <summary>
        /// 获取节点的子节点
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        /// <returns>子节点列表</returns>
        public List<BehaviorNode> GetChildren(string nodeId)
        {
            var node = GetNode(nodeId);
            if (node == null)
                return new List<BehaviorNode>();

            return node.ChildrenIds.Select(GetNode).Where(n => n != null).ToList();
        }

        /// <summary>
        /// 获取节点的父节点
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        /// <returns>父节点</returns>
        public BehaviorNode GetParent(string nodeId)
        {
            var node = GetNode(nodeId);
            return node == null ? null : GetNode(node.ParentId);
        }

        /// <summary>
        /// 检查连接是否会形成循环
        /// </summary>
        /// <param name="parentId">父节点ID</param>
        /// <param name="childId">子节点ID</param>
        /// <returns>是否会形成循环</returns>
        private bool WouldCreateCycle(string parentId, string childId)
        {
            if (parentId == childId)
                return true;

            var visited = new HashSet<string>();
            var current = parentId;

            while (!string.IsNullOrEmpty(current) && !visited.Contains(current))
            {
                visited.Add(current);
                var node = GetNode(current);
                if (node == null)
                    break;

                current = node.ParentId;
                if (current == childId)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 设置根节点
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        /// <returns>是否设置成功</returns>
        public bool SetRootNode(string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId) || GetNode(nodeId) == null)
                return false;

            RootNodeId = nodeId;
            return true;
        }

        /// <summary>
        /// 清空图
        /// </summary>
        public void Clear()
        {
            Nodes.Clear();
            RootNodeId = null;
        }

        /// <summary>
        /// 验证图的完整性
        /// </summary>
        /// <returns>验证结果</returns>
        public BehaviorGraphValidationResult ValidateGraph()
        {
            var result = new BehaviorGraphValidationResult();

            // 检查根节点
            if (string.IsNullOrEmpty(RootNodeId))
            {
                result.AddError("行为树必须设置根节点");
            }
            else if (GetNode(RootNodeId) == null)
            {
                result.AddError("根节点不存在");
            }

            // 检查节点完整性
            foreach (var node in Nodes)
            {
                // 检查节点ID重复
                var duplicates = Nodes.Where(n => n.Id == node.Id).ToList();
                if (duplicates.Count > 1)
                {
                    result.AddError($"节点ID重复: {node.Id}");
                }

                // 检查父子关系一致性
                if (node.Id != RootNodeId)
                {
                    if (string.IsNullOrEmpty(node.ParentId))
                    {
                        // 没有父节点的节点，跳过检查。
                        continue;
                    }

                    var parent = GetNode(node.ParentId);
                    if (parent == null)
                    {
                        result.AddError($"节点 {node.Name} 的父节点不存在: {node.ParentId}");
                    }
                    else if (!parent.ChildrenIds.Contains(node.Id))
                    {
                        result.AddError($"父子关系不一致: {parent.Name} 未包含子节点 {node.Name}");
                    }
                }

                // 检查子节点关系一致性
                foreach (var childId in node.ChildrenIds)
                {
                    var child = GetNode(childId);
                    if (child == null)
                    {
                        result.AddError($"节点 {node.Name} 的子节点不存在: {childId}");
                    }
                    else if (child.ParentId != node.Id)
                    {
                        result.AddError($"父子关系不一致: {child.Name} 的父节点不是 {node.Name}");
                    }
                }

                // 检查子节点数量限制
                if (node.MaxChildrenCount > 0 && node.ChildrenIds.Count > node.MaxChildrenCount)
                {
                    result.AddError($"节点 {node.Name} 子节点数量超过限制（{node.ChildrenIds.Count}/{node.MaxChildrenCount}）");
                }

                // 检查支持子节点的节点必须至少有一个子节点
                if (node.SupportChildren && node.ChildrenIds.Count == 0)
                {
                    result.AddError($"节点 {node.Name} 支持子节点但没有连接任何子节点，这会导致行为树执行异常");
                }
            }

            // 检查循环引用
            var visited = new HashSet<string>();
            var recursionStack = new HashSet<string>();
            foreach (var node in Nodes)
            {
                if (HasCycle(node.Id, visited, recursionStack))
                {
                    result.AddError($"检测到循环引用，涉及节点: {node.Name}");
                }
            }

            return result;
        }

        /// <summary>
        /// 检查是否存在循环引用
        /// </summary>
        /// <param name="nodeId">节点ID</param>
        /// <param name="visited">已访问节点</param>
        /// <param name="recursionStack">递归栈</param>
        /// <returns>是否存在循环</returns>
        private bool HasCycle(string nodeId, HashSet<string> visited, HashSet<string> recursionStack)
        {
            if (recursionStack.Contains(nodeId))
                return true;

            if (visited.Contains(nodeId))
                return false;

            visited.Add(nodeId);
            recursionStack.Add(nodeId);

            var node = GetNode(nodeId);
            if (node != null)
            {
                foreach (var childId in node.ChildrenIds)
                {
                    if (HasCycle(childId, visited, recursionStack))
                        return true;
                }
            }

            recursionStack.Remove(nodeId);
            return false;
        }

        /// <summary>
        /// 克隆图
        /// </summary>
        /// <returns>克隆的图</returns>
        public BehaviorGraph Clone()
        {
            var clone = CreateInstance<BehaviorGraph>();
            clone.GraphId = Guid.NewGuid().ToString();
            clone.GraphName = GraphName + " (Copy)";
            clone.Description = Description;
            clone.GraphOffset = GraphOffset;
            clone.GraphScale = GraphScale;

            // 克隆节点并建立ID映射
            var idMapping = new Dictionary<string, string>();
            foreach (var node in Nodes)
            {
                var clonedNode = node.Clone();
                clone.AddNode(clonedNode);
                idMapping[node.Id] = clonedNode.Id;
            }

            // 重新建立父子关系
            for (int i = 0; i < Nodes.Count; i++)
            {
                var originalNode = Nodes[i];
                var clonedNode = clone.Nodes[i];

                // 更新父节点ID
                if (!string.IsNullOrEmpty(originalNode.ParentId) && idMapping.ContainsKey(originalNode.ParentId))
                {
                    clonedNode.ParentId = idMapping[originalNode.ParentId];
                }

                // 更新子节点ID列表
                clonedNode.ChildrenIds.Clear();
                foreach (var childId in originalNode.ChildrenIds)
                {
                    if (idMapping.ContainsKey(childId))
                    {
                        clonedNode.ChildrenIds.Add(idMapping[childId]);
                    }
                }
            }

            // 更新根节点ID
            if (!string.IsNullOrEmpty(RootNodeId) && idMapping.ContainsKey(RootNodeId))
            {
                clone.RootNodeId = idMapping[RootNodeId];
            }

            return clone;
        }
    }

    /// <summary>
    /// 行为树图验证结果
    /// </summary>
    public class BehaviorGraphValidationResult
    {
        private readonly List<string> m_Errors = new List<string>();
        private readonly List<string> m_Warnings = new List<string>();

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid => m_Errors.Count == 0;

        /// <summary>
        /// 错误列表
        /// </summary>
        public IReadOnlyList<string> Errors => m_Errors;

        /// <summary>
        /// 警告列表
        /// </summary>
        public IReadOnlyList<string> Warnings => m_Warnings;

        /// <summary>
        /// 添加错误
        /// </summary>
        /// <param name="error">错误信息</param>
        public void AddError(string error)
        {
            if (!string.IsNullOrEmpty(error))
                m_Errors.Add(error);
        }

        /// <summary>
        /// 添加警告
        /// </summary>
        /// <param name="warning">警告信息</param>
        public void AddWarning(string warning)
        {
            if (!string.IsNullOrEmpty(warning))
                m_Warnings.Add(warning);
        }
    }
}