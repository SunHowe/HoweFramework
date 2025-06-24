namespace HoweFramework.Editor
{
    /// <summary>
    /// 总是失败节点模板
    /// </summary>
    public class AlwaysFailureNodeTemplate : BehaviorNodeTemplate_Decor
    {
        public override string NodeName => "Always Failure";
        public override string NodeDescription => "总是返回失败，无论子节点结果如何";
        public override string RuntimeTypeName => "HoweFramework.BehaviorAlwaysFailure";
    }
} 