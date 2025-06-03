using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 列表工具类。
    /// </summary>
    public static class ListUtility
    {
        /// <summary>
        /// 添加元素到列表中，并保持列表有序。
        /// </summary>
        /// <typeparam name="T">列表元素类型。</typeparam>
        /// <param name="list">列表。</param>
        /// <param name="item">要添加的元素。</param>
        /// <param name="comparer">比较器。</param>
        public static void BinaryInsert<T>(this List<T> list, T item, IComparer<T> comparer)
        {
            if (list.Count == 0)
            {
                list.Add(item);
                return;
            }

            int left = 0;
            int right = list.Count - 1;

            while (left <= right)
            {
                int mid = (left + right) / 2;
                int compareResult = comparer.Compare(item, list[mid]);

                if (compareResult < 0)
                {
                    right = mid - 1;
                }
                else
                {
                    left = mid + 1;
                }
            }

            list.Insert(left, item);
        }

        /// <summary>
        /// 添加元素到列表中，并保持列表有序。
        /// </summary>
        /// <typeparam name="T">列表元素类型。</typeparam>
        /// <param name="list">列表。</param>
        /// <param name="item">要添加的元素。</param>
        /// <param name="comparison">比较器。</param>
        public static void BinaryInsert<T>(this List<T> list, T item, Comparison<T> comparison)
        {
            if (list.Count == 0)
            {
                list.Add(item);
            }

            int left = 0;
            int right = list.Count - 1;

            while (left <= right)
            {
                int mid = (left + right) / 2;
                int compareResult = comparison(item, list[mid]);

                if (compareResult < 0)
                {
                    right = mid - 1;
                }
                else
                {
                    left = mid + 1;
                }
            }

            list.Insert(left, item);
        }

        /// <summary>
        /// 添加元素到列表中，并保持列表有序。
        /// </summary>
        /// <typeparam name="T">列表元素类型。</typeparam>
        /// <param name="list">列表。</param>
        /// <param name="item">要添加的元素。</param>
        public static void BinaryInsert<T>(this List<T> list, T item) where T : IComparable<T>
        {
            BinaryInsert(list, item, Comparer<T>.Default);
        }
    }
}

