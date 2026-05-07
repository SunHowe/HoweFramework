using System;
using System.IO;

namespace Protocol
{
    /// <summary>
    /// 协议头辅助类。
    /// </summary>
    public static class ProtocolHeaderHelper
    {
        /// <summary>
        /// 协议头长度。
        /// </summary>
        public const int HeaderLength = 12;

        /// <summary>
        /// 协议包体长度限制。
        /// </summary>
        public const int BodyLengthLimit = 65536;

        /// <summary>
        /// 尝试反序列化协议头。
        /// </summary>
        /// <param name="buffer">缓冲区。</param>
        /// <param name="header">协议头。</param>
        /// <returns>是否反序列化成功。</returns>
        public static bool TryDeserialize(ReadOnlySpan<byte> buffer, out ProtocolHeader header)
        {
            if (buffer.Length < HeaderLength)
            {
                header = default;
                return false;
            }

            header = new ProtocolHeader
            {
                ProtocolId = ByteHelper.ToUInt16(buffer),
                BodyLength = ByteHelper.ToUInt16(buffer.Slice(2)),
                RpcId = ByteHelper.ToInt32(buffer.Slice(4)),
                Param = ByteHelper.ToInt32(buffer.Slice(8)),
            };

            return true;
        }

        /// <summary>
        /// 序列化协议头。
        /// </summary>
        /// <param name="header">协议头。</param>
        /// <param name="destination">目标流。</param>
        public static void Serialize(in ProtocolHeader header, Stream destination)
        {
            ByteHelper.Serialize(header.ProtocolId, destination);
            ByteHelper.Serialize(header.BodyLength, destination);
            ByteHelper.Serialize(header.RpcId, destination);
            ByteHelper.Serialize(header.Param, destination);
        }

        /// <summary>
        /// 序列化协议头。
        /// </summary>
        /// <param name="header">协议头。</param>
        /// <param name="buffer">缓冲区。</param>
        /// <param name="offset">偏移量。</param>
        public static void Serialize(in ProtocolHeader header, byte[] buffer, int offset)
        {
            ByteHelper.Serialize(header.ProtocolId, buffer, offset);
            ByteHelper.Serialize(header.BodyLength, buffer, offset + 2);
            ByteHelper.Serialize(header.RpcId, buffer, offset + 4);
            ByteHelper.Serialize(header.Param, buffer, offset + 8);
        }
    }
}