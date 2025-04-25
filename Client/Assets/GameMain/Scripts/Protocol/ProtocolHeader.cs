using System.Runtime.InteropServices;

namespace Protocol
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RequestHeader
    {
        public ushort ProtocolId;
        public ushort BodyLength;
        
        public int RpcId;
        public int MagicNumber;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ResponseHeader
    {
        public ushort ProtocolId;
        public ushort BodyLength;
        
        public int RpcId;
        public int ErrorCode;
    }
}