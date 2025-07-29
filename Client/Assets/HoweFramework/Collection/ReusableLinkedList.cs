using System;

namespace HoweFramework
{
    /// <summary>
    /// 可复用链表。
    /// </summary>
    public sealed class ReusableLinkedList<T> : LinkedListEx<T>, IReference, IDisposable
    {
        /// <summary>
        /// 在自己Dispose时是否释放列表中的元素。
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

        public static ReusableLinkedList<T> Create(bool disposeItems = false)
        {
            var list = ReferencePool.Acquire<ReusableLinkedList<T>>();
            list.DisposeItems = disposeItems;
            return list;
        }
    }
}
