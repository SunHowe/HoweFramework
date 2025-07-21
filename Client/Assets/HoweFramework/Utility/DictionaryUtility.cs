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
        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, Dictionary<TKey, TValue> other)
        {
            foreach (var item in other)
            {
                dict[item.Key] = item.Value;
            }
        }
    }
}