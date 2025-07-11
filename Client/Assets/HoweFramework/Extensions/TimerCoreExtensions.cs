using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// TimerCore扩展。
    /// </summary>
    public static class TimerCoreExtensions
    {
        /// <summary>
        /// 通过定时器驱动的异步等待。
        /// </summary>
        /// <param name="core">定时器核心接口。</param>
        /// <param name="delay">等待时间。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>异步任务。</returns>
        public static UniTask Delay(this ITimerCore core, float delay, CancellationToken token = default)
        {
            return TimerUniTaskCompletionSource.CreateUniTask(core, delay, token);
        }

        /// <summary>
        /// 通过定时器驱动的异步等待。
        /// </summary>
        private sealed class TimerUniTaskCompletionSource : IDisposable, IReference
        {
            private int m_TimerId;
            private ITimerCore m_TimerCore;
            private CancellationToken m_CancellationToken;
            private AutoResetUniTaskCompletionSource m_UniTaskCompletionSource;

            public void Dispose()
            {
                ReferencePool.Release(this);
            }

            public void Clear()
            {
                m_TimerId = 0;
                m_TimerCore = null;
                m_CancellationToken = CancellationToken.None;
                m_UniTaskCompletionSource = null;
            }

            private void TrySetResult()
            {
                if (m_TimerId == 0)
                {
                    return;
                }

                var completionSource = m_UniTaskCompletionSource;

                Dispose();

                completionSource.TrySetResult();
            }

            private void TrySetCanceled(object param)
            {
                if (param is not int timerId)
                {
                    return;
                }

                if (timerId != m_TimerId)
                {
                    return;
                }

                var completionSource = m_UniTaskCompletionSource;
                var token = m_CancellationToken;

                m_TimerCore.RemoveTimer(m_TimerId);
                Dispose();

                completionSource.TrySetCanceled(token);
            }

            /// <summary>
            /// 创建一个定时器驱动的异步等待。
            /// </summary>
            public static UniTask CreateUniTask(ITimerCore timerCore, float delay, CancellationToken token)
            {
                if (token.IsCancellationRequested)
                {
                    return UniTask.FromCanceled(token);
                }

                var source = ReferencePool.Acquire<TimerUniTaskCompletionSource>();

                var timerId = timerCore.AddTimer(delay, 1, source.TrySetResult);
                token.Register(source.TrySetCanceled, timerId);

                source.m_TimerId = timerId;
                source.m_TimerCore = timerCore;
                source.m_CancellationToken = token;
                source.m_UniTaskCompletionSource = AutoResetUniTaskCompletionSource.Create();

                return source.m_UniTaskCompletionSource.Task;
            }
        }
    }
}