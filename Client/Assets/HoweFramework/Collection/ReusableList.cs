using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 可复用列表。
    /// </summary>
    public sealed class ReusableList<T> : List<T>, IReference, IDisposable
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

        public static ReusableList<T> Create(bool disposeItems = false)
        {
            var list = ReferencePool.Acquire<ReusableList<T>>();
            list.DisposeItems = disposeItems;
            return list;
        }
    }
}
