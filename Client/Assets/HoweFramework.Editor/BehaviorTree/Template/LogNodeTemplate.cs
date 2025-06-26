using System.Collections.Generic;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 日志节点模板
    /// </summary>
    public class LogNodeTemplate : BehaviorNodeTemplate_Action
    {
        public override string NodeName => "Log";
        public override string NodeDescription => "输出日志信息";
        public override string RuntimeTypeName => "HoweFramework.BehaviorLog";

        public override List<BehaviorNodePropertyTemplate> DefaultProperties =>
            new List<BehaviorNodePropertyTemplate>
            {
                new BehaviorNodePropertyTemplate("Message", BehaviorPropertyType.String, "Hello World"),
                new BehaviorNodePropertyTemplate("LogLevel", BehaviorPropertyType.Int, 0)
            };
    }
} 