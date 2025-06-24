namespace HoweFramework.Editor
{
    /// <summary>
    /// 断开节点连接命令
    /// </summary>
    public class DisconnectNodesCommand : IBehaviorGraphCommand
    {
        private BehaviorGraph m_Graph;
        private string m_ParentId;
        private string m_ChildId;
        
        public string Description => "断开节点连接";
        
        public DisconnectNodesCommand(BehaviorGraph graph, string parentId, string childId)
        {
            m_Graph = graph;
            m_ParentId = parentId;
            m_ChildId = childId;
        }
        
        public void Execute()
        {
            m_Graph.DisconnectNodes(m_ParentId, m_ChildId);
        }
        
        public void Undo()
        {
            m_Graph.ConnectNodes(m_ParentId, m_ChildId);
        }
    }
} 