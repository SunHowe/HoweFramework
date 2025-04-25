using ServerProtocol;

namespace IGrains;

/// <summary>
/// 消息接收Grain, 用于接收客户端的消息.
/// </summary>
public interface IReceiverGrain : IGrainWithGuidKey
{
    /// <summary>
    /// 用于接收客户端的消息.
    /// </summary>
    Task OnReceive(ServerPackage package);

    /// <summary>
    /// 网关建立链接.
    /// </summary>
    Task OnGatewayConnected();

    /// <summary>
    /// 网关断开链接.
    /// </summary>
    Task OnGatewayDisconnected();
}