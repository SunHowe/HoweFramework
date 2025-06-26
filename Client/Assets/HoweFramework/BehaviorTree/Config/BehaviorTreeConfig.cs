using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 行为树配置。
    /// </summary>
    public sealed class BehaviorTreeConfig : IReference, ISerializable, IDisposable
    {
        /// <summary>
        /// 根节点ID。
        /// </summary>
        public string RootNodeId;

        /// <summary>
        /// 节点列表。
        /// </summary>
        public List<BehaviorNodeConfig> Nodes { get; } = new();

        /// <summary>
        /// 节点映射。
        /// </summary>
        public Dictionary<string, BehaviorNodeConfig> NodeMap { get; } = new();

        /// <summary>
        /// 清空。
        /// </summary>
        public void Clear()
        {
            RootNodeId = default;

            foreach (var node in Nodes)
            {
                ReferencePool.Release(node);
            }

            Nodes.Clear();
            NodeMap.Clear();
        }

        /// <summary>
        /// 释放。
        /// </summary>
        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        /// <summary>
        /// 根据配置创建行为树。
        /// </summary>
        /// <returns>行为树。</returns>
        public BehaviorRoot CreateBehaviorTree()
        {
            if (!NodeMap.TryGetValue(RootNodeId, out var rootNodeConfig))
            {
                throw new ErrorCodeException(ErrorCode.BehaviorNodeConfigNotFound);
            }

            if (rootNodeConfig.CreateBehaviorNode() is not BehaviorRoot rootNode)
            {
                throw new ErrorCodeException(ErrorCode.BehaviorNodeCreateFailed);
            }

            using var checkNodeQueue = ReusableQueue<(IBehaviorNode, BehaviorNodeConfig)>.Create();
            using var nodeListBuffer = ReusableList<IBehaviorNode>.Create();

            nodeListBuffer.Add(rootNode);
            checkNodeQueue.Enqueue((rootNode, rootNodeConfig));

            var errorCode = 0;

            while (checkNodeQueue.Count > 0)
            {
                var (node, nodeConfig) = checkNodeQueue.Dequeue();
                var decorNode = node as BehaviorDecorNodeBase;
                var compositeNode = node as BehaviorCompositeNodeBase;

                if (nodeConfig.ChildrenIds.Count == 0)
                {
                    // 装饰节点和复合节点不能没有子节点。
                    if (decorNode != null || compositeNode != null)
                    {
                        errorCode = ErrorCode.BehaviorNodeInvalid;
                        break;
                    }

                    continue;
                }

                if (decorNode == null && compositeNode == null)
                {
                    errorCode = ErrorCode.BehaviorNodeInvalid;
                    break;
                }

                if (decorNode != null)
                {
                    // 装饰节点只能有一个子节点。
                    if (nodeConfig.ChildrenIds.Count > 1)
                    {
                        errorCode = ErrorCode.BehaviorNodeInvalid;
                        break;
                    }

                    var childId = nodeConfig.ChildrenIds[0];
                    if (!NodeMap.TryGetValue(childId, out var childNodeConfig))
                    {
                        errorCode = ErrorCode.BehaviorNodeConfigNotFound;
                        break;
                    }

                    var childNode = childNodeConfig.CreateBehaviorNode();
                    if (childNode == null)
                    {
                        errorCode = ErrorCode.BehaviorNodeCreateFailed;
                        break;
                    }

                    decorNode.AddChild((BehaviorNodeBase)childNode);
                    nodeListBuffer.Add(childNode);
                    checkNodeQueue.Enqueue((childNode, childNodeConfig));
                    continue;
                }
                else if (compositeNode != null)
                {
                    // 复合节点可以有多个子节点。
                    foreach (var childId in nodeConfig.ChildrenIds)
                    {
                        if (!NodeMap.TryGetValue(childId, out var childNodeConfig))
                        {
                            errorCode = ErrorCode.BehaviorNodeConfigNotFound;
                            break;
                        }

                        var childNode = childNodeConfig.CreateBehaviorNode();
                        if (childNode == null)
                        {
                            errorCode = ErrorCode.BehaviorNodeCreateFailed;
                            break;
                        }

                        compositeNode.AddChild((BehaviorNodeBase)childNode);
                        nodeListBuffer.Add(childNode);
                        checkNodeQueue.Enqueue((childNode, childNodeConfig));
                    }

                    if (errorCode != 0)
                    {
                        break;
                    }

                    continue;
                }
                else
                {
                    // 有子节点的必须是装饰节点或复合节点。
                    errorCode = ErrorCode.BehaviorNodeInvalid;
                    break;
                }
            }

            if (errorCode != 0)
            {
                foreach (var node in nodeListBuffer)
                {
                    node.Dispose();
                }

                throw new ErrorCodeException(errorCode);
            }

            return rootNode;
        }

        public static BehaviorTreeConfig Create()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
            {
                return new BehaviorTreeConfig();
            }
#endif

            return ReferencePool.Acquire<BehaviorTreeConfig>();
        }

        public void Serialize(IBufferWriter writer)
        {
            writer.WriteString(RootNodeId);
            writer.WriteObjectList(Nodes);
        }

        public void Deserialize(IBufferReader reader)
        {
            RootNodeId = reader.ReadString();
            reader.ReadObjectList(Nodes);
            UpdateNodeMap();
        }

        /// <summary>
        /// 更新节点映射。
        /// </summary>
        private void UpdateNodeMap()
        {
            NodeMap.Clear();
            foreach (var node in Nodes)
            {
                NodeMap[node.Id] = node;
            }
        }
    }
}