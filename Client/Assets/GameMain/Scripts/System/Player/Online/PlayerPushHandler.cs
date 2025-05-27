using HoweFramework;
using Protocol;

namespace GameMain
{
    /// <summary>
    /// 玩家信息推送消息包处理器。
    /// </summary>
    public sealed class PlayerPushHandler : OrleansPacketHandler<PlayerPush>
    {
        protected override void Handle(INetworkChannel networkChannel, PlayerPush packet)
        {
            PlayerSystem.Instance.PlayerInfo.Value = new PlayerInfo(packet.PlayerId, packet.PlayerName, packet.LoginTime, packet.CreateTime);
        }
    }
}

