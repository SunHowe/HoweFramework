using ServerProtocol;

namespace IGrains;

/// <summary>
/// 玩家Grain, 用于对应玩家的Actor实例, 它将负责网关服务器到玩家Actor的交互.
/// </summary>
public interface IPlayerGrain : IGrainWithGuidKey
{
    /// <summary>
    /// 登录成功回调. 用于与客户端会话实例建立联系.
    /// </summary>
    Task OnLogin();
    
    /// <summary>
    /// 接收到网关服务器的消息.
    /// </summary>
    Task OnReceive(ServerPackage package);
}