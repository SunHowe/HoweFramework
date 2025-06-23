namespace HoweFramework.Editor
{
    /// <summary>
    /// 添加节点命令
    /// </summary>
    public class AddNodeCommand : IBehaviorGraphCommand
    {
        private BehaviorGraph m_Graph;
        private BehaviorNode m_Node;
        
        public string Description => $"添加节点 {m_Node.Name}";
        
        public AddNodeCommand(BehaviorGraph graph, BehaviorNode node)
        {
            m_Graph = graph;
            m_Node = node;
        }
        
        public void Execute()
        {
            m_Graph.AddNode(m_Node);
        }
        
        public void Undo()
        {
            m_Graph.RemoveNode(m_Node.Id);
        }
    }
} 