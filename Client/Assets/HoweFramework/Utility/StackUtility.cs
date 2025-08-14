using System;
using System.Collections.Generic;

namespace HoweFramework
{
    public static class StackUtility
    {
        /// <summary>
        /// 释放栈中的元素。
        /// </summary>
        /// <typeparam name="T">栈元素类型。</typeparam>
        /// <param name="stack">栈。</param>
        public static void DisposeItems<T>(this Stack<T> stack) where T : IDisposable
        {
            while (stack.Count > 0)
            {
                stack.Pop().Dispose();
            }
        }
    }
}