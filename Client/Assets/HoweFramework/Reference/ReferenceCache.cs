using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 引用缓存。
    /// </summary>
    internal class ReferenceCache : IReferenceCache
    {
        /// <summary>
        /// 缓存的引用数量。
        /// </summary>
        public int Count => m_ReferenceQueue.Count;

        /// <summary>
        /// 引用队列。
        /// </summary>
        private readonly Stack<IReference> m_ReferenceQueue = new();

        /// <summary>
        /// 已入池实例集合，用于拒绝重复 Release。
        /// </summary>
        private readonly HashSet<IReference> m_InPoolSet = new(ReferenceIdentityComparer.Instance);

        /// <summary>
        /// 引用类型。
        /// </summary>
        private readonly Type m_ReferenceType;

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="referenceType">引用类型。</param>
        public ReferenceCache(Type referenceType)
        {
            m_ReferenceType = referenceType;
        }

        /// <summary>
        /// 出队。
        /// </summary>
        /// <returns>引用。</returns>
        public IReference Dequeue()
        {
            if (m_ReferenceQueue.Count > 0)
            {
                var instance = m_ReferenceQueue.Pop();
                m_InPoolSet.Remove(instance);
                return instance;
            }

            return (IReference)Activator.CreateInstance(m_ReferenceType);
        }

        /// <summary>
        /// 入队。
        /// </summary>
        /// <param name="reference">引用。</param>
        public void Enqueue(IReference reference)
        {
            if (!m_InPoolSet.Add(reference))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                throw new ErrorCodeException(FrameworkErrorCode.InvalidOperationException, $"Reference of type '{m_ReferenceType.FullName}' is already in pool. Duplicate Release is not allowed.");
#else
                Log.Error($"Reference of type '{m_ReferenceType.FullName}' is already in pool. Duplicate Release is ignored.");
                return;
#endif
            }

            m_ReferenceQueue.Push(reference);
        }

        /// <summary>
        /// 清空。
        /// </summary>
        public void Clear()
        {
            m_ReferenceQueue.Clear();
            m_InPoolSet.Clear();
        }
    }
}
