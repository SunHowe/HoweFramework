namespace HoweFramework
{
    /// <summary>
    /// 行为树装饰节点基类。
    /// </summary>
    public abstract class BehaviorDecorNodeBase : BehaviorNodeBase
    {
        /// <summary>
        /// 子节点。
        /// </summary>
        private BehaviorNodeBase m_Child;

        /// <summary>
        /// 设置子节点。
        /// </summary>
        /// <param name="child">子节点。</param>
        public void AddChild(BehaviorNodeBase child)
        {
            if (m_Child != null)
            {
                throw new ErrorCodeException(ErrorCode.BehaviorTreeDecorNodeAlreadyHasChild);
            }

            m_Child = child;
            m_Child.SetContext(Context);
        }

        /// <summary>
        /// 重置状态。
        /// </summary>
        public override void ResetState()
        {
            m_Child.ResetState();
        }

        /// <summary>
        /// 执行子节点。
        /// </summary>
        /// <returns>返回执行结果。</returns>
        protected int ExecuteChild()
        {
            if (m_Child == null)
            {
                return ErrorCode.BehaviorTreeDecorNodeNoChild;
            }

            return m_Child.Execute();
        }

        /// <summary>
        /// 清理。
        /// </summary>
        public override void Clear()
        {
            m_Child?.Dispose();
            m_Child = null;
            base.Clear();
        }
    }
}