using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 可复用队列。
    /// </summary>
    public sealed class ReusableQueue<T> : Queue<T>, IReference, IDisposable
    {
        /// <summary>
        /// 在自己Dispose时是否释放队列中的元素。
        /// </summary>
        public bool DisposeItems { get; set; }

        public void Dispose()
        {
            if (DisposeItems)
            {
                while (Count > 0)
                {
                    if (Dequeue() is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }

            ReferencePool.Release(this);
        }

        public static ReusableQueue<T> Create(bool disposeItems = false)
        {
            var queue = ReferencePool.Acquire<ReusableQueue<T>>();
            queue.DisposeItems = disposeItems;
            return queue;
        }
    }
}
