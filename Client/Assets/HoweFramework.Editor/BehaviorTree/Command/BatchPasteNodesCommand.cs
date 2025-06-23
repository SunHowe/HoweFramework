using System.Collections.Generic;
using UnityEngine;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 批量粘贴节点命令
    /// </summary>
    public class BatchPasteNodesCommand : IBehaviorGraphCommand
    {
        private readonly BehaviorGraph m_Graph;
        private readonly List<BehaviorNode> m_NodesToAdd;
        private readonly List<ConnectionInfo> m_ConnectionsToAdd;
        private readonly List<BehaviorNode> m_AddedNodes;
        private readonly System.Action m_OnUndoCallback; // 撤销时的回调，用于清理视图

        /// <summary>
        /// 命令描述
        /// </summary>
        public string Description => $"批量粘贴 {m_NodesToAdd.Count} 个节点";

        /// <summary>
        /// 连接信息
        /// </summary>
        [System.Serializable]
        public class ConnectionInfo
        {
            public string ParentId;
            public string ChildId;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="graph">行为树图</param>
        /// <param name="nodesToAdd">要添加的节点</param>
        /// <param name="connectionsToAdd">要添加的连接</param>
        /// <param name="onUndoCallback">撤销时的回调</param>
        public BatchPasteNodesCommand(BehaviorGraph graph, List<BehaviorNode> nodesToAdd, List<ConnectionInfo> connectionsToAdd, System.Action onUndoCallback = null)
        {
            m_Graph = graph;
            m_NodesToAdd = new List<BehaviorNode>(nodesToAdd);
            m_ConnectionsToAdd = new List<ConnectionInfo>(connectionsToAdd);
            m_AddedNodes = new List<BehaviorNode>();
            m_OnUndoCallback = onUndoCallback;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public void Execute()
        {
            // 添加所有节点
            foreach (var node in m_NodesToAdd)
            {
                m_Graph.AddNode(node);
                m_AddedNodes.Add(node);
            }

            // 建立所有连接
            foreach (var connection in m_ConnectionsToAdd)
            {
                m_Graph.ConnectNodes(connection.ParentId, connection.ChildId);
            }
        }

        /// <summary>
        /// 撤销命令
        /// </summary>
        public void Undo()
        {
            // 断开所有连接
            foreach (var connection in m_ConnectionsToAdd)
            {
                m_Graph.DisconnectNodes(connection.ParentId, connection.ChildId);
            }

            // 移除所有节点
            foreach (var node in m_AddedNodes)
            {
                m_Graph.RemoveNode(node.Id);
            }

            // 调用撤销回调，清理视图
            m_OnUndoCallback?.Invoke();

            m_AddedNodes.Clear();
        }
    }
} 