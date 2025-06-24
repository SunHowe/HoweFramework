namespace HoweFramework.Editor
{
    /// <summary>
    /// 节点连接信息
    /// </summary>
    public struct BehaviorNodeConnection
    {
        public string ParentId;
        public string ChildId;
        
        public BehaviorNodeConnection(string parentId, string childId)
        {
            ParentId = parentId;
            ChildId = childId;
        }
    }
} 