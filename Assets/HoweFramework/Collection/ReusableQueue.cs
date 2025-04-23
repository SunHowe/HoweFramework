using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 可复用队列。
    /// </summary>
    public sealed class ReusableQueue<T> : Queue<T>, IReference, IDisposable
    {
        public int ReferenceId { get; set; }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public static ReusableQueue<T> Create()
        {
            return ReferencePool.Acquire<ReusableQueue<T>>();
        }
    }
}
