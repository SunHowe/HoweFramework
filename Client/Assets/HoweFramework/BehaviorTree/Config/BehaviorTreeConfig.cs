using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 行为树配置。
    /// </summary>
    public sealed class BehaviorTreeConfig : IReference, ISerializable
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

            NodeMap.Clear();
            foreach (var node in Nodes)
            {
                NodeMap[node.Id] = node;
            }
        }
    }
}