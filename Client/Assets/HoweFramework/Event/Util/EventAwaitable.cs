using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 事件等待器。
    /// </summary>
    public sealed class EventAwaitable : IDisposable, IReference
    {
        private static int s_ReferenceId = 0;

        private int m_EventId;
        private IEventSubscribe m_EventSubscribe;
        private AutoResetUniTaskCompletionSource<bool> m_UniTaskCompletionSource;
        private CancellationTokenRegistration m_Registration;
        private UniTask<bool> m_UniTask;
        private int m_ReferenceId = 0;

        public void Clear()
        {
            m_EventId = 0;
            m_EventSubscribe = null;
            m_UniTaskCompletionSource = null;
            m_Registration = default;
            m_ReferenceId = 0;
            m_UniTask = default;
        }

        public void Dispose()
        {
            if (m_ReferenceId == 0)
            {
                return;
            }

            m_EventSubscribe.Unsubscribe(m_EventId, OnEvent);

            // 注销取消回调，避免长寿命 token 源上累积指向已回收对象的注册项。
            m_Registration.Dispose();
            m_Registration = default;

            ReferencePool.Release(this);
        }

        private void OnEvent(object sender, GameEventArgs e)
        {
            if (m_ReferenceId == 0)
            {
                return;
            }

            m_UniTaskCompletionSource.TrySetResult(true);
        }

        public UniTask<bool> WaitAsync()
        {
            return m_UniTask;
        }

        /// <summary>
        /// 创建事件等待器。
        /// </summary>
        /// <param name="eventId">事件Id。</param>
        /// <param name="eventDispatcher">事件调度器。若未指定则使用默认事件调度器。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>事件等待器。</returns>
        public static EventAwaitable Create(int eventId, IEventSubscribe eventSubscribe = null, CancellationToken token = default)
        {
            eventSubscribe ??= EventModule.Instance.EventDispatcher;

            var eventAwaitable = ReferencePool.Acquire<EventAwaitable>();
            eventAwaitable.m_EventId = eventId;
            eventAwaitable.m_EventSubscribe = eventSubscribe;
            eventAwaitable.m_UniTaskCompletionSource = AutoResetUniTaskCompletionSource<bool>.Create();
            eventAwaitable.m_ReferenceId = Interlocked.Increment(ref s_ReferenceId);
            eventAwaitable.m_UniTask = eventAwaitable.m_UniTaskCompletionSource.Task;

            eventSubscribe.Subscribe(eventId, eventAwaitable.OnEvent);

            if (token != default)
            {
                // 保存注册句柄，Dispose 时注销。
                eventAwaitable.m_Registration = token.Register(eventAwaitable.OnTokenCancel, eventAwaitable.m_ReferenceId);
            }

            return eventAwaitable;
        }

        /// <summary>
        /// 等待事件。
        /// </summary>
        /// <param name="eventId">事件Id。</param>
        /// <param name="eventDispatcher">事件调度器。若未指定则使用默认事件调度器。</param>
        /// <param name="token">取消令牌。</param>
        public static async UniTask<bool> WaitAsync(int eventId, IEventSubscribe eventSubscribe = null, CancellationToken token = default)
        {
            using var eventAwaitable = Create(eventId, eventSubscribe, token);
            return await eventAwaitable.WaitAsync();
        }

        private void OnTokenCancel(object state)
        {
            if (state is int referenceId && referenceId == m_ReferenceId)
            {
                m_UniTaskCompletionSource.TrySetResult(false);
            }
        }
    }
}
