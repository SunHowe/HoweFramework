using IGrains;
using Protocol;
using ServerProtocol;

namespace Grains;

public static class UserSessionGrainExtensions
{
    /// <summary>
    /// 发送协议包给客户端.
    /// </summary>
    public static Task Send(this IPlayerSessionGrain grain, IProtocol protocol)
    {
        return grain.Send(ServerPackageHelper.Pack(protocol));
    }

    /// <summary>
    /// 发送协议响应包给客户端。
    /// </summary>
    public static Task SendResponse(this IPlayerSessionGrain grain, int rpcId, int errorCode)
    {
        var serverPackage = new ServerPackage
        {
            RpcId = rpcId,
            ErrorCode = errorCode,
            ProtocolId = (ushort)ProtocolId.CommonResp,
        };
        
        return grain.Send(serverPackage);
    }

    /// <summary>
    /// 发送协议响应包给客户端。
    /// </summary>
    public static Task SendResponse(this IPlayerSessionGrain sessionGrain, int rpcId, IProtocolResponse response)
    {
        var serverPackage = ServerPackageHelper.Pack(response);
        serverPackage.RpcId = rpcId;
        serverPackage.ErrorCode = response.ErrorCode;
        
        return sessionGrain.Send(serverPackage);
    }
}