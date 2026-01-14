
using Geek.Server.App.Common;
using Geek.Server.App.Common.Event;
using Geek.Server.App.Common.Session;
using Geek.Server.App.Logic.Role.Base;
using Geek.Server.Core.Actors;
using Geek.Server.Core.Events;
using Geek.Server.Core.Hotfix.Agent;
using Geek.Server.Core.Timer;
using Server.Logic.Common.Handler;
using Server.Logic.Logic.Role.Bag;
using Server.Logic.Logic.Server;

namespace Server.Logic.Logic.Role.Base
{
    public class RoleCompAgent : StateCompAgent<RoleComp, RoleState>, ICrossDay
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();


        [Event(EventID.SessionRemove)]
        private class EL : EventListener<RoleCompAgent>
        {
            protected override Task HandleEvent(RoleCompAgent agent, Event evt)
            {
                return agent.OnLogout();
            }
        }

        [Service]
        public virtual async Task OnLogin(bool isNewRole, LoginResp resp)
        {
            SetAutoRecycle(false);
            if (isNewRole)
            {
                State.CreateTime = DateTime.Now;
                State.Level = 1;
                State.VipLevel = 1;
                State.RoleName = new Random().Next(1000, 10000).ToString();//随机给一个
            }
            
            State.LoginTime = DateTime.Now;

            // 填写基础信息。
            resp.UserInfo = new UserInfo()
            {
                CreateTime = State.CreateTime.Ticks,
                Level = State.Level,
                RoleId = State.RoleId,
                RoleName = State.RoleName,
                VipLevel = State.VipLevel
            };
            
            // 激活各组件填写上线数据推送。
            await (await GetCompAgent<BagCompAgent>()).OnLogin(isNewRole, resp);
        }

        public async Task OnLogout()
        {
            //移除在线玩家
            var serverComp = await ActorMgr.GetCompAgent<ServerCompAgent>();
            await serverComp.RemoveOnlineRole(ActorId);
            //下线后会被自动回收
            SetAutoRecycle(true);
            QuartzTimer.Unschedule(ScheduleIdSet);
        }

        Task ICrossDay.OnCrossDay(int openServerDay)
        {
            return Task.CompletedTask;
        }

        public void NotifyClient(Message msg, int uniId = 0, ServerErrorCode code = ServerErrorCode.Success)
        {
            var session = SessionManager.Get(ActorId);
            session?.Channel?.Write(msg, uniId, code);
        }
    }
}
