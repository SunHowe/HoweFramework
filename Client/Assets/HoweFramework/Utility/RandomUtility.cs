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

            return list[random.GetRandom(list.Count)];
        }
    }
}
