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
            return instance;
        }

        /// <summary>
        /// 获取引用。
        /// </summary>
        /// <param name="type">引用类型。</param>
        /// <returns>引用。</returns>
        public static object Acquire(Type type)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!type.IsClass || !type.IsSubclassOf(typeof(IReference)))
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "Type is invalid.");
            }
#endif

            var cache = GetCache(type, true);
            var instance = cache.Count > 0 ? cache.Dequeue() : Activator.CreateInstance(type);
            return instance;
        }

        /// <summary>
        /// 释放引用。
        /// </summary>
        /// <param name="instance">引用。</param>
        public static void Release(IReference instance)
        {
            instance.Clear();
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
