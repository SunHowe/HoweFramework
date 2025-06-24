namespace HoweFramework
{
    /// <summary>
    /// 行为树失败节点。
    /// </summary>
    public sealed class BehaviorFailure : BehaviorActionNodeBase
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
            return ErrorCode;
        }

        /// <summary>
        /// 重置状态。
        /// </summary>
        public override void ResetState()
        {
        }

        /// <summary>
        /// 创建行为树失败节点。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        /// <returns>返回行为树失败节点。</returns>
        public static BehaviorFailure Create(int errorCode = HoweFramework.ErrorCode.Unknown)
        {
            var behaviorFailure = ReferencePool.Acquire<BehaviorFailure>();
            behaviorFailure.ErrorCode = errorCode;
            return behaviorFailure;
        }
    }
}