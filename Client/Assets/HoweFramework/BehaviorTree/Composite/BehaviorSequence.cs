namespace HoweFramework
{
    /// <summary>
    /// 行为树顺序节点。所有子节点必须全部执行成功，才算执行成功。
    /// </summary>
    public sealed class BehaviorSequence : BehaviorCompositeNodeBase
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
                if (result != ErrorCode.Success)
                {
                    return result;
                }

                ++m_CurrentIndex;
            }

            return ErrorCode.Success;
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
        /// 创建行为树顺序节点。
        /// </summary>
        /// <returns>返回行为树顺序节点。</returns>
        public static BehaviorSequence Create()
        {
            return ReferencePool.Acquire<BehaviorSequence>();;
        }
    }
}