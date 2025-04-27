using System;
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
    }
}
