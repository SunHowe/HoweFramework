using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 随机数工具类。
    /// </summary>
    public static class RandomUtility
    {
        /// <summary>
        /// 获取随机项。
        /// </summary>
        /// <typeparam name="T">类型。</typeparam>
        /// <param name="random">随机数管理器。</param>
        /// <param name="list">列表。</param>
        /// <returns>随机项。</returns>
        public static T GetRandomItem<T>(this IRandom random, List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return default;
            }

            if (list.Count == 1)
            {
                return list[0];
            }

            return list[random.GetRandom(list.Count)];
        }

        /// <summary>
        /// 随机选择一个。
        /// </summary>
        /// <typeparam name="T">类型。</typeparam>
        /// <param name="random">随机数。</param>
        /// <param name="item1">选项1。</param>
        /// <param name="item2">选项2。</param>
        /// <returns>随机选择的一个选项。</returns>
        public static T RandomOne<T>(this IRandom random, T item1, T item2)
        {
            return random.GetRandom(2) == 0 ? item1 : item2;
        }

        /// <summary>
        /// 随机选择一个。
        /// </summary>
        /// <typeparam name="T">类型。</typeparam>
        /// <param name="random">随机数。</param>
        /// <param name="item1">选项1。</param>
        /// <param name="item2">选项2。</param>
        /// <param name="item3">选项3。</param>
        /// <returns>随机选择的一个选项。</returns>
        public static T RandomOne<T>(this IRandom random, T item1, T item2, T item3)
        {
            var index = random.GetRandom(3);
            return index switch
            {
                0 => item1,
                1 => item2,
                2 => item3,
                _ => default,
            };
        }

        /// <summary>
        /// 随机选择一个。
        /// </summary>
        /// <typeparam name="T">类型。</typeparam>
        /// <param name="random">随机数。</param>
        /// <param name="items">选项。</param>
        /// <returns>随机选择的一个选项。</returns>
        public static T RandomOne<T>(this IRandom random, params T[] items)
        {
            if (items == null || items.Length == 0)
            {
                return default;
            }

            if (items.Length == 1)
            {
                return items[0];
            }

            return items[random.GetRandom(items.Length)];
        }

        /// <summary>
        /// 随机选择一个。
        /// </summary>
        /// <typeparam name="T">类型。</typeparam>
        /// <param name="random">随机数。</param>
        /// <param name="list">列表。</param>
        /// <returns>随机选择的一个选项。</returns>
        public static T RandomOne<T>(this IRandom random, List<T> list)
        {
            return random.GetRandomItem(list);
        }

        /// <summary>
        /// 是否命中。基于百分比命中概率。
        /// </summary>
        /// <param name="random">随机数。</param>
        /// <param name="hitRate">命中概率。</param>
        /// <returns>是否命中。</returns>
        public static bool IsHit(this IRandom random, int hitRate)
        {
            return random.GetRandom(100) < hitRate;
        }

        /// <summary>
        /// 是否命中。基于千分比命中概率。
        /// </summary>
        /// <param name="random">随机数。</param>
        /// <param name="hitRate">命中概率。</param>
        /// <returns>是否命中。</returns>
        public static bool IsHitThousand(this IRandom random, int hitRate)
        {
            return random.GetRandom(1000) < hitRate;
        }

        /// <summary>
        /// 是否命中。基于万分比命中概率。
        /// </summary>
        /// <param name="random">随机数。</param>
        /// <param name="hitRate">命中概率。</param>
        /// <returns>是否命中。</returns>
        public static bool IsHitTenThousand(this IRandom random, int hitRate)
        {
            return random.GetRandom(10000) < hitRate;
        }
    }
}
