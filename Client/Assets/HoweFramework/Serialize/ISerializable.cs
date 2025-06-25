using System.Buffers;

namespace HoweFramework
{
    /// <summary>
    /// 可序列化接口。
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// 序列化。
        /// </summary>
        /// <param name="writer">序列化器。</param>
        void Serialize(IBufferWriter writer);

        /// <summary>
        /// 反序列化。
        /// </summary>
        /// <param name="reader">反序列化器。</param>
        void Deserialize(IBufferReader reader);
    }
}