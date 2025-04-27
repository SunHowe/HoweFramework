using System;
using System.Collections.Concurrent;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 可释放的信号量。
    /// </summary>
    public sealed class DisposableSemaphoreSlim : IDisposable
    {
        private readonly SemaphoreSlim m_Semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// 等待信号量。结合using使用。
        /// </summary>
        public async UniTask<IDisposable> WaitAsync()
        {
            await m_Semaphore.WaitAsync();
            return this;
        }

        public void Dispose()
        {
            m_Semaphore.Release();
        }

        /// <summary>
        /// 全局信号量字典。
        /// </summary>
        private static readonly ConcurrentDictionary<int, DisposableSemaphoreSlim> s_DisposableSemaphoreSlimMap = new();

        /// <summary>
        /// 等待全局指定Key的信号量。结合using使用。
        /// </summary>
        /// <param name="key">信号量Key。</param>
        /// <returns>信号量。</returns>
        public static UniTask<IDisposable> WaitAsync(int key)
        {
            if (!s_DisposableSemaphoreSlimMap.TryGetValue(key, out var disposableSemaphoreSlim))
            {
                disposableSemaphoreSlim = new DisposableSemaphoreSlim();
                s_DisposableSemaphoreSlimMap.TryAdd(key, disposableSemaphoreSlim);
            }

            return disposableSemaphoreSlim.WaitAsync();
        }
    }
}
