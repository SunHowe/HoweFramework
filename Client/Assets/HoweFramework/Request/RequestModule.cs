using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 请求模块，为远程交互请求提供统一的注册功能。
    /// </summary>
    public sealed class RequestModule : ModuleBase<RequestModule>
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