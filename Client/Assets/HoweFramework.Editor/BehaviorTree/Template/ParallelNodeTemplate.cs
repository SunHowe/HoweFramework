using System.Collections.Generic;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 并行节点模板
    /// </summary>
    public class ParallelNodeTemplate : BehaviorNodeTemplate_Composite
    {
        public override string NodeName => "Parallel";
        public override string NodeDescription => "同时执行所有子节点";
        public override string RuntimeTypeName => "HoweFramework.BehaviorParallel";

        public override List<BehaviorNodePropertyTemplate> DefaultProperties =>
            new List<BehaviorNodePropertyTemplate>
            {
                new BehaviorNodePropertyTemplate(1, "RequireAll", BehaviorNodePropertyValueType.Bool, false),
                new BehaviorNodePropertyTemplate(2, "RequireCount", BehaviorNodePropertyValueType.Int, 1)
            };
    }
} 