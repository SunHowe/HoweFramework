using IGrains;
using Protocol;
using ServerProtocol;

namespace Grains;

/// <summary>
/// 玩家Grain, 用于对应玩家的Actor实例, 它将负责网关服务器到玩家Actor的交互.
/// </summary>
public class UserGrain : Grain, IUserGrain
{
    public Task OnLogin(Guid sessionId)
    {
        return Task.CompletedTask;
    }

    public async Task OnReceive(ServerPackage package)
    {
        var handler = ProtocolHandlerManager.Get(package.ProtocolId);
        var sessionGrain = GrainFactory.GetGrain<IUserSessionGrain>(this.GetPrimaryKey());
        if (handler == null)
        {
            // 无协议处理器.
            Console.WriteLine($"No handler for protocol: {package.ProtocolId} rpcId={package.RpcId}");
            await SendResponse(sessionGrain, package.RpcId, GameErrorCode.NoHandler);
            return;
        }

        var protocol = package.Unpack();
        if (protocol == null)
        {
            // 协议解析失败.
            Console.WriteLine($"Failed to unpack protocol: {package.ProtocolId} rpcId={package.RpcId}");
            await SendResponse(sessionGrain, package.RpcId, GameErrorCode.ProtocolUnpackError);
            return;
        }

        var response = await handler.Handle(sessionGrain, protocol);
        await SendResponse(sessionGrain, package.RpcId, response);
    }

    private static async Task SendResponse(IUserSessionGrain sessionGrain, int rpcId, int errorCode)
    {
        var serverPackage = new ServerPackage
        {
            RpcId = rpcId,
            ErrorCode = errorCode,
            ProtocolId = (ushort)ProtocolId.CommonResp,
        };
        
        await sessionGrain.Send(serverPackage);
    }

    private static async Task SendResponse(IUserSessionGrain sessionGrain, int rpcId, IProtocolResponse response)
    {
        var serverPackage = ServerPackageHelper.Pack(response);
        serverPackage.RpcId = rpcId;
        serverPackage.ErrorCode = response.ErrorCode;
        
        await sessionGrain.Send(serverPackage);
    }
}