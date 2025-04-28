using GrainStates.Player;
using IGrains;
using Protocol;
using ServerProtocol;

namespace Grains.Player;

/// <summary>
/// 玩家基础信息模块。
/// </summary>
public class PlayerBasicGrain : PlayerFeatureGrain<PlayerBasicState>, IPlayerBasicGrain
{
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);
        await ReadStateAsync();
    }

    /// <summary>
    /// 登录成功。
    /// </summary>
    public override async Task OnLoginSuccess()
    {
        if (State.CreateTime == default)
        {
            State.CreateTime = DateTime.Now;
        }

        State.LoginTime = DateTime.Now;

        await WriteStateAsync();
        await Send(PlayerPush.Create(this.GetPrimaryKey(), State.Name, State.CreateTime, State.LoginTime));
    }

    /// <summary>
    /// 修改玩家名称。
    /// </summary>
    public async Task ModifyPlayerName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new GameException(ErrorCode.InvalidParam);
        }

        State.Name = name;
        await WriteStateAsync();
    }
}
