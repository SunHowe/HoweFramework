using System;
using System.Collections.Generic;

namespace GrainStates.Player;

/// <summary>
/// 玩家背包状态。
/// </summary>
[Serializable]
public class PlayerBagState
{
    /// <summary>
    /// 背包道具数量。
    /// </summary>
    public Dictionary<int, long> ItemDict { get; set; }
}