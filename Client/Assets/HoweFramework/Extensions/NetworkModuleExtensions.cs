using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 网络模块扩展。
    /// </summary>
    public static class NetworkModuleExtensions
    {
        /// <summary>
        /// 发送协议包，并等待响应。
        /// </summary>
        /// <param name="packet">协议包。</param>
        /// <param name="channelName">网络频道名。</param>
        /// <returns>响应。</returns>
        public static UniTask<IResponse> SendPacketAsync(this Packet packet, string channelName = null)
        {
            return NetworkPacketRequest.Create(packet, channelName).Execute();
        }
    }
}
