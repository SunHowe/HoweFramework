using System.Net;
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
        /// <param name="networkChannel">网络频道。若未指定则使用默认网络频道。</param>
        /// <returns>响应。</returns>
        public static UniTask<IResponse> SendPacketAsync(this Packet packet, INetworkChannel networkChannel = null)
        {
            return NetworkPacketRequest.Create(packet, networkChannel).Execute();
        }

        /// <summary>
        /// 发送协议包，并等待响应。
        /// </summary>
        /// <typeparam name="T">响应类型。</typeparam>
        /// <param name="packet">协议包。</param>
        /// <param name="networkChannel">网络频道。若未指定则使用默认网络频道。</param>
        /// <returns>响应。</returns>
        public static UniTask<T> SendPacketAsync<T>(this Packet packet, INetworkChannel networkChannel = null) where T : class, IResponse, new()
        {
            return NetworkPacketRequest.Create(packet, networkChannel).Execute().As<T>();
        }

        /// <summary>
        /// 连接网络频道。
        /// </summary>
        /// <param name="networkModule">网络模块。</param>
        /// <param name="address">连接地址。</param>
        /// <param name="port">连接端口。</param>
        /// <param name="networkChannel">网络频道。若未指定则使用默认网络频道。</param>
        /// <returns>响应。</returns>
        public static UniTask<IResponse> ConnectAsync(this NetworkModule networkModule, IPAddress address, int port, INetworkChannel networkChannel = null)
        {
            return NetworkChannelConnectRequest.Create(address, port, networkChannel).Execute();
        }

        /// <summary>
        /// 连接网络频道。
        /// </summary>
        /// <param name="networkModule">网络模块。</param>
        /// <param name="address">连接地址。</param>
        /// <param name="port">连接端口。</param>
        /// <param name="networkChannel">网络频道。若未指定则使用默认网络频道。</param>
        /// <returns>响应。</returns>
        public static UniTask<IResponse> ConnectAsync(this NetworkModule networkModule, string address, int port, INetworkChannel networkChannel = null)
        {
            return ConnectAsync(networkModule, IPAddress.Parse(address), port, networkChannel);
        }

        /// <summary>
        /// 连接网络频道。
        /// </summary>
        /// <param name="networkChannel">网络频道。</param>
        /// <param name="address">连接地址。</param>
        /// <param name="port">连接端口。</param>
        /// <returns>响应。</returns>
        public static UniTask<IResponse> ConnectAsync(this INetworkChannel networkChannel, IPAddress address, int port)
        {
            return NetworkChannelConnectRequest.Create(address, port, networkChannel).Execute();
        }

        /// <summary>
        /// 连接网络频道。
        /// </summary>
        /// <param name="networkChannel">网络频道。</param>
        /// <param name="address">连接地址。</param>
        /// <param name="port">连接端口。</param>
        /// <returns>响应。</returns>
        public static UniTask<IResponse> ConnectAsync(this INetworkChannel networkChannel, string address, int port)
        {
            return ConnectAsync(networkChannel, IPAddress.Parse(address), port);
        }
    }
}
