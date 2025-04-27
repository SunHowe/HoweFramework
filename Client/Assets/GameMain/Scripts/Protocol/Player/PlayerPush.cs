using System;
using MemoryPack;

namespace Protocol
{
    /// <summary>
    /// 玩家信息推送消息包。
    /// </summary>
    [MemoryPackable]
    public partial class PlayerPush : ProtocolBase
    {
        /// <summary>
        /// 玩家Id。
        /// </summary>
        public Guid PlayerId { get; set; }

        /// <summary>
        /// 玩家名称。
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// 登录时间。
        /// </summary>
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// 创建时间。
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}

