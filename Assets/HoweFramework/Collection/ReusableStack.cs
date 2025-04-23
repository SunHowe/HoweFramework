using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 可复用栈。
    /// </summary>
    public sealed class ReusableStack<T> : Stack<T>, IReference, IDisposable
    {
        public int ReferenceId { get; set; }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public static ReusableStack<T> Create()
        {
            return ReferencePool.Acquire<ReusableStack<T>>();
        }
    }
}
