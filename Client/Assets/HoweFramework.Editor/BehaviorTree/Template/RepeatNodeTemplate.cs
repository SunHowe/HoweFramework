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
                new BehaviorNodePropertyTemplate(1, "RepeatCount", BehaviorPropertyType.Int, 1),
                new BehaviorNodePropertyTemplate(2, "IgnoreFailure", BehaviorPropertyType.Bool, false)
            };
    }
} 