namespace HoweFramework
{
    /// <summary>
    /// 行为树选择器节点。所有子节点中，只要有一个子节点执行成功，就认为执行成功。
    /// </summary>
    public sealed class BehaviorSelector : BehaviorCompositeNodeBase
    {
        /// <summary>
        /// 当前索引。
        /// </summary>
        private int m_CurrentIndex = 0;

        /// <summary>
        /// 执行。
        /// </summary>
        /// <returns>返回执行结果。</returns>
        public override int Execute()
        {
            while (m_CurrentIndex < ChildCount)
            {
                var result = ExecuteChild(m_CurrentIndex);
                if (result == ErrorCode.BehaviorRunningState)
                {
                    return result;
                }

                if (result == ErrorCode.Success)
                {
                    return result;
                }

                ++m_CurrentIndex;
            }

            return ErrorCode.BehaviorTreeSelectorNoSuccess;
        }

        /// <summary>
        /// 重置状态。
        /// </summary>
        public override void ResetState()
        {
            m_CurrentIndex = 0;
        }

        /// <summary>
        /// 清理。
        /// </summary>
        public override void Clear()
        {
            m_CurrentIndex = 0;
            base.Clear();
        }

        /// <summary>
        /// 创建行为树选择器节点。
        /// </summary>
        /// <returns>返回行为树选择器节点。</returns>
        public static BehaviorSelector Create()
        {
            return ReferencePool.Acquire<BehaviorSelector>();
        }
    }
}   