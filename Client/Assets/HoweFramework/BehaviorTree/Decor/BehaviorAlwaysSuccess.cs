namespace HoweFramework
{
    /// <summary>
    /// 行为树-总是成功装饰节点。无论子节点执行结果如何(除运行状态外)，都返回成功。
    /// </summary>
    public sealed class BehaviorAlwaysSuccess : BehaviorDecorNodeBase
    {
        /// <summary>
        /// 执行。
        /// </summary>
        /// <returns>返回执行结果。</returns>
        public override int Execute()
        {
            var result = ExecuteChild();
            if (result == ErrorCode.BehaviorRunningState)
            {
                return result;
            }

            return ErrorCode.Success;
        }

        /// <summary>
        /// 创建行为树-总是成功装饰节点。
        /// </summary>
        /// <returns>返回行为树-总是成功装饰节点。</returns>
        public static BehaviorAlwaysSuccess Create()
        {
            return ReferencePool.Acquire<BehaviorAlwaysSuccess>();
        }
    }
}