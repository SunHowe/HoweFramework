using HoweFramework;
using Protocol;

namespace GameMain
{
    /// <summary>
    /// 背包推送消息包处理器。
    /// </summary>
    public sealed class BagPushHandler : OrleansPacketHandler<BagPush>
    {
        protected override void Handle(INetworkChannel networkChannel, BagPush packet)
        {
            Log.Info("收到背包推送消息包");

            foreach (var item in packet.ItemDict)
            {
                Log.Info($"背包物品：{item.Key}，数量：{item.Value}");
            }
        }
    }
}
