using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 行为树并行节点。所有子节点必须全部执行成功，才算执行成功。
    /// </summary>
    public sealed class BehaviorParallel : BehaviorCompositeNodeBase
    {
        /// <summary>
        /// 子节点执行结果列表。
        /// </summary>
        private readonly List<int> m_ChildResultList = new();

        /// <summary>
        /// 执行。
        /// </summary>
        /// <returns>返回执行结果。</returns>
        public override int Execute()
        {
            if (m_ChildResultList.Count != ChildCount)
            {
                for (int i = 0; i < ChildCount; i++)
                {
                    m_ChildResultList.Add(ErrorCode.BehaviorRunningState);
                }
            }

            var result = ErrorCode.Success;

            for (int i = 0; i < ChildCount; i++)
            {
                var childResult = m_ChildResultList[i];
                if (childResult == ErrorCode.Success)
                {
                    continue;
                }

                if (childResult != ErrorCode.BehaviorRunningState)
                {
                    result = childResult;
                    continue;
                }

                childResult = ExecuteChild(i);
                m_ChildResultList[i] = childResult;

                if (childResult == ErrorCode.Success)
                {
                    continue;
                }

                if (childResult == ErrorCode.BehaviorRunningState)
                {
                    if (result == ErrorCode.Success)
                    {
                        // 仅当其他节点都是成功，才允许返回Running状态。
                        result = childResult;
                    }

                    continue;
                }

                // 记录失败的错误码。
                result = childResult;
            }

            return result;
        }

        /// <summary>
        /// 重置状态。
        /// </summary>
        public override void ResetState()
        {
            m_ChildResultList.Clear();
            base.ResetState();
        }

        /// <summary>
        /// 清理。
        /// </summary>
        public override void Clear()
        {
            m_ChildResultList.Clear();
            base.Clear();
        }
    }
}