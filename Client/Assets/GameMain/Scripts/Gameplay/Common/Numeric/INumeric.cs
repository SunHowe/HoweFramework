using System;
using System.Collections.Generic;

namespace GameMain
{
    /// <summary>
    /// 数值接口。
    /// </summary>
    public interface INumeric : IDisposable
    {
        /// <summary>
        /// 数值字典。
        /// </summary>
        IReadOnlyDictionary<int, long> NumericDict { get; }
        
        /// <summary>
        /// 获取属性值。
        /// </summary>
        /// <param name="id">属性id。</param>
        /// <param name="subType">属性子类型。</param>
        /// <returns>属性值。</returns>
        long Get(int id, NumericSubType subType = NumericSubType.Final);

        /// <summary>
        /// 获取属性值。
        /// </summary>
        /// <param name="key">属性键值。</param>
        /// <returns>属性值。</returns>
        long GetByKey(int key);

        /// <summary>
        /// 克隆数值。
        /// </summary>
        INumeric Clone();
    }
}