namespace HoweFramework.Editor
{
    /// <summary>
    /// 总是成功节点模板
    /// </summary>
    public class AlwaysSuccessNodeTemplate : BehaviorNodeTemplate_Decor
    {
        public override string NodeName => "Always Success";
        public override string NodeDescription => "总是返回成功，无论子节点结果如何";
        public override string RuntimeTypeName => "HoweFramework.BehaviorAlwaysSuccess";
    }
} 