namespace HoweFramework
{
    /// <summary>
    /// 行为树日志节点。
    /// </summary>
    public sealed class BehaviorLog : BehaviorActionNodeBase
    {
        /// <summary>
        /// 消息。
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 日志级别。
        /// </summary>
        public int LogLevel { get; set; }

        /// <summary>
        /// 执行。
        /// </summary>
        /// <returns>返回执行结果。</returns>
        public override int Execute()
        {
            Log.Info(TextUtility.ParseTemplate(Message, GetContextValue));
            return ErrorCode.Success;
        }

        /// <summary>
        /// 重置状态。
        /// </summary>
        public override void ResetState()
        {
        }

        /// <summary>
        /// 清理。
        /// </summary>
        public override void Clear()
        {
            Message = null;
            base.Clear();
        }

        /// <summary>
        /// 获取上下文值。
        /// </summary>
        private bool GetContextValue(string key, out string value)
        {
            value = Context.GetValueString(key);
            return value != null;
        }

        /// <summary>
        /// 创建行为树日志节点。
        /// </summary>
        /// <param name="message">消息。</param>
        /// <returns>返回行为树日志节点。</returns>
        public static BehaviorLog Create(string message)
        {
            var behaviorLog = ReferencePool.Acquire<BehaviorLog>();
            behaviorLog.Message = message;
            return behaviorLog;
        }
    }
}