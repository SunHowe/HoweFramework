using System;

namespace GrainStates.Player;

/// <summary>
/// 玩家基础信息状态。
/// </summary>
[Serializable]
public class PlayerBasicState
{
    /// <summary>
    /// 玩家名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 创建时间。
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 登录时间。
    /// </summary>
    public DateTime LoginTime { get; set; }
}