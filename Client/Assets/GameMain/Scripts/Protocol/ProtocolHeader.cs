using System.Runtime.InteropServices;

namespace Protocol
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ProtocolHeader
    {
        /// <summary>
        /// 协议ID。
        /// </summary>
        public ushort ProtocolId;

        /// <summary>
        /// 包体长度。
        /// </summary>
        public ushort BodyLength;
        
        /// <summary>
        /// 请求ID。
        /// </summary>
        public int RpcId;

        /// <summary>
        /// 参数。在请求时是MagicNumber，在响应时是ErrorCode。
        /// </summary>
        public int Param;
    }
}