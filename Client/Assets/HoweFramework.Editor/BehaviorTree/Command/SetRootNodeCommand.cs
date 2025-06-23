namespace HoweFramework.Editor
{
    /// <summary>
    /// 设置根节点命令
    /// </summary>
    public class SetRootNodeCommand : IBehaviorGraphCommand
    {
        private BehaviorGraph m_Graph;
        private string m_NewRootId;
        private string m_PreviousRootId;
        
        public string Description => "设置根节点";
        
        public SetRootNodeCommand(BehaviorGraph graph, string newRootId)
        {
            m_Graph = graph;
            m_NewRootId = newRootId;
            m_PreviousRootId = graph.RootNodeId;
        }
        
        public void Execute()
        {
            m_Graph.SetRootNode(m_NewRootId);
        }
        
        public void Undo()
        {
            m_Graph.SetRootNode(m_PreviousRootId);
        }
    }
} 