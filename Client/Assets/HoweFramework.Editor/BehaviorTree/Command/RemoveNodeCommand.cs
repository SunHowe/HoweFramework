using System.Collections.Generic;
using System.Linq;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 删除节点命令
    /// </summary>
    public class RemoveNodeCommand : IBehaviorGraphCommand
    {
        private BehaviorGraph m_Graph;
        private BehaviorNode m_Node;
        private List<BehaviorNodeConnection> m_RemovedConnections;
        
        public string Description => $"删除节点 {m_Node.Name}";
        
        public RemoveNodeCommand(BehaviorGraph graph, BehaviorNode node)
        {
            m_Graph = graph;
            m_Node = node;
            m_RemovedConnections = new List<BehaviorNodeConnection>();
            
            // 不允许删除Root节点
            if (m_Node.NodeType == BehaviorNodeType.Root)
            {
                throw new System.InvalidOperationException("不允许删除Root节点");
            }
        }
        
        public void Execute()
        {
            // 记录被删除的连接
            m_RemovedConnections.Clear();
            
            // 记录作为父节点的连接
            foreach (var childId in m_Node.ChildrenIds.ToList())
            {
                m_RemovedConnections.Add(new BehaviorNodeConnection(m_Node.Id, childId));
            }
            
            // 记录作为子节点的连接
            if (!string.IsNullOrEmpty(m_Node.ParentId))
            {
                m_RemovedConnections.Add(new BehaviorNodeConnection(m_Node.ParentId, m_Node.Id));
            }
            
            // 删除节点
            m_Graph.RemoveNode(m_Node.Id);
        }
        
        public void Undo()
        {
            // 恢复节点
            m_Graph.AddNode(m_Node);
            
            // 恢复连接
            foreach (var connection in m_RemovedConnections)
            {
                m_Graph.ConnectNodes(connection.ParentId, connection.ChildId);
            }
        }
    }
} 