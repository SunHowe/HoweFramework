using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 带实例id的引用缓存。
    /// </summary>
    internal class ReferenceWithIdCache : IReferenceCache
    {
        /// <summary>
        /// 缓存的引用数量。
        /// </summary>
        public int Count => m_ReferenceQueue.Count;

        /// <summary>
        /// 引用队列。
        /// </summary>
        private readonly Queue<IReferenceWithId> m_ReferenceQueue = new();

        /// <summary>
        /// 已入池实例集合，用于拒绝重复 Release。
        /// </summary>
        private readonly HashSet<IReference> m_InPoolSet = new(ReferenceIdentityComparer.Instance);

        /// <summary>
        /// 引用类型。
        /// </summary>
        private readonly Type m_ReferenceType;

        /// <summary>
        /// 自增长实例id。
        /// </summary>
        private int m_InstanceId = 0;

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="referenceType">引用类型。</param>
        public ReferenceWithIdCache(Type referenceType)
        {
            m_ReferenceType = referenceType;
        }

        /// <summary>
        /// 出队。
        /// </summary>
        public IReference Dequeue()
        {
            IReferenceWithId instance;
            if (m_ReferenceQueue.Count > 0)
            {
                instance = m_ReferenceQueue.Dequeue();
                m_InPoolSet.Remove(instance);
            }
            else
            {
                instance = (IReferenceWithId)Activator.CreateInstance(m_ReferenceType);
            }

            instance.InstanceId = ++m_InstanceId;
            return instance;
        }

        /// <summary>
        /// 入队。
        /// </summary>
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

            var instance = (IReferenceWithId)reference;
            instance.InstanceId = 0;
            m_ReferenceQueue.Enqueue(instance);
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