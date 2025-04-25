using MemoryPack;

namespace Protocol
{
    /// <summary>
    /// 心跳包。
    /// </summary>
    [MemoryPackable]
    public partial class Heartbeat : ProtocolBase
    {
        /// <summary>
        /// 服务器的Unix时间戳(秒)。
        /// </summary>
        public long UnixTime { get; set; }
    }
}