using System.Collections.Generic;
using YooAsset;

namespace HoweFramework
{
    /// <summary>
    /// YooAsset资源管线远程路径服务.
    /// </summary>
    public sealed class YooAssetRemoteServices : IRemoteService
    {
        private readonly string m_DefaultHostServer;
        private readonly string m_FallbackHostServer;

        public YooAssetRemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            m_DefaultHostServer = defaultHostServer;
            m_FallbackHostServer = fallbackHostServer;
        }

        public IReadOnlyList<string> GetRemoteUrls(string fileName)
        {
            return new List<string>{
                $"{m_DefaultHostServer}/{fileName}",
                $"{m_FallbackHostServer}/{fileName}"
            };
        }
    }
}