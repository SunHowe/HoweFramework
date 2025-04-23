using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 引用池。
    /// </summary>
    public static class ReferencePool
    {
        /// <summary>
        /// 引用缓存字典。
        /// </summary>
        private static readonly Dictionary<Type, ReferenceCache> m_ReferenceCacheDict = new();

        /// <summary>
        /// 获取引用。
        /// </summary>
        /// <typeparam name="T">引用类型。</typeparam>
        /// <returns>引用。</returns>
        public static T Acquire<T>() where T : class, IReference, new()
        {
            var cache = GetCache(typeof(T), true);
            var instance = cache.Count > 0 ? (T)cache.Dequeue() : new T();
            instance.ReferenceId = cache.AcquireInstanceId();
            return instance;
        }

        /// <summary>
        /// 释放引用。
        /// </summary>
        /// <param name="instance">引用。</param>
        public static void Release(IReference instance)
        {
            if (instance.ReferenceId == 0)
            {
                // 未被引用池管理的实例或重复释放的实例。
                instance.Clear();
                return;
            }

            instance.Clear();
            instance.ReferenceId = 0;
            GetCache(instance.GetType(), true).Enqueue(instance);
        }

        /// <summary>
        /// 清理缓存。
        /// </summary>
        /// <typeparam name="T">引用类型。</typeparam>
        public static void ClearCache<T>() where T : class, IReference
        {
            var cache = GetCache(typeof(T), false);
            if (cache == null)
            {
                return;
            }

            cache.Clear();
        }

        /// <summary>
        /// 清理所有缓存。
        /// </summary>
        public static void ClearAllCache()
        {
            m_ReferenceCacheDict.Clear();
        }

        /// <summary>
        /// 获取引用缓存。
        /// </summary>
        /// <param name="type">引用类型。</param>
        /// <param name="createIfNotExists">如果缓存不存在，是否创建。</param>
        /// <returns>引用缓存。</returns>
        private static ReferenceCache GetCache(Type type, bool createIfNotExists = true)
        {
            if (m_ReferenceCacheDict.TryGetValue(type, out var cache))
            {
                return cache;
            }

            if (!createIfNotExists)
            {
                return null;
            }

            cache = new ReferenceCache();
            m_ReferenceCacheDict[type] = cache;

            return cache;
        }
    }
}
