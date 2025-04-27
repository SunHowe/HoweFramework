namespace HoweFramework
{
    /// <summary>
    /// 远程请求模块。
    /// </summary>
    public sealed class RemoteRequestModule : ModuleBase<RemoteRequestModule>
    {
        /// <summary>
        /// 创建一个远程请求调度器。
        /// </summary>
        /// <returns>远程请求调度器。</returns>
        public IRemoteRequestDispatcher CreateRemoteRequestDispatcher()
        {
            return ReferencePool.Acquire<RemoteRequestDispatcher>();
        }

        protected override void OnInit()
        {
        }

        protected override void OnDestroy()
        {
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}