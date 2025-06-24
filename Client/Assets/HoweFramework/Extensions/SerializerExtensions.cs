namespace HoweFramework
{
    /// <summary>
    /// 序列化器扩展。
    /// </summary>
    public static class SerializerExtensions
    {
        /// <summary>
        /// 注册内置序列化器。
        /// </summary>
        /// <param name="serializer">序列化器。</param>
        public static void RegisterBuiltinSerializers(this ISerializer serializer)
        {
            serializer.RegisterCustomSerializer(BooleanSerializer.Create());
            serializer.RegisterCustomSerializer(ByteSerializer.Create());
            serializer.RegisterCustomSerializer(Int16Serializer.Create());
            serializer.RegisterCustomSerializer(Int32Serializer.Create());
            serializer.RegisterCustomSerializer(Int64Serializer.Create());
            serializer.RegisterCustomSerializer(UInt16Serializer.Create());
            serializer.RegisterCustomSerializer(UInt32Serializer.Create());
            serializer.RegisterCustomSerializer(UInt64Serializer.Create());
            serializer.RegisterCustomSerializer(StringSerializer.Create());
            serializer.RegisterCustomSerializer(CharSerializer.Create());
            serializer.RegisterCustomSerializer(SByteSerializer.Create());
            serializer.RegisterCustomSerializer(SingleSerializer.Create());
            serializer.RegisterCustomSerializer(DoubleSerializer.Create());
            serializer.RegisterCustomSerializer(DecimalSerializer.Create());
            serializer.RegisterCustomSerializer(DateTimeSerializer.Create());
            serializer.RegisterCustomSerializer(TimeSpanSerializer.Create());
            serializer.RegisterCustomSerializer(GuidSerializer.Create());
        }
    }
}