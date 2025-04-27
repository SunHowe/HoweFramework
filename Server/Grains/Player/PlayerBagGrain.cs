using IGrains;
using Protocol;

namespace Grains.Player;

/// <summary>
/// 玩家背包模块。
/// </summary>
public class PlayerBagGrain : PlayerFeatureGrainBase, IPlayerBagGrain
{
    /// <summary>
    /// 登录成功。
    /// </summary>
    public override async Task OnLoginSuccess()
    {
        var bagPush = BagPush.Create(new Dictionary<int, long>()
        {
            { 1, 1000 },
            { 2, 1000 },
        });

        await Send(bagPush);
    }
}