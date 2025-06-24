namespace HoweFramework
{
    /// <summary>
    /// 行为节点基类。
    /// </summary>
    public abstract class BehaviorNodeBase : IBehaviorNode, IReference
    {
        /// <summary>
        /// 行为树上下文。
        /// </summary>
        public IBehaviorContext Context { get; private set; }

        /// <summary>
        /// 设置行为树上下文。
        /// </summary>
        /// <param name="context">行为树上下文。</param>
        internal void SetContext(IBehaviorContext context)
        {
            Context = context;
        }

        /// <summary>
        /// 执行行为。
        /// </summary>
        /// <returns>返回执行结果。</returns>
        public abstract int Execute();

        /// <summary>
        /// 重置状态。
        /// </summary>
        public abstract void ResetState();
        
        /// <summary>
        /// 释放。
        /// </summary>
        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        /// <summary>
        /// 清理。
        /// </summary>
        public virtual void Clear()
        {
            Context = null;
        }
    }
}