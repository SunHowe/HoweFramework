using System.Collections.Generic;
using MemoryPack;

namespace Protocol
{
    /// <summary>
    /// 背包数据推送。
    /// </summary>
    [MemoryPackable]
    public partial class BagPush : ProtocolBase
    {
        /// <summary>
        /// 道具数量字典。
        /// </summary>
        public Dictionary<int, long> ItemDict { get; set; }
    }
}