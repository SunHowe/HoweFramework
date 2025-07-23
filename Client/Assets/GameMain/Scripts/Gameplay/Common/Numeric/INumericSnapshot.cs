using System;
using System.Collections.Generic;

namespace GameMain
{
    /// <summary>
    /// 数值快照。
    /// </summary>
    public interface INumericSnapshot : IDisposable
    {
        /// <summary>
        /// 数值字典。
        /// </summary>
        IReadOnlyDictionary<int, long> NumericDict { get; }

        /// <summary>
        /// 获取最终值。
        /// </summary>
        /// <param name="id">属性id。</param>
        /// <returns>属性值。</returns>
        long GetFinal(int id);
        
        /// <summary>
        /// 获取属性值。
        /// </summary>
        /// <param name="id">属性id。</param>
        /// <returns>属性值。</returns>
        long Get(int id);
    }
}