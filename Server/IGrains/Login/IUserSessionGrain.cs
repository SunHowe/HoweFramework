using ServerProtocol;

namespace IGrains;

/// <summary>
/// 玩家会话Grain, 用于对应玩家的Actor实例, 它将负责玩家Actor到网关服务器的交互.
/// </summary>
public interface IUserSessionGrain : IGrainWithGuidKey
{
    /// <summary>
    /// 登录成功回调. 用于与客户端会话实例建立联系.
    /// </summary>
    Task OnLogin(Guid sessionId);

    /// <summary>
    /// 发送消息给网关服务器.
    /// </summary>
    Task Send(ServerPackage package);
}
