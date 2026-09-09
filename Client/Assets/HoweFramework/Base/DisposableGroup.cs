using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 可释放对象组。
    /// </summary>
    public sealed class DisposableGroup : IDisposable
    {
        private readonly List<IDisposable> m_DisposableList = new List<IDisposable>();

        /// <summary>
        /// 添加可释放对象。
        /// </summary>
        /// <param name="disposable">可释放对象。</param>
        /// <exception cref="ArgumentNullException">disposable 为 null。</exception>
        public void Add(IDisposable disposable)
        {
            if (disposable == null)
            {
                throw new ArgumentNullException(nameof(disposable));
            }

            m_DisposableList.Add(disposable);
        }

        /// <summary>
        /// 释放所有可释放对象。单个对象释放异常不中断其余对象的释放。
        /// </summary>
        public void Dispose()
        {
            try
            {
                foreach (var disposable in m_DisposableList)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch (Exception e)
                    {
                        Log.Error($"DisposableGroup dispose error: {e.Message}\n{e.StackTrace}");
                    }
                }
            }
            finally
            {
                m_DisposableList.Clear();
            }
        }
    }
}
