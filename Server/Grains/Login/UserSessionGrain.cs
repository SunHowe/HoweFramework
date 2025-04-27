using IGrains;
using Protocol;
using ServerProtocol;

namespace Grains;

/// <summary>
/// 玩家会话Grain, 用于对应玩家的Actor实例, 它将负责玩家Actor到网关服务器的交互.
/// </summary>
public class UserSessionGrain : Grain, IUserSessionGrain
{
    private Guid? m_SessionId;
    
    public Task OnLogin(Guid sessionId)
    {
        m_SessionId = sessionId;
        return Task.CompletedTask;
    }

    public async Task Send(ServerPackage package)
    {
        if (m_SessionId == null)
        {
            Console.WriteLine("SessionId is null");
            return;
        }

        await GrainFactory.GetGrain<ISessionGrain>(m_SessionId.Value).Send(package);
    }
}

public static class UserSessionGrainExtensions
{
    /// <summary>
    /// 发送协议包给客户端.
    /// </summary>
    public static Task Send(this IUserSessionGrain grain, IProtocol protocol)
    {
        return grain.Send(ServerPackageHelper.Pack(protocol));
    }

    /// <summary>
    /// 发送协议响应包给客户端。
    /// </summary>
    public static Task SendResponse(this IUserSessionGrain grain, int rpcId, int errorCode)
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
    public static Task SendResponse(this IUserSessionGrain sessionGrain, int rpcId, IProtocolResponse response)
    {
        var serverPackage = ServerPackageHelper.Pack(response);
        serverPackage.RpcId = rpcId;
        serverPackage.ErrorCode = response.ErrorCode;
        
        return sessionGrain.Send(serverPackage);
    }
}