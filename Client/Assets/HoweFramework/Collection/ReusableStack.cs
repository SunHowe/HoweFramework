using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 可复用栈。
    /// </summary>
    public sealed class ReusableStack<T> : Stack<T>, IReference, IDisposable
    {
        /// <summary>
        /// 在自己Dispose时是否释放栈中的元素。
        /// </summary>
        public bool DisposeItems { get; set; }

        public void Dispose()
        {
            if (DisposeItems)
            {
                while (Count > 0)
                {
                    if (Pop() is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }

            ReferencePool.Release(this);
        }

        public static ReusableStack<T> Create(bool disposeItems = false )
        {
            var stack = ReferencePool.Acquire<ReusableStack<T>>();
            stack.DisposeItems = disposeItems;
            return stack;
        }
    }
}
