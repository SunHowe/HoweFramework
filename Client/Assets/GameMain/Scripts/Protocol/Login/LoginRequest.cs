using MemoryPack;

namespace Protocol
{
    /// <summary>
    /// 登录请求.
    /// </summary>
    [MemoryPackable]
    public partial class LoginRequest : ProtocolBase, IProtocolRequest
    {
        public string Account { get; set; }
        public string Password { get; set; }
    }

}