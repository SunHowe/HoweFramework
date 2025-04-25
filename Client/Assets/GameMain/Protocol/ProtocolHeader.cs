using System.Runtime.InteropServices;

namespace Protocol
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RequestHeader
    {
        public ushort ProtocolId;
        public int RpcId;
        public int BodyLength;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ResponseHeader
    {
        public ushort ProtocolId;
        public int RpcId;
        public int ErrorCode;
        public int BodyLength;
    }

}