using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 可复用哈希集合。
    /// </summary>
    public sealed class ReusableHashSet<T> : HashSet<T>, IReference, IDisposable
    {
        /// <summary>
        /// 在自己Dispose时是否释放集合中的元素。
        /// </summary>
        public bool DisposeItems { get; set; }

        private bool m_Disposed;

        public void Dispose()
        {
            if (m_Disposed)
            {
                return;
            }

            m_Disposed = true;

            if (DisposeItems)
            {
                foreach (var item in this)
                {
                    if (item is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }

            ReferencePool.Release(this);
        }

        public static ReusableHashSet<T> Create(bool disposeItems = false)
        {
            var set = ReferencePool.Acquire<ReusableHashSet<T>>();
            set.m_Disposed = false;
            set.DisposeItems = disposeItems;
            return set;
        }
    }
}
