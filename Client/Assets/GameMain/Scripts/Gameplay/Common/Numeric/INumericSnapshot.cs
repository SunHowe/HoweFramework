using System;
using System.Collections.Generic;

namespace GameMain
{
    /// <summary>
    /// 数值快照。
    /// </summary>
    public interface INumericSnapshot : INumeric, IDisposable
    {
        /// <summary>
        /// 数值字典。
        /// </summary>
        IReadOnlyDictionary<int, long> NumericDict { get; }
    }
}