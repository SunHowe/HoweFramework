using ServerProtocol;

namespace IGrains;

/// <summary>
/// 会话Grain, 用于游戏服务器发送消息给客户端.
/// </summary>
public interface ISessionGrain : IGrainWithGuidKey
{
    /// <summary>
    /// 往客户端发送协议包.
    /// </summary>
    Task Send(ServerPackage package);
}