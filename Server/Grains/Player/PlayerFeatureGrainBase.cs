using IGrains;
using Protocol;

namespace Grains.Player;

/// <summary>
/// 玩家功能模块抽象类。
/// </summary>
public abstract class PlayerFeatureGrainBase : Grain, IPlayerFeatureGrain
{
    public abstract Task OnLoginSuccess();
    
    /// <summary>
    /// 发送协议包给客户端.
    /// </summary>
    public Task Send(IProtocol protocol)
    {
        return GrainFactory.GetGrain<IPlayerSessionGrain>(this.GetPrimaryKey()).Send(protocol);
    }

    /// <summary>
    /// 发送协议响应包给客户端。
    /// </summary>
    public Task SendResponse(int rpcId, int errorCode)
    {
        return GrainFactory.GetGrain<IPlayerSessionGrain>(this.GetPrimaryKey()).SendResponse(rpcId, errorCode);
    }

    /// <summary>
    /// 发送协议响应包给客户端。
    /// </summary>
    public Task SendResponse(int rpcId, IProtocolResponse response)
    {
        return GrainFactory.GetGrain<IPlayerSessionGrain>(this.GetPrimaryKey()).SendResponse(rpcId, response);
    }
}