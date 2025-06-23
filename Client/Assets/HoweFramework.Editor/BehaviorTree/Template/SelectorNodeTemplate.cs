namespace HoweFramework.Editor
{
    /// <summary>
    /// 选择器节点模板
    /// </summary>
    public class SelectorNodeTemplate : BehaviorNodeTemplate_Composite
    {
        public override string NodeName => "Selector";
        public override string NodeDescription => "按顺序执行子节点，直到有一个成功";
        public override string RuntimeTypeName => "HoweFramework.BehaviorSelector";
    }
} 