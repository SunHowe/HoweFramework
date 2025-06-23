using System.Collections.Generic;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 重复节点模板
    /// </summary>
    public class RepeatNodeTemplate : BehaviorNodeTemplate_Decor
    {
        public override string NodeName => "Repeat";
        public override string NodeDescription => "重复执行子节点指定次数";
        public override string RuntimeTypeName => "HoweFramework.BehaviorRepeat";

        public override List<BehaviorNodePropertyTemplate> DefaultProperties =>
            new List<BehaviorNodePropertyTemplate>
            {
                new BehaviorNodePropertyTemplate("RepeatCount", BehaviorNodePropertyValueType.Int, "重复次数", 1, true),
                new BehaviorNodePropertyTemplate("IgnoreFailure", BehaviorNodePropertyValueType.Bool, "忽略失败", false)
            };
    }
} 