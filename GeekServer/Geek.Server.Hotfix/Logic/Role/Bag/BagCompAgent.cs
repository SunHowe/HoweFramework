
using Geek.Server.App.Common.Event;
using Geek.Server.App.Logic.Role.Bag;
using Geek.Server.Core.Hotfix.Agent;
using Geek.Server.Core.Utils;
using Server.Logic.Common.Events;
using Server.Logic.Common.Extension;

namespace Server.Logic.Logic.Role.Bag
{
    public class BagCompAgent : StateCompAgent<BagComp, BagState>
    {
        private readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        public override void Active()
        {
        }

        /// <summary>
        /// 登录成功回调。
        /// </summary>
        public Task OnLogin(bool isNewRole, LoginResp resp)
        {
            if (isNewRole)
            {
                State.ItemMap.Add(101, 1);
                State.ItemMap.Add(103, 100);
            }
            
            resp.BagInfo = new UserBagInfo
            {
                ItemDic = new Dictionary<int, long>(State.ItemMap),
            };
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// 宠物合成
        /// </summary>
        /// <returns></returns>
        public Task ComposePet(BagComposePetReq req, BagComposePetResp resp)
        {
            const int requiredCount = 10;
            if (!State.ItemMap.TryGetValue(req.FragmentId, out var count) || count < requiredCount)
            {
                // 物品不足
                throw new ServerErrorCodeException(ServerErrorCode.Bag_ItemNotEnough);
            }
            
            State.ItemMap[req.FragmentId] -= requiredCount;
            // 推送背包数据变动。
            this.NotifyClient(new UserBagInfo
            {
                ItemDic = new Dictionary<int, long>
                {
                    { req.FragmentId, State.ItemMap[req.FragmentId] }
                }
            });
            
            //宠物碎片合成相关逻辑
            //.....
            //.....

            //合成成功后分发一个获得宠物的事件(在PetCompAgent中监听此事件)
            this.Dispatch(EventID.GotNewPet, new OneParam<int>(1000));

            resp.PetId = 1000;
            return Task.CompletedTask;
        }
    }
}
