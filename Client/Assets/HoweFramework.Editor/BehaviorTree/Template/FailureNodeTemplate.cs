namespace HoweFramework.Editor
{
    /// <summary>
    /// 失败节点模板
    /// </summary>
    public class FailureNodeTemplate : BehaviorNodeTemplate_Action
    {
        public override string NodeName => "Failure";
        public override string NodeDescription => "总是返回失败";
        public override string RuntimeTypeName => "HoweFramework.BehaviorFailure";
    }
} 