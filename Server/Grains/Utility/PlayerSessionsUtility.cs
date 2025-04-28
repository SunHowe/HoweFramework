using IGrains;
using Protocol;
using ServerProtocol;

namespace Grains.Utility;

/// <summary>
/// 玩家会话工具。
/// </summary>
public static class PlayerSessionsUtility
{
    /// <summary>
    /// 发送协议到客户端。
    /// </summary>
    /// <param name="grainFactory">Grain工厂。</param>
    /// <param name="playerId">玩家Id。</param>
    /// <param name="protocol">协议。</param>
    public static async Task SendToClient(this IGrainFactory grainFactory, Guid playerId, IProtocol protocol)
    {
        var session = grainFactory.GetGrain<IPlayerSessionGrain>(playerId);
        await session.Send(protocol);
    }

    /// <summary>
    /// 广播协议到客户端。
    /// </summary>
    /// <param name="grainFactory">Grain工厂。</param>
    /// <param name="playerList">玩家列表。</param>
    /// <param name="protocol">协议。</param>
    public static async Task Broadcast(this IGrainFactory grainFactory, List<Guid> playerList, IProtocol protocol)
    {
        var serverPackage = ServerPackageHelper.Pack(protocol);
        var taskBuffer = new List<Task>(playerList.Count);
        foreach (var playerId in playerList)
        {
            var session = grainFactory.GetGrain<IPlayerSessionGrain>(playerId);
            taskBuffer.Add(session.Send(serverPackage));
        }
        
        await Task.WhenAll(taskBuffer);
    }
}
