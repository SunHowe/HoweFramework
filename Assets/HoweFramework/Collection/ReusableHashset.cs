using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 可复用哈希集合。
    /// </summary>
    public sealed class ReusableHashSet<T> : HashSet<T>, IReference, IDisposable
    {
        public int ReferenceId { get; set; }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public static ReusableHashSet<T> Create()
        {
            return ReferencePool.Acquire<ReusableHashSet<T>>();
        }
    }
}
