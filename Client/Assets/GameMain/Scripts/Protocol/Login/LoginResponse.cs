using MemoryPack;

namespace Protocol
{
    [MemoryPackable]
    public partial class LoginResponse : ProtocolBase, IProtocolResponse
    {
        public long UserId { get; set; }
    }
}