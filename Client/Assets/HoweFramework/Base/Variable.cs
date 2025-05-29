using System;

namespace HoweFramework
{
    /// <summary>
    /// 通用的引用变量。
    /// </summary>
    public class Variable<T> : IDisposable, IReference
    {
        /// <summary>
        /// 引用的变量值。
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// 是否在释放自己时释放变量值。
        /// </summary>
        public bool ReleaseValue { get; set; }

        public void Clear()
        {
            if (ReleaseValue && Value is IReference reference)
            {
                ReferencePool.Release(reference);
            }

            Value = default;
            ReleaseValue = false;
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public static implicit operator T(Variable<T> variable)
        {
            return variable.Value;
        }

        public static Variable<T> Create(T value = default, bool releaseValue = false)
        {
            var variable = ReferencePool.Acquire<Variable<T>>();
            variable.Value = value;
            variable.ReleaseValue = releaseValue;
            return variable;
        }
    }
}
