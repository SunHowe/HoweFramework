
using Geek.Server.Core.Net.BaseHandler;

namespace Server.Logic.Logic.Login
{
    [MsgMapping(typeof(LoginReq))]
    internal class ReqLoginHandler : GlobalCompReqHandler<LoginCompAgent, LoginReq, LoginResp>
    {
        protected override Task OnHandleRequest(LoginReq req, LoginResp resp)
        {
            return Comp.OnLogin(Channel, req, resp);
        }
    }
}
