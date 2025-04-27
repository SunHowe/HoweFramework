using IGrains;
using ServerProtocol;

namespace Grains;

/// <summary>
/// 玩家会话Grain, 用于对应玩家的Actor实例, 它将负责玩家Actor到网关服务器的交互.
/// </summary>
public class PlayerSessionGrain : Grain, IPlayerSessionGrain
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