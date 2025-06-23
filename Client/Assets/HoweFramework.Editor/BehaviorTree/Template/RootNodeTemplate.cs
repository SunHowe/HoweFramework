using System.Collections.Generic;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 根节点模板
    /// </summary>
    public class RootNodeTemplate : BehaviorNodeTemplate
    {
        public override BehaviorNodeType NodeType => BehaviorNodeType.Root;
        public override string NodeName => "Root";
        public override string NodeDescription => "行为树的根节点";
        public override string RuntimeTypeName => "HoweFramework.BehaviorRoot";
        public override bool SupportChildren => true;
        public override int MaxChildrenCount => 1;
    }
} 