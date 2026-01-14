using Geek.Server.Core.Net.BaseHandler;

namespace Server.Logic.Logic.Role.Bag
{
    [MsgMapping(typeof(BagComposePetReq))]
    public class ReqComposePetHandler : RoleCompReqHandler<BagCompAgent, BagComposePetReq, BagComposePetResp>
    {
        protected override Task OnHandleRequest(BagComposePetReq req, BagComposePetResp resp)
        {
            return Comp.ComposePet(req, resp);
        }
    }
}
