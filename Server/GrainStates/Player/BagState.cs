using System;
using System.Collections.Generic;

namespace GrainStates.Player;

[Serializable]
public class BagState
{
    public const string StorageName = "PlayerBag";
    
    /// <summary>
    /// 背包道具数量。
    /// </summary>
    public Dictionary<int, long> ItemDict { get; set; }
}