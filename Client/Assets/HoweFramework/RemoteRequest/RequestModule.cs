using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 远程请求模块。
    /// </summary>
    public sealed class RemoteRequestModule : ModuleBase<RemoteRequestModule>
    {
        private readonly List<RemoteRequestDispatcher> m_Dispatchers = new();

        /// <summary>
        /// 创建一个远程请求调度器。
        /// </summary>
        /// <returns>远程请求调度器。</returns>
        public IRemoteRequestDispatcher CreateRemoteRequestDispatcher()
        {
            var dispatcher = ReferencePool.Acquire<RemoteRequestDispatcher>();
            m_Dispatchers.Add(dispatcher);
            return dispatcher;
        }

        /// <summary>
        /// 调度器 Dispose 时从扫描列表移除。
        /// </summary>
        internal void UnregisterDispatcher(RemoteRequestDispatcher dispatcher)
        {
            m_Dispatchers.Remove(dispatcher);
        }

        protected override void OnInit()
        {
        }

        protected override void OnDestroy()
        {
            m_Dispatchers.Clear();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (m_Dispatchers.Count == 0)
            {
                return;
            }

            using var snapshot = ReusableList<RemoteRequestDispatcher>.Create();
            for (int i = 0; i < m_Dispatchers.Count; i++)
            {
                snapshot.Add(m_Dispatchers[i]);
            }

            foreach (var dispatcher in snapshot)
            {
                dispatcher.ScanTimeoutRequests();
            }
        }
    }
}
