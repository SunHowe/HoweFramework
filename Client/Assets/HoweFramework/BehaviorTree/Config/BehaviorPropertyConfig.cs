namespace HoweFramework
{
    /// <summary>
    /// 行为树节点属性配置。
    /// </summary>
    public sealed class BehaviorPropertyConfig : IReference, ISerializable
    {
        /// <summary>
        /// 属性ID。
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 属性类型。
        /// </summary>
        public BehaviorPropertyType PropertyType { get; set; }

        /// <summary>
        /// 布尔值。
        /// </summary>
        public bool BoolValue
        {
            get => PropertyType == BehaviorPropertyType.Bool ? m_BoolValue : default;
            set
            {
                m_BoolValue = value;
                PropertyType = BehaviorPropertyType.Bool;
            }
        }

        /// <summary>
        /// 整数。
        /// </summary>
        public int IntValue
        {
            get => PropertyType == BehaviorPropertyType.Int ? m_IntValue : default;
            set
            {
                m_IntValue = value;
                PropertyType = BehaviorPropertyType.Int;
            }
        }

        /// <summary>
        /// 长整数。
        /// </summary>
        public long LongValue
        {
            get => PropertyType == BehaviorPropertyType.Long ? m_LongValue : default;
            set
            {
                m_LongValue = value;
                PropertyType = BehaviorPropertyType.Long;
            }
        }

        /// <summary>
        /// 单精度浮点数。
        /// </summary>
        public float FloatValue
        {
            get => PropertyType == BehaviorPropertyType.Float ? m_FloatValue : default;
            set
            {
                m_FloatValue = value;
                PropertyType = BehaviorPropertyType.Float;
            }
        }

        /// <summary>
        /// 双精度浮点数。
        /// </summary>
        public double DoubleValue
        {
            get => PropertyType == BehaviorPropertyType.Double ? m_DoubleValue : default;
            set
            {
                m_DoubleValue = value;
                PropertyType = BehaviorPropertyType.Double;
            }
        }

        /// <summary>
        /// 字符串。
        /// </summary>
        public string StringValue
        {
            get => PropertyType == BehaviorPropertyType.String ? m_StringValue : default;
            set
            {
                m_StringValue = value;
                PropertyType = BehaviorPropertyType.String;
            }
        }

        private bool m_BoolValue;
        private int m_IntValue;
        private long m_LongValue;
        private float m_FloatValue;
        private double m_DoubleValue;
        private string m_StringValue;

        /// <summary>
        /// 清空。
        /// </summary>
        public void Clear()
        {
            m_BoolValue = default;
            m_IntValue = default;
            m_LongValue = default;
            m_FloatValue = default;
            m_DoubleValue = default;
            m_StringValue = default;

            PropertyType = default;
            Id = default;
        }

        /// <summary>
        /// 创建行为树节点属性配置。
        /// </summary>
        public static BehaviorPropertyConfig Create()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
            {
                return new BehaviorPropertyConfig();
            }
#endif

            return ReferencePool.Acquire<BehaviorPropertyConfig>();
        }

        /// <summary>
        /// 创建布尔值属性。
        /// </summary>
        /// <param name="id">属性ID。</param>
        /// <param name="value">属性值。</param>
        /// <returns>创建的布尔值属性。</returns>
        public static BehaviorPropertyConfig CreateBoolProperty(int id, bool value)
        {
            var config = Create();
            config.Id = id;
            config.BoolValue = value;
            return config;
        }

        /// <summary>
        /// 创建32位整数属性。
        /// </summary>
        /// <param name="id">属性ID。</param>
        /// <param name="value">属性值。</param>
        /// <returns>创建的32位整数属性。</returns>
        public static BehaviorPropertyConfig CreateIntProperty(int id, int value)
        {
            var config = Create();
            config.Id = id;
            config.PropertyType = BehaviorPropertyType.Int;
            config.IntValue = value;
            return config;
        }

        /// <summary>
        /// 创建64位整数属性。
        /// </summary>
        /// <param name="id">属性ID。</param>
        /// <param name="value">属性值。</param>
        /// <returns>创建的64位整数属性。</returns>
        public static BehaviorPropertyConfig CreateLongProperty(int id, long value)
        {
            var config = Create();
            config.Id = id;
            config.PropertyType = BehaviorPropertyType.Long;
            config.LongValue = value;
            return config;
        }

        /// <summary>
        /// 创建单精度浮点数属性。
        /// </summary>
        /// <param name="id">属性ID。</param>
        /// <param name="value">属性值。</param>
        /// <returns>创建的单精度浮点数属性。</returns>
        public static BehaviorPropertyConfig CreateFloatProperty(int id, float value)
        {
            var config = Create();
            config.Id = id;
            config.PropertyType = BehaviorPropertyType.Float;
            config.FloatValue = value;
            return config;
        }

        /// <summary>
        /// 创建双精度浮点数属性。
        /// </summary>
        /// <param name="id">属性ID。</param>
        /// <param name="value">属性值。</param>
        /// <returns>创建的双精度浮点数属性。</returns>
        public static BehaviorPropertyConfig CreateDoubleProperty(int id, double value)
        {
            var config = Create();
            config.Id = id;
            config.PropertyType = BehaviorPropertyType.Double;
            config.DoubleValue = value;
            return config;
        }

        /// <summary>
        /// 创建字符串属性。
        /// </summary>
        /// <param name="id">属性ID。</param>
        /// <param name="value">属性值。</param>
        /// <returns>创建的字符串属性。</returns>
        public static BehaviorPropertyConfig CreateStringProperty(int id, string value)
        {
            var config = Create();
            config.Id = id;
            config.PropertyType = BehaviorPropertyType.String;
            config.StringValue = value;
            return config;
        }

        public void Serialize(IBufferWriter writer)
        {
            writer.WriteInt32(Id);
            writer.WriteInt32((int)PropertyType);
            switch (PropertyType)
            {
                case BehaviorPropertyType.Bool:
                    writer.WriteBool(BoolValue);
                    break;
                case BehaviorPropertyType.Int:
                    writer.WriteInt32(IntValue);
                    break;
                case BehaviorPropertyType.Long:
                    writer.WriteInt64(LongValue);
                    break;
                case BehaviorPropertyType.Float:
                    writer.WriteFloat(FloatValue);
                    break;
                case BehaviorPropertyType.Double:
                    writer.WriteDouble(DoubleValue);
                    break;
                case BehaviorPropertyType.String:
                    writer.WriteString(StringValue);
                    break;
            }
        }

        public void Deserialize(IBufferReader reader)
        {
            Id = reader.ReadInt32();
            PropertyType = (BehaviorPropertyType)reader.ReadInt32();
            switch (PropertyType)
            {
                case BehaviorPropertyType.Bool:
                    BoolValue = reader.ReadBool();
                    break;
                case BehaviorPropertyType.Int:
                    IntValue = reader.ReadInt32();
                    break;
                case BehaviorPropertyType.Long:
                    LongValue = reader.ReadInt64();
                    break;
                case BehaviorPropertyType.Float:
                    FloatValue = reader.ReadFloat();
                    break;
                case BehaviorPropertyType.Double:
                    DoubleValue = reader.ReadDouble();
                    break;
                case BehaviorPropertyType.String:
                    StringValue = reader.ReadString();
                    break;
            }
        }
    }
}