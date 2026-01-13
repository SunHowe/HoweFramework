using Geek.Server.App.Common.Session;

namespace Server.Logic.Common.Helper;

public static class SessionHelper
{
    /// <summary>
    /// 通知客户端。
    /// </summary>
    /// <param name="actorId">角色id。</param>
    /// <param name="msg">消息。</param>
    public static void NotifyClient(long actorId, Message msg)
    {
        var session = SessionManager.Get(actorId);
        session?.Channel?.Write(msg);
    }

    /// <summary>
    /// 通知多个客户端。
    /// </summary>
    /// <param name="actorIds">角色id集合。</param>
    /// <param name="msg">消息。</param>
    public static void NotifyClients(ICollection<long> actorIds, Message msg)
    {
        foreach (var actorId in actorIds)
        {
            NotifyClient(actorId, msg);
        }
    }

    /// <summary>
    /// 通知多个客户端，排除指定角色。  
    /// </summary>
    /// <param name="actorIds">角色id集合。</param>
    /// <param name="msg">消息。</param>
    /// <param name="exceptActorId">排除的角色id。</param>
    public static void NotifyClientsExcept(ICollection<long> actorIds, Message msg, long exceptActorId)
    {
        foreach (var actorId in actorIds)
        {
            if (actorId != exceptActorId)
            {
                NotifyClient(actorId, msg);
            }
        }
    }
}