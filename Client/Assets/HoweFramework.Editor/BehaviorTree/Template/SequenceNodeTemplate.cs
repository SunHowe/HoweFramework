namespace HoweFramework.Editor
{
    /// <summary>
    /// 序列节点模板
    /// </summary>
    public class SequenceNodeTemplate : BehaviorNodeTemplate_Composite
    {
        public override string NodeName => "Sequence";
        public override string NodeDescription => "按顺序执行所有子节点，直到有一个失败";
        public override string RuntimeTypeName => "HoweFramework.BehaviorSequence";
    }
} 