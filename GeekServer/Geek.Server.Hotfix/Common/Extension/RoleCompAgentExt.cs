using Geek.Server.Core.Actors;
using Geek.Server.Core.Hotfix.Agent;
using Server.Logic.Common.Helper;

namespace Server.Logic.Common.Extension;

public static class RoleCompAgentExt
{
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();
    
    /// <summary>
    /// 通知客户端。
    /// </summary>
    public static void NotifyClient(this ICompAgent agent, Message msg)
    {
        if (agent.OwnerType != ActorType.Role)
        {
            Log.Warn("仅支持RoleComp组件使用这种方式通知客户端，其他的请使用SessionHelper.NotifyClient");
            return;
        }
            
        SessionHelper.NotifyClient(agent.ActorId, msg);
    }
}