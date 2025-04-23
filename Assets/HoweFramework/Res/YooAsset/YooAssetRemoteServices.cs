using YooAsset;

namespace HoweFramework
{
    /// <summary>
    /// YooAsset资源管线远程路径服务.
    /// </summary>
    public sealed class YooAssetRemoteServices : IRemoteServices
    {
        private readonly string m_DefaultHostServer;
        private readonly string m_FallbackHostServer;

        public YooAssetRemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            m_DefaultHostServer = defaultHostServer;
            m_FallbackHostServer = fallbackHostServer;
        }
        
        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            return $"{m_DefaultHostServer}/{fileName}";
        }

        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{m_FallbackHostServer}/{fileName}";
        }
    }
}