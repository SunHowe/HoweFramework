using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 引用缓存。
    /// </summary>
    internal class ReferenceCache
    {
        /// <summary>
        /// 缓存的引用数量。
        /// </summary>
        public int Count => m_ReferenceQueue.Count;

        /// <summary>
        /// 引用队列。
        /// </summary>
        private readonly Queue<IReference> m_ReferenceQueue = new();

        /// <summary>
        /// 自增长实例id。
        /// </summary>
        private int m_InstanceId;

        /// <summary>
        /// 获取实例id。
        /// </summary>
        /// <returns>实例id。</returns>
        public int AcquireInstanceId()
        {
            return ++m_InstanceId;
        }

        /// <summary>
        /// 出队。
        /// </summary>
        /// <returns>引用。</returns>
        public IReference Dequeue()
        {
            return m_ReferenceQueue.Dequeue();
        }

        /// <summary>
        /// 入队。
        /// </summary>
        /// <param name="reference">引用。</param>
        public void Enqueue(IReference reference)
        {
            m_ReferenceQueue.Enqueue(reference);
        }

        /// <summary>
        /// 清空。
        /// </summary>
        public void Clear()
        {
            m_ReferenceQueue.Clear();
        }
    }
}
