using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 序列化扩展。
    /// </summary>
    public static class SerializeExtensions
    {
        /// <summary>
        /// 写入布尔列表。
        /// </summary>
        /// <param name="writer">缓冲区写入器。</param>
        /// <param name="value">布尔列表。</param>
        public static void WriteBoolList(this IBufferWriter writer, ICollection<bool> value)
        {
            if (writer == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "缓冲区写入器不能为空。");
            }

            if (value == null)
            {
                writer.WriteInt32(0);
                return;
            }

            writer.WriteInt32(value.Count);
            foreach (var item in value)
            {
                writer.WriteBool(item);
            }
        }

        /// <summary>
        /// 写入短整型列表。
        /// </summary>
        /// <param name="writer">缓冲区写入器。</param>
        /// <param name="value">短整型列表。</param>
        public static void WriteInt16List(this IBufferWriter writer, ICollection<short> value)
        {
            if (writer == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "缓冲区写入器不能为空。");
            }

            if (value == null)
            {
                writer.WriteInt32(0);
                return;
            }

            writer.WriteInt32(value.Count);
            foreach (var item in value)
            {
                writer.WriteInt16(item);
            }
        }

        /// <summary>
        /// 写入整型列表。
        /// </summary>
        /// <param name="writer">缓冲区写入器。</param>
        /// <param name="value">整型列表。</param>
        public static void WriteInt32List(this IBufferWriter writer, ICollection<int> value)
        {
            if (writer == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "缓冲区写入器不能为空。");
            }

            if (value == null)
            {
                writer.WriteInt32(0);
                return;
            }

            writer.WriteInt32(value.Count);
            foreach (var item in value)
            {
                writer.WriteInt32(item);
            }
        }

        /// <summary>
        /// 写入长整型列表。
        /// </summary>
        /// <param name="writer">缓冲区写入器。</param>
        /// <param name="value">长整型列表。</param>
        public static void WriteInt64List(this IBufferWriter writer, ICollection<long> value)
        {
            if (writer == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "缓冲区写入器不能为空。");
            }

            if (value == null)
            {
                writer.WriteInt32(0);
                return;
            }

            writer.WriteInt32(value.Count);
            foreach (var item in value)
            {
                writer.WriteInt64(item);
            }
        }

        /// <summary>
        /// 写入单精度浮点数列表。
        /// </summary>
        /// <param name="writer">缓冲区写入器。</param>
        /// <param name="value">单精度浮点数列表。</param>
        public static void WriteFloatList(this IBufferWriter writer, ICollection<float> value)
        {
            if (writer == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "缓冲区写入器不能为空。");
            }

            if (value == null)
            {
                writer.WriteInt32(0);
                return;
            }

            writer.WriteInt32(value.Count);
            foreach (var item in value)
            {
                writer.WriteFloat(item);
            }
        }

        /// <summary>
        /// 写入双精度浮点数列表。
        /// </summary>
        /// <param name="writer">缓冲区写入器。</param>
        /// <param name="value">双精度浮点数列表。</param>
        public static void WriteDoubleList(this IBufferWriter writer, ICollection<double> value)
        {
            if (writer == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "缓冲区写入器不能为空。");
            }

            if (value == null)
            {
                writer.WriteInt32(0);
                return;
            }

            writer.WriteInt32(value.Count);
            foreach (var item in value)
            {
                writer.WriteDouble(item);
            }
        }

        /// <summary>
        /// 写入字符串列表。
        /// </summary>
        /// <param name="writer">缓冲区写入器。</param>
        /// <param name="value">字符串列表。</param>
        public static void WriteStringList(this IBufferWriter writer, ICollection<string> value)
        {
            if (writer == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "缓冲区写入器不能为空。");
            }

            if (value == null)
            {
                writer.WriteInt32(0);
                return;
            }

            writer.WriteInt32(value.Count);
            foreach (var item in value)
            {
                writer.WriteString(item);
            }
        }

        /// <summary>
        /// 写入可序列化对象的列表。
        /// </summary>
        /// <param name="writer">缓冲区写入器。</param>
        /// <param name="value">可序列化列表。</param>
        public static void WriteObjectList<T>(this IBufferWriter writer, ICollection<T> value) where T : ISerializable
        {
            if (writer == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "缓冲区写入器不能为空。");
            }

            if (value == null)
            {
                writer.WriteInt32(0);
                return;
            }

            writer.WriteInt32(value.Count);
            foreach (var item in value)
            {
                writer.WriteObject(item);
            }
        }

        /// <summary>
        /// 读取布尔列表。
        /// </summary>
        /// <param name="reader">缓冲区读取器。</param>
        /// <param name="list">布尔列表（输出）。</param>
        public static void ReadBoolList(this IBufferReader reader, List<bool> list)
        {
            if (reader == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "缓冲区读取器不能为空。");
            }
            if (list == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "目标列表不能为空。");
            }
            list.Clear();
            var count = reader.ReadInt32();
            if (count == 0)
            {
                return;
            }
            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadBool());
            }
        }

        /// <summary>
        /// 读取短整型列表。
        /// </summary>
        /// <param name="reader">缓冲区读取器。</param>
        /// <param name="list">短整型列表（输出）。</param>
        public static void ReadInt16List(this IBufferReader reader, List<short> list)
        {
            if (reader == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "缓冲区读取器不能为空。");
            }
            if (list == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "目标列表不能为空。");
            }
            list.Clear();
            var count = reader.ReadInt32();
            if (count == 0)
            {
                return;
            }
            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadInt16());
            }
        }

        /// <summary>
        /// 读取整型列表。
        /// </summary>
        /// <param name="reader">缓冲区读取器。</param>
        /// <param name="list">整型列表（输出）。</param>
        public static void ReadInt32List(this IBufferReader reader, List<int> list)
        {
            if (reader == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "缓冲区读取器不能为空。");
            }
            if (list == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "目标列表不能为空。");
            }
            list.Clear();
            var count = reader.ReadInt32();
            if (count == 0)
            {
                return;
            }
            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadInt32());
            }
        }

        /// <summary>
        /// 读取长整型列表。
        /// </summary>
        /// <param name="reader">缓冲区读取器。</param>
        /// <param name="list">长整型列表（输出）。</param>
        public static void ReadInt64List(this IBufferReader reader, List<long> list)
        {
            if (reader == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "缓冲区读取器不能为空。");
            }
            if (list == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "目标列表不能为空。");
            }
            list.Clear();
            var count = reader.ReadInt32();
            if (count == 0)
            {
                return;
            }
            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadInt64());
            }
        }

        /// <summary>
        /// 读取单精度浮点数列表。
        /// </summary>
        /// <param name="reader">缓冲区读取器。</param>
        /// <param name="list">单精度浮点数列表（输出）。</param>
        public static void ReadFloatList(this IBufferReader reader, List<float> list)
        {
            if (reader == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "缓冲区读取器不能为空。");
            }
            if (list == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "目标列表不能为空。");
            }
            list.Clear();
            var count = reader.ReadInt32();
            if (count == 0)
            {
                return;
            }
            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadFloat());
            }
        }

        /// <summary>
        /// 读取双精度浮点数列表。
        /// </summary>
        /// <param name="reader">缓冲区读取器。</param>
        /// <param name="list">双精度浮点数列表（输出）。</param>
        public static void ReadDoubleList(this IBufferReader reader, List<double> list)
        {
            if (reader == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "缓冲区读取器不能为空。");
            }
            if (list == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "目标列表不能为空。");
            }
            list.Clear();
            var count = reader.ReadInt32();
            if (count == 0)
            {
                return;
            }
            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadDouble());
            }
        }

        /// <summary>
        /// 读取字符串列表。
        /// </summary>
        /// <param name="reader">缓冲区读取器。</param>
        /// <param name="list">字符串列表（输出）。</param>
        public static void ReadStringList(this IBufferReader reader, List<string> list)
        {
            if (reader == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "缓冲区读取器不能为空。");
            }
            if (list == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "目标列表不能为空。");
            }
            list.Clear();
            var count = reader.ReadInt32();
            if (count == 0)
            {
                return;
            }
            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadString());
            }
        }

        /// <summary>
        /// 读取可序列化对象的列表。
        /// </summary>
        /// <param name="reader">缓冲区读取器。</param>
        /// <param name="list">可序列化对象列表（输出）。</param>
        public static void ReadObjectList<T>(this IBufferReader reader, List<T> list) where T : ISerializable
        {
            if (reader == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "缓冲区读取器不能为空。");
            }
            if (list == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "目标列表不能为空。");
            }
            list.Clear();
            var count = reader.ReadInt32();
            if (count == 0)
            {
                return;
            }
            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadObject<T>());
            }
        }
    }
}