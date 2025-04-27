using Grains.Extensions;
using GrainStates.Player;
using IGrains;
using Protocol;

namespace Grains.Player;

/// <summary>
/// 玩家背包模块。
/// </summary>
public class PlayerBagGrain : PlayerFeatureGrainBase, IPlayerBagGrain
{
    private readonly IPersistentState<BagState> m_BagState;

    public PlayerBagGrain(
        [PersistentState(BagState.StorageName)]
        IPersistentState<BagState> bagState)
    {
        m_BagState = bagState;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);
        await m_BagState.ReadStateAsync(cancellationToken);
        
        m_BagState.State.ItemDict ??= new Dictionary<int, long>();
    }

    /// <summary>
    /// 登录成功。
    /// </summary>
    public override async Task OnLoginSuccess()
    {
        var bagPush = BagPush.Create(m_BagState.State.ItemDict.Clone());

        await Send(bagPush);

        // 测试，启动一个定时器，每5s发送一次数据给客户端。
        this.RegisterGrainTimer(OnTimerInvoke, TimeSpan.Zero, TimeSpan.FromSeconds(5));
    }

    private Task OnTimerInvoke()
    {
        var bagPush = BagPush.Create(m_BagState.State.ItemDict.Clone());

        return Send(bagPush);
    }
}