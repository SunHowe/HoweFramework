using IGrains;
using Protocol;
using ServerProtocol;

namespace Grains;

/// <summary>
/// 消息接收Grain, 用于接收客户端的消息.
/// </summary>
public class ReceiverGrain : Grain, IReceiverGrain
{
    /// <summary>
    /// 是否已经登录成功.
    /// </summary>
    private bool m_IsLoginSucess;

    /// <summary>
    /// 玩家的唯一id.
    /// </summary>
    private Guid m_UserId;

    public Task OnGatewayConnected()
    {
        return Task.CompletedTask;
    }

    public Task OnGatewayDisconnected()
    {
        return Task.CompletedTask;
    }

    public async Task OnReceive(ServerPackage package)
    {
        if (package.ProtocolId == (ushort)ProtocolId.LoginRequest)
        {
            var sessionGrain = GrainFactory.GetGrain<ISessionGrain>(this.GetPrimaryKey());

            if (m_IsLoginSucess)
            {
                // 重复登录.
                await sessionGrain.SendResponse(package.RpcId, HoweFramework.ErrorCode.LoginDuplicate);
                return;
            }

            try
            {
                // 处理登录请求.
                var request = package.Unpack<LoginRequest>();
                var loginGrain = GrainFactory.GetGrain<ILoginGrain>(Guid.NewGuid());
                var userId = await loginGrain.Login(request!.Account!, request!.Password!);

                m_IsLoginSucess = true;
                m_UserId = userId;

                // 登录成功, 激活IPlayerSessionGrain和IPlayerGrain.
                await GrainFactory.GetGrain<IPlayerSessionGrain>(userId).OnLogin(this.GetPrimaryKey());
                await GrainFactory.GetGrain<IPlayerGrain>(userId).OnLogin();
                await sessionGrain.SendResponse(package.RpcId, LoginResponse.Create(userId));
            }
            catch (GameException e)
            {
                m_IsLoginSucess = false;
                m_UserId = Guid.Empty;
                Console.WriteLine(e);
                await sessionGrain.SendResponse(package.RpcId, e.ErrorCode);
            }
            catch (Exception e)
            {
                m_IsLoginSucess = false;
                m_UserId = Guid.Empty;
                Console.WriteLine(e);
                await sessionGrain.SendResponse(package.RpcId, HoweFramework.ErrorCode.Exception);
            }

            return;
        }

        // 处理其他请求.
        if (!m_IsLoginSucess)
        {
            // 未登录成功不允许请求其他包.
            var sessionGrain = GrainFactory.GetGrain<ISessionGrain>(this.GetPrimaryKey());
            await sessionGrain.SendResponse(package.RpcId, HoweFramework.ErrorCode.NoLogin);
            return;
        }

        // 转发给UserGrain进行处理.
        var userGrain = GrainFactory.GetGrain<IPlayerGrain>(m_UserId);
        await userGrain.OnReceive(package);
    }

    
}