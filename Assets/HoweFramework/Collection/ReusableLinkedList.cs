using System;

namespace HoweFramework
{
    /// <summary>
    /// 可复用链表。
    /// </summary>
    public sealed class ReusableLinkedList<T> : LinkedListEx<T>, IReference, IDisposable
    {
        public int ReferenceId { get; set; }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public static ReusableLinkedList<T> Create()
        {
            return ReferencePool.Acquire<ReusableLinkedList<T>>();
        }
    }
}
