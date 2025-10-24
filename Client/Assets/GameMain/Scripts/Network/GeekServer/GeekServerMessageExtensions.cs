using Cysharp.Threading.Tasks;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// GeekServer消息扩展。
    /// </summary>
    public static class GeekServerMessageExtensions
    {
        /// <summary>
        /// 发送协议包，并等待响应。
        /// </summary>
        /// <param name="message">协议包。</param>
        /// <param name="networkChannel">网络频道。若未指定则使用默认网络频道。</param>
        /// <returns>响应。</returns>
        public static async UniTask<IResponse> SendPacketAsync(this Message message, INetworkChannel networkChannel = null)
        {
            var packet = ReferencePool.Acquire<GeekServerRequestPacket>();
            packet.Message = message;
            using var responsePacket = await packet.SendPacketAsync<GeekServerResponsePacket>(networkChannel);
            var responseMessage = responsePacket.Message;
            responsePacket.Message = null;
            return (ResponseMessage)responseMessage;
        }

        /// <summary>
        /// 发送协议包，并等待响应。
        /// </summary>
        /// <typeparam name="T">响应类型。</typeparam>
        /// <param name="message">协议包。</param>
        /// <param name="networkChannel">网络频道。若未指定则使用默认网络频道。</param>
        /// <returns>响应。</returns>
        public static UniTask<T> SendPacketAsync<T>(this Message message, INetworkChannel networkChannel = null) where T : ResponseMessage, new()
        {
            return message.SendPacketAsync(networkChannel).As<T>();
        }
    }
}