using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 行为树复合节点基类。
    /// </summary>
    public abstract class BehaviorCompositeNodeBase : BehaviorNodeBase
    {
        /// <summary>
        /// 子节点数量。
        /// </summary>
        public int ChildCount => m_Children.Count;

        /// <summary>
        /// 子节点列表。
        /// </summary>
        private readonly List<BehaviorNodeBase> m_Children = new List<BehaviorNodeBase>();

        /// <summary>
        /// 添加子节点。
        /// </summary>
        /// <param name="child">子节点。</param>
        public void AddChild(BehaviorNodeBase child)
        {
            m_Children.Add(child);
            child.SetContext(Context);
        }

        /// <summary>
        /// 重置状态。
        /// </summary>
        public override void ResetState()
        {
            foreach (var child in m_Children)
            {
                child.ResetState();
            }
        }

        /// <summary>
        /// 执行子节点。
        /// </summary>
        /// <param name="index">子节点索引。</param>
        /// <returns>返回执行结果。</returns>
        protected int ExecuteChild(int index)
        {
            if (index < 0 || index >= m_Children.Count)
            {
                return ErrorCode.InvalidParam;
            }

            return m_Children[index].Execute();
        }

        /// <summary>
        /// 清理。
        /// </summary>
        public override void Clear()
        {
            foreach (var child in m_Children)
            {
                child.Dispose();
            }
            
            m_Children.Clear();
            base.Clear();
        }
    }
}