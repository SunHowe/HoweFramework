namespace HoweFramework
{
    /// <summary>
    /// 行为树-总是失败装饰节点。无论子节点执行结果如何(除运行状态外)，都返回失败。
    /// </summary>
    public sealed class BehaviorAlwaysFailure : BehaviorDecorNodeBase
    {
        /// <summary>
        /// 错误码。
        /// </summary>
        public int ErrorCode { get; set; } = HoweFramework.ErrorCode.Unknown;

        /// <summary>
        /// 执行。
        /// </summary>
        /// <returns>返回执行结果。</returns>
        public override int Execute()
        {
            var result = ExecuteChild();
            if (result == HoweFramework.ErrorCode.BehaviorRunningState)
            {
                return result;
            }

            return ErrorCode;
        }

        /// <summary>
        /// 创建行为树-总是失败装饰节点。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        /// <returns>返回行为树-总是失败装饰节点。</returns>
        public static BehaviorAlwaysFailure Create(int errorCode = HoweFramework.ErrorCode.Unknown)
        {
            var behaviorAlwaysFailure = ReferencePool.Acquire<BehaviorAlwaysFailure>();
            behaviorAlwaysFailure.ErrorCode = errorCode;
            return behaviorAlwaysFailure;
        }
    }
}