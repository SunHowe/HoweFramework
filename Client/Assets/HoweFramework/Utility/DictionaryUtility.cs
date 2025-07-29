using System.Collections.Generic;

namespace HoweFramework
{
    public static class DictionaryUtility
    {
        /// <summary>
        /// 将另一个字典的键值对添加到当前字典中。
        /// </summary>
        /// <typeparam name="TKey">键类型。</typeparam>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="dict">当前字典。</param>
        /// <param name="other">要添加的另一个字典。</param>
        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, IReadOnlyDictionary<TKey, TValue> other)
        {
            foreach (var item in other)
            {
                dict[item.Key] = item.Value;
            }
        }

        /// <summary>
        /// 克隆字典。
        /// </summary>
        /// <typeparam name="TKey">键类型。</typeparam>
        /// <typeparam name="TValue">值类型。</typeparam>
        /// <param name="dict">要克隆的字典。</param>
        /// <returns>克隆后的字典。</returns>
        public static ReusableDictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> dict)
        {
            var newDict = ReusableDictionary<TKey, TValue>.Create();
            foreach (var item in dict)
            {
                newDict[item.Key] = item.Value;
            }
            return newDict;
        }
    }
}