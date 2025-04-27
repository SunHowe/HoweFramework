using System;
using MemoryPack;

namespace Protocol
{
    [MemoryPackable]
    public partial class LoginResponse : ProtocolBase, IProtocolResponse
    {
        public Guid UserId { get; set; }
    }
}