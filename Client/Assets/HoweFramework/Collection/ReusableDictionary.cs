using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 可复用字典。
    /// </summary>
    public sealed class ReusableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IReference, IDisposable
    {
        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public static ReusableDictionary<TKey, TValue> Create()
        {
            return ReferencePool.Acquire<ReusableDictionary<TKey, TValue>>();
        }
    }
}
