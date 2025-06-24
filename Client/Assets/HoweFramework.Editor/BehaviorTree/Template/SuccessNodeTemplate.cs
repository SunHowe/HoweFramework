namespace HoweFramework.Editor
{
    /// <summary>
    /// 成功节点模板
    /// </summary>
    public class SuccessNodeTemplate : BehaviorNodeTemplate_Action
    {
        public override string NodeName => "Success";
        public override string NodeDescription => "总是返回成功";
        public override string RuntimeTypeName => "HoweFramework.BehaviorSuccess";
    }
} 