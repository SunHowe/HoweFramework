using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 行为树节点配置。
    /// </summary>
    public sealed class BehaviorNodeConfig : IReference, ISerializable
    {
        /// <summary>
        /// 节点ID。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 运行时类型名。
        /// </summary>
        public string RuntimeTypeName { get; set; }

        /// <summary>
        /// 属性列表。
        /// </summary>
        public List<BehaviorPropertyConfig> Properties { get; } = new();

        /// <summary>
        /// 子节点ID列表。
        /// </summary>
        public List<string> ChildrenIds { get; } = new();

        /// <summary>
        /// 清空。
        /// </summary>
        public void Clear()
        {
            Id = default;
            RuntimeTypeName = default;
            foreach (var property in Properties)
            {
                ReferencePool.Release(property);
            }
            Properties.Clear();
            ChildrenIds.Clear();
        }

        /// <summary>
        /// 根据配置创建行为节点。
        /// </summary>
        /// <returns>行为节点。</returns>
        public IBehaviorNode CreateBehaviorNode()
        {
            var nodeType = AssemblyUtility.GetRuntimeType(RuntimeTypeName);
            if (nodeType == null)
            {
                return null;
            }

            if (ReferencePool.Acquire(nodeType) is not BehaviorNodeBase node)
            {
                return null;
            }

            foreach (var property in Properties)
            {
                node.SetProperty(property);
            }

            return node;
        }

        public static BehaviorNodeConfig Create()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
            {
                return new BehaviorNodeConfig();
            }
#endif

            return ReferencePool.Acquire<BehaviorNodeConfig>();
        }

        public void Serialize(IBufferWriter writer)
        {
            writer.WriteString(Id);
            writer.WriteString(RuntimeTypeName);
            writer.WriteObjectList(Properties);
            writer.WriteStringList(ChildrenIds);
        }

        public void Deserialize(IBufferReader reader)
        {
            Id = reader.ReadString();
            RuntimeTypeName = reader.ReadString();
            reader.ReadObjectList(Properties);
            reader.ReadStringList(ChildrenIds);
        }
    }
}