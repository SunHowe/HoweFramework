namespace IGrains;

/// <summary>
/// 玩家基础信息模块。
/// </summary>
public interface IPlayerBasicGrain : IPlayerFeatureGrain
{
    /// <summary>
    /// 修改玩家名称。
    /// </summary>
    Task ModifyPlayerName(string name);
}