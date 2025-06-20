using System;

namespace HoweFramework
{
    /// <summary>
    /// 行为节点接口。
    /// </summary>
    public interface IBehaviorNode : IDisposable
    {
        /// <summary>
        /// 行为树上下文。
        /// </summary>
        IBehaviorContext Context { get; }

        /// <summary>
        /// 执行行为。
        /// </summary>
        /// <returns>返回执行结果。</returns>
        int Execute();

        /// <summary>
        /// 重置状态。
        /// </summary>
        void ResetState();
    }
}