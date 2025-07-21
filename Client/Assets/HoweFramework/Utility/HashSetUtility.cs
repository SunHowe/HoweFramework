using System.Collections.Generic;

namespace HoweFramework
{
    public static class HashSetUtility
    {
        /// <summary>
        /// 将另一个集合的元素添加到当前集合中。
        /// </summary>
        /// <typeparam name="T">集合元素类型。</typeparam>
        /// <param name="set">当前集合。</param>
        /// <param name="other">要添加的另一个集合。</param>
        public static void AddRange<T>(this HashSet<T> set, HashSet<T> other)
        {
            foreach (var item in other)
            {
                set.Add(item);
            }
        }
    }
}