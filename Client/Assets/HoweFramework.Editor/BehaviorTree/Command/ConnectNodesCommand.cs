namespace HoweFramework.Editor
{
    /// <summary>
    /// 连接节点命令
    /// </summary>
    public class ConnectNodesCommand : IBehaviorGraphCommand
    {
        private BehaviorGraph m_Graph;
        private string m_ParentId;
        private string m_ChildId;
        private string m_PreviousParentId; // 子节点之前的父节点ID
        
        public string Description => "连接节点";
        
        public ConnectNodesCommand(BehaviorGraph graph, string parentId, string childId)
        {
            m_Graph = graph;
            m_ParentId = parentId;
            m_ChildId = childId;
            
            // 记录子节点之前的父节点
            var childNode = m_Graph.GetNode(m_ChildId);
            m_PreviousParentId = childNode?.ParentId;
            
            // 不允许Root节点作为子节点
            if (childNode != null && childNode.NodeType == BehaviorNodeType.Root)
            {
                throw new System.InvalidOperationException("Root节点不允许有父节点");
            }
        }
        
        public void Execute()
        {
            m_Graph.ConnectNodes(m_ParentId, m_ChildId);
        }
        
        public void Undo()
        {
            m_Graph.DisconnectNodes(m_ParentId, m_ChildId);
            
            // 如果之前有父节点，恢复连接
            if (!string.IsNullOrEmpty(m_PreviousParentId))
            {
                m_Graph.ConnectNodes(m_PreviousParentId, m_ChildId);
            }
        }
    }
} 