using Grains.Extensions;
using GrainStates.Player;
using IGrains;
using Protocol;

namespace Grains.Player;

/// <summary>
/// 玩家背包模块。
/// </summary>
public class PlayerBagGrain : PlayerFeatureGrain<PlayerBagState>, IPlayerBagGrain
{
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);
        await ReadStateAsync();
        
        State.ItemDict ??= new Dictionary<int, long>();
    }

    /// <summary>
    /// 登录成功。
    /// </summary>
    public override async Task OnLoginSuccess()
    {
        await Send(BagPush.Create(State.ItemDict.Clone()));
    }
}