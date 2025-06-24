namespace HoweFramework
{
    /// <summary>
    /// 行为树成功节点。
    /// </summary>
    public sealed class BehaviorSuccess : BehaviorActionNodeBase
    {
        /// <summary>
        /// 执行。
        /// </summary>
        /// <returns>返回执行结果。</returns>
        public override int Execute()
        {
            return ErrorCode.Success;
        }

        /// <summary>
        /// 重置状态。
        /// </summary>
        public override void ResetState()
        {
        }

        /// <summary>
        /// 创建行为树成功节点。
        /// </summary>
        /// <returns>返回行为树成功节点。</returns>
        public static BehaviorSuccess Create()
        {
            return ReferencePool.Acquire<BehaviorSuccess>();
        }
    }
}