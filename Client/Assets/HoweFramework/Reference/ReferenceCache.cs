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
