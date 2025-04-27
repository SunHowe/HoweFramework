using IGrains;
using Protocol;
using ServerProtocol;

namespace Grains;

/// <summary>
/// 玩家Grain, 用于对应玩家的Actor实例, 它将负责网关服务器到玩家Actor的交互.
/// </summary>
public class UserGrain : Grain, IUserGrain
{
    public async Task OnLogin(Guid sessionId)
    {
        // 触发各模块的登录成功事件.
    }

    public async Task OnReceive(ServerPackage package)
    {
        var handler = ProtocolHandlerManager.Get(package.ProtocolId);
        var sessionGrain = GrainFactory.GetGrain<IUserSessionGrain>(this.GetPrimaryKey());
        if (handler == null)
        {
            // 无协议处理器.
            Console.WriteLine($"No handler for protocol: {package.ProtocolId} rpcId={package.RpcId}");
            await sessionGrain.SendResponse(package.RpcId, HoweFramework.ErrorCode.NetworkNotSupportPacket);
            return;
        }

        var protocol = package.Unpack();
        if (protocol == null)
        {
            // 协议解析失败.
            Console.WriteLine($"Failed to unpack protocol: {package.ProtocolId} rpcId={package.RpcId}");
            await sessionGrain.SendResponse(package.RpcId, HoweFramework.ErrorCode.NetworkDeserializePacketError);
            return;
        }

        var response = await handler.Handle(sessionGrain, protocol);
        await sessionGrain.SendResponse(package.RpcId, response);
    }
}