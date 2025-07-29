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

        public void Dispose()
        {
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
            set.DisposeItems = disposeItems;
            return set;
        }
    }
}
