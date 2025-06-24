using UnityEngine;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 复制节点命令
    /// </summary>
    public class DuplicateNodeCommand : IBehaviorGraphCommand
    {
        private BehaviorGraph m_Graph;
        private BehaviorNode m_OriginalNode;
        private BehaviorNode m_DuplicatedNode;
        
        public string Description => $"复制节点 {m_OriginalNode.Name}";
        
        public DuplicateNodeCommand(BehaviorGraph graph, BehaviorNode originalNode)
        {
            m_Graph = graph;
            m_OriginalNode = originalNode;
            
            // 不允许复制Root节点
            if (m_OriginalNode.NodeType == BehaviorNodeType.Root)
            {
                throw new System.InvalidOperationException("不允许复制Root节点");
            }
            
            // 创建复制的节点
            m_DuplicatedNode = m_OriginalNode.Clone();
            m_DuplicatedNode.Id = System.Guid.NewGuid().ToString();
            m_DuplicatedNode.GraphPosition += new Vector2(50, 50); // 偏移位置
            m_DuplicatedNode.ParentId = null; // 清空父节点
            m_DuplicatedNode.ChildrenIds.Clear(); // 清空子节点
        }
        
        public void Execute()
        {
            m_Graph.AddNode(m_DuplicatedNode);
        }
        
        public void Undo()
        {
            m_Graph.RemoveNode(m_DuplicatedNode.Id);
        }
    }
} 