namespace HoweFramework
{
    /// <summary>
    /// 行为树-重复装饰节点。重复执行子节点，直到子节点成功N次。
    /// 若子节点返回Running，则返回Running，下次继续运行。
    /// 若子节点返回失败，则返回失败。
    /// </summary>
    public sealed class BehaviorRepeat : BehaviorDecorNodeBase
    {
        /// <summary>
        /// 重复次数。
        /// </summary>
        public int RepeatCount { get; set; }

        /// <summary>
        /// 已运行次数。
        /// </summary>
        private int m_Times;

        /// <summary>
        /// 上一次执行结果。
        /// </summary>
        private int m_PreviousResult;

        /// <summary>
        /// 执行。
        /// </summary>
        /// <returns>返回执行结果。</returns>
        public override int Execute()
        {
            // 若上一次执行结果不是成功或运行状态，则返回上一次执行结果。
            if (m_PreviousResult != ErrorCode.Success && m_PreviousResult != ErrorCode.BehaviorRunningState)
            {
                return m_PreviousResult;
            }

            while (m_Times < RepeatCount)
            {
                var result = ExecuteChild();
                if (result == ErrorCode.BehaviorRunningState)
                {
                    m_PreviousResult = result;
                    return ErrorCode.BehaviorRunningState;
                }

                if (result != ErrorCode.Success)
                {
                    m_PreviousResult = result;
                    return result;
                }

                ++m_Times;
            }

            return ErrorCode.Success;
        }

        /// <summary>
        /// 重置状态。
        /// </summary>
        public override void ResetState()
        {
            m_Times = 0;
            m_PreviousResult = ErrorCode.Success;
            base.ResetState();
        }

        /// <summary>
        /// 清理。
        /// </summary>
        public override void Clear()
        {
            m_Times = 0;
            m_PreviousResult = ErrorCode.Success;
            RepeatCount = 0;
            base.Clear();
        }

        /// <summary>
        /// 创建行为树-重复装饰节点。
        /// </summary>
        /// <param name="timesLimit">次数限制。</param>
        /// <returns>返回行为树-重复装饰节点。</returns>
        public static BehaviorRepeat Create(int timesLimit)
        {
            if (timesLimit <= 0)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "timesLimit must be greater than 0");
            }

            var behaviorRepeat = ReferencePool.Acquire<BehaviorRepeat>();
            behaviorRepeat.RepeatCount = timesLimit;
            return behaviorRepeat;
        }
    }
}