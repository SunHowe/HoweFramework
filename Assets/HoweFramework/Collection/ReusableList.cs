using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 可复用列表。
    /// </summary>
    public sealed class ReusableList<T> : List<T>, IReference, IDisposable
    {
        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public static ReusableList<T> Create()
        {
            return ReferencePool.Acquire<ReusableList<T>>();
        }
    }
}
