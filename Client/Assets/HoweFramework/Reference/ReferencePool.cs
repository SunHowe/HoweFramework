using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
        private static readonly Dictionary<Type, IReferenceCache> m_ReferenceCacheDict = new();

        /// <summary>
        /// 线程同步锁。网络线程（Socket 回调）与主线程都会访问引用池，必须加锁。
        /// </summary>
        private static readonly object s_Lock = new();

        /// <summary>
        /// 获取引用。
        /// </summary>
        /// <typeparam name="T">引用类型。</typeparam>
        /// <returns>引用。</returns>
        public static T Acquire<T>() where T : class, IReference, new()
        {
            return (T)Acquire(typeof(T));
        }

        /// <summary>
        /// 获取引用。
        /// </summary>
        /// <param name="type">引用类型。</param>
        /// <returns>引用。</returns>
        public static IReference Acquire(Type type)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!type.IsClass || !typeof(IReference).IsAssignableFrom(type))
            {
                throw new ErrorCodeException(FrameworkErrorCode.InvalidOperationException, "Type is invalid.");
            }
#endif

            lock (s_Lock)
            {
                return GetCache(type, true).Dequeue();
            }
        }

        /// <summary>
        /// 释放引用。
        /// </summary>
        /// <param name="instance">引用。</param>
        public static void Release(IReference instance)
        {
            if (instance == null)
            {
                return;
            }

            instance.Clear();
            lock (s_Lock)
            {
                GetCache(instance.GetType(), true).Enqueue(instance);
            }
        }

        /// <summary>
        /// 清理缓存。
        /// </summary>
        /// <typeparam name="T">引用类型。</typeparam>
        public static void ClearCache<T>() where T : class, IReference
        {
            lock (s_Lock)
            {
                var cache = GetCache(typeof(T), false);
                if (cache == null)
                {
                    return;
                }

                cache.Clear();
            }
        }

        /// <summary>
        /// 清理所有缓存。
        /// </summary>
        public static void ClearAllCache()
        {
            lock (s_Lock)
            {
                foreach (var cache in m_ReferenceCacheDict.Values)
                {
                    cache.Clear();
                }
                m_ReferenceCacheDict.Clear();
            }
        }

        /// <summary>
        /// 获取引用缓存。
        /// </summary>
        /// <param name="type">引用类型。</param>
        /// <param name="createIfNotExists">如果缓存不存在，是否创建。</param>
        /// <returns>引用缓存。</returns>
        private static IReferenceCache GetCache(Type type, bool createIfNotExists = true)
        {
            if (m_ReferenceCacheDict.TryGetValue(type, out var cache))
            {
                return cache;
            }

            if (!createIfNotExists)
            {
                return null;
            }

            if (typeof(IReferenceWithId).IsAssignableFrom(type))
            {
                cache = new ReferenceWithIdCache(type);
            }
            else
            {
                cache = new ReferenceCache(type);
            }

            m_ReferenceCacheDict[type] = cache;

            return cache;
        }
    }

    /// <summary>
    /// 按对象引用比较，避免 IReference 重写 Equals 后误判重复入池。
    /// </summary>
    internal sealed class ReferenceIdentityComparer : IEqualityComparer<IReference>
    {
        public static readonly ReferenceIdentityComparer Instance = new();

        private ReferenceIdentityComparer()
        {
        }

        public bool Equals(IReference x, IReference y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(IReference obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
