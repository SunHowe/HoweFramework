using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 可复用字典。
    /// </summary>
    public sealed class ReusableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IReference, IDisposable
    {
        /// <summary>
        /// 在自己Dispose时是否释放字典中的元素。
        /// </summary>
        public bool DisposeItems { get; set; }

        private bool m_Disposed;

        public void Dispose()
        {
            if (m_Disposed)
            {
                return;
            }

            m_Disposed = true;

            if (DisposeItems)
            {
                foreach (var item in this)
                {
                    if (item.Value is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }

            ReferencePool.Release(this);
        }

        public static ReusableDictionary<TKey, TValue> Create(bool disposeItems = false)
        {
            var dict = ReferencePool.Acquire<ReusableDictionary<TKey, TValue>>();
            dict.m_Disposed = false;
            dict.DisposeItems = disposeItems;
            return dict;
        }
    }
}
