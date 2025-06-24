using System.Collections.Generic;
using System.Linq;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 删除多个节点命令
    /// </summary>
    public class RemoveMultipleNodesCommand : IBehaviorGraphCommand
    {
        private BehaviorGraph m_Graph;
        private List<BehaviorNode> m_Nodes;
        private List<BehaviorNodeConnection> m_RemovedConnections;
        
        public string Description => $"删除 {m_Nodes.Count} 个节点";
        
        public RemoveMultipleNodesCommand(BehaviorGraph graph, List<BehaviorNode> nodes)
        {
            m_Graph = graph;
            m_RemovedConnections = new List<BehaviorNodeConnection>();
            
            // 过滤掉Root节点，不允许删除
            m_Nodes = new List<BehaviorNode>(nodes.Where(n => n.NodeType != BehaviorNodeType.Root));
            
            // 如果原列表中包含Root节点，给出警告
            if (nodes.Count != m_Nodes.Count)
            {
                UnityEngine.Debug.LogWarning("已过滤掉Root节点，Root节点不允许删除");
            }
        }
        
        public void Execute()
        {
            // 记录被删除的连接
            m_RemovedConnections.Clear();
            var nodeIds = new HashSet<string>(m_Nodes.Select(n => n.Id));
            
            foreach (var node in m_Nodes)
            {
                // 记录作为父节点的连接（只记录连接到非删除节点的连接）
                foreach (var childId in node.ChildrenIds.ToList())
                {
                    if (!nodeIds.Contains(childId))
                    {
                        m_RemovedConnections.Add(new BehaviorNodeConnection(node.Id, childId));
                    }
                }
                
                // 记录作为子节点的连接（只记录来自非删除节点的连接）
                if (!string.IsNullOrEmpty(node.ParentId) && !nodeIds.Contains(node.ParentId))
                {
                    m_RemovedConnections.Add(new BehaviorNodeConnection(node.ParentId, node.Id));
                }
            }
            
            // 删除节点
            foreach (var node in m_Nodes)
            {
                m_Graph.RemoveNode(node.Id);
            }
        }
        
        public void Undo()
        {
            // 恢复节点
            foreach (var node in m_Nodes)
            {
                m_Graph.AddNode(node);
            }
            
            // 恢复连接
            foreach (var connection in m_RemovedConnections)
            {
                m_Graph.ConnectNodes(connection.ParentId, connection.ChildId);
            }
        }
    }
} 