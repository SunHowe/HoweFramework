namespace HoweFramework.Editor
{
    /// <summary>
    /// 反转节点模板
    /// </summary>
    public class InverterNodeTemplate : BehaviorNodeTemplate_Decor
    {
        public override string NodeName => "Inverter";
        public override string NodeDescription => "反转子节点的执行结果";
        public override string RuntimeTypeName => "HoweFramework.BehaviorAlwaysFailure";
    }
} 