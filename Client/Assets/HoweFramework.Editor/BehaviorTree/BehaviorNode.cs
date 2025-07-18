using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 行为树编辑器节点数据类
    /// </summary>
    [Serializable]
    public class BehaviorNode
    {
        [SerializeField] private string m_Id;
        [SerializeField] private string m_Name;
        [SerializeField] private string m_TypeName;
        [SerializeField] private BehaviorNodeType m_NodeType;
        [SerializeField] private List<BehaviorNodeProperty> m_Properties;
        [SerializeField] private bool m_SupportChildren;
        [SerializeField] private int m_MaxChildrenCount; // -1代表无限制
        [SerializeField] private List<string> m_ChildrenIds;
        [SerializeField] private string m_ParentId;
        [SerializeField] private Vector2 m_GraphPosition;

        /// <summary>
        /// 节点唯一ID
        /// </summary>
        public string Id
        {
            get => m_Id;
            set => m_Id = value;
        }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name
        {
            get => m_Name;
            set => m_Name = value;
        }

        /// <summary>
        /// 节点类型名
        /// </summary>
        public string TypeName
        {
            get => m_TypeName;
            set => m_TypeName = value;
        }

        /// <summary>
        /// 节点类型
        /// </summary>
        public BehaviorNodeType NodeType
        {
            get => m_NodeType;
            set => m_NodeType = value;
        }

        /// <summary>
        /// 节点属性列表
        /// </summary>
        public List<BehaviorNodeProperty> Properties
        {
            get => m_Properties ??= new List<BehaviorNodeProperty>();
            set => m_Properties = value;
        }

        /// <summary>
        /// 是否支持子节点
        /// </summary>
        public bool SupportChildren
        {
            get => m_SupportChildren;
            set => m_SupportChildren = value;
        }

        /// <summary>
        /// 支持的子节点数量限制，-1代表无限制
        /// </summary>
        public int MaxChildrenCount
        {
            get => m_MaxChildrenCount;
            set => m_MaxChildrenCount = value;
        }

        /// <summary>
        /// 关联的子节点ID列表
        /// </summary>
        public List<string> ChildrenIds
        {
            get => m_ChildrenIds ??= new List<string>();
            set => m_ChildrenIds = value;
        }

        /// <summary>
        /// 父节点ID
        /// </summary>
        public string ParentId
        {
            get => m_ParentId;
            set => m_ParentId = value;
        }

        /// <summary>
        /// 在BehaviorGraph图上的位置
        /// </summary>
        public Vector2 GraphPosition
        {
            get => m_GraphPosition;
            set => m_GraphPosition = value;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BehaviorNode()
        {
            m_Id = Guid.NewGuid().ToString();
            m_Properties = new List<BehaviorNodeProperty>();
            m_ChildrenIds = new List<string>();
            m_MaxChildrenCount = -1;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <param name="name">节点名称</param>
        /// <param name="typeName">节点类型名</param>
        public BehaviorNode(string id, string name, string typeName) : this()
        {
            m_Id = id;
            m_Name = name;
            m_TypeName = typeName;
        }

        /// <summary>
        /// 检查是否可以添加子节点
        /// </summary>
        /// <returns>是否可以添加</returns>
        public bool CanAddChild()
        {
            if (!SupportChildren)
                return false;

            if (MaxChildrenCount == -1)
                return true;

            return ChildrenIds.Count < MaxChildrenCount;
        }

        /// <summary>
        /// 添加子节点ID
        /// </summary>
        /// <param name="childId">子节点ID</param>
        /// <returns>是否添加成功</returns>
        public bool AddChildId(string childId)
        {
            if (!CanAddChild() || ChildrenIds.Contains(childId))
                return false;

            ChildrenIds.Add(childId);
            return true;
        }

        /// <summary>
        /// 移除子节点ID
        /// </summary>
        /// <param name="childId">子节点ID</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveChildId(string childId)
        {
            return ChildrenIds.Remove(childId);
        }

        /// <summary>
        /// 克隆节点
        /// </summary>
        /// <returns>克隆的节点</returns>
        public BehaviorNode Clone()
        {
            var clone = new BehaviorNode(Guid.NewGuid().ToString(), Name, TypeName)
            {
                NodeType = NodeType,
                SupportChildren = SupportChildren,
                MaxChildrenCount = MaxChildrenCount,
                GraphPosition = GraphPosition
            };

            // 复制属性
            foreach (var property in Properties)
            {
                clone.Properties.Add(property.Clone());
            }

            return clone;
        }

        /// <summary>
        /// 转换为运行时配置
        /// </summary>
        /// <returns>运行时配置</returns>
        public BehaviorNodeConfig ToRuntimeConfig()
        {
            var config = new BehaviorNodeConfig();
            config.Id = Id;
            config.RuntimeTypeName = TypeName;
            config.ChildrenIds.AddRange(ChildrenIds);

            foreach (var property in Properties)
            {
                config.Properties.Add(property.ToRuntimeConfig());
            }

            return config;
        }
    }

    /// <summary>
    /// 行为树节点属性
    /// </summary>
    [Serializable]
    public class BehaviorNodeProperty
    {
        [SerializeField] private string m_Name;
        [SerializeField] private BehaviorPropertyType m_ValueType;
        [SerializeField] private bool m_BoolValue;
        [SerializeField] private int m_IntValue;
        [SerializeField] private long m_LongValue;
        [SerializeField] private float m_FloatValue;
        [SerializeField] private double m_DoubleValue;
        [SerializeField] private string m_StringValue;

        /// <summary>
        /// 属性名
        /// </summary>
        public string Name
        {
            get => m_Name;
            set => m_Name = value;
        }

        /// <summary>
        /// 属性值类型
        /// </summary>
        public BehaviorPropertyType ValueType
        {
            get => m_ValueType;
            set => m_ValueType = value;
        }

        /// <summary>
        /// 属性值
        /// </summary>
        public object Value
        {
            get
            {
                switch (ValueType)
                {
                    case BehaviorPropertyType.Bool:
                        return m_BoolValue;
                    case BehaviorPropertyType.Int:
                        return m_IntValue;
                    case BehaviorPropertyType.Long:
                        return m_LongValue;
                    case BehaviorPropertyType.Float:
                        return m_FloatValue;
                    case BehaviorPropertyType.Double:
                        return m_DoubleValue;
                    case BehaviorPropertyType.String:
                        return m_StringValue;
                    default:
                        return null;
                }
            }
            set => SetValue(value);
        }

        /// <summary>
        /// 布尔值
        /// </summary>
        public bool BoolValue
        {
            get => m_BoolValue;
            set
            {
                m_BoolValue = value;
                m_ValueType = BehaviorPropertyType.Bool;
            }
        }

        /// <summary>
        /// 整数值
        /// </summary>
        public int IntValue
        {
            get => m_IntValue;
            set
            {
                m_IntValue = value;
                m_ValueType = BehaviorPropertyType.Int;
            }
        }

        /// <summary>
        /// 长整数值
        /// </summary>
        public long LongValue
        {
            get => m_LongValue;
            set
            {
                m_LongValue = value;
                m_ValueType = BehaviorPropertyType.Long;
            }
        }

        /// <summary>
        /// 浮点数值
        /// </summary>
        public float FloatValue
        {
            get => m_FloatValue;
            set
            {
                m_FloatValue = value;
                m_ValueType = BehaviorPropertyType.Float;
            }
        }

        /// <summary>
        /// 双精度浮点数值
        /// </summary>
        public double DoubleValue
        {
            get => m_DoubleValue;
            set
            {
                m_DoubleValue = value;
                m_ValueType = BehaviorPropertyType.Double;
            }
        }

        /// <summary>
        /// 字符串值
        /// </summary>
        public string StringValue
        {
            get => m_StringValue;
            set
            {
                m_StringValue = value;
                m_ValueType = BehaviorPropertyType.String;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BehaviorNodeProperty()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">属性名</param>
        /// <param name="valueType">值类型</param>
        /// <param name="value">属性值</param>
        public BehaviorNodeProperty(string name, BehaviorPropertyType valueType, object value = null)
        {
            m_Name = name;
            m_ValueType = valueType;
            m_StringValue = string.Empty;
            if (value != null)
            {
                SetValue(value);
            }
            else
            {
                SetDefaultValue();
            }
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="value">属性值</param>
        private void SetValue(object value)
        {
            if (value == null)
            {
                SetDefaultValue();
                return;
            }

            switch (ValueType)
            {
                case BehaviorPropertyType.Bool:
                    m_BoolValue = Convert.ToBoolean(value);
                    break;
                case BehaviorPropertyType.Int:
                    m_IntValue = Convert.ToInt32(value);
                    break;
                case BehaviorPropertyType.Long:
                    m_LongValue = Convert.ToInt64(value);
                    break;
                case BehaviorPropertyType.Float:
                    m_FloatValue = Convert.ToSingle(value);
                    break;
                case BehaviorPropertyType.Double:
                    m_DoubleValue = Convert.ToDouble(value);
                    break;
                case BehaviorPropertyType.String:
                    m_StringValue = value.ToString();
                    break;
            }
        }

        /// <summary>
        /// 设置默认值
        /// </summary>
        private void SetDefaultValue()
        {
            switch (ValueType)
            {
                case BehaviorPropertyType.Bool:
                    m_BoolValue = false;
                    break;
                case BehaviorPropertyType.Int:
                    m_IntValue = 0;
                    break;
                case BehaviorPropertyType.Long:
                    m_LongValue = 0L;
                    break;
                case BehaviorPropertyType.Float:
                    m_FloatValue = 0f;
                    break;
                case BehaviorPropertyType.Double:
                    m_DoubleValue = 0.0;
                    break;
                case BehaviorPropertyType.String:
                    m_StringValue = string.Empty;
                    break;
            }
        }

        /// <summary>
        /// 克隆属性
        /// </summary>
        /// <returns>克隆的属性</returns>
        public BehaviorNodeProperty Clone()
        {
            return new BehaviorNodeProperty(Name, ValueType, Value);
        }

        /// <summary>
        /// 转换为运行时配置
        /// </summary>
        /// <returns>运行时配置</returns>
        public BehaviorPropertyConfig ToRuntimeConfig()
        {
            var config = new BehaviorPropertyConfig();
            config.Name = Name;
            config.PropertyType = ValueType;
            switch (ValueType)
            {
                case BehaviorPropertyType.Bool:
                    config.BoolValue = BoolValue;
                    break;
                case BehaviorPropertyType.Int:
                    config.IntValue = IntValue;
                    break;
                case BehaviorPropertyType.Long:
                    config.LongValue = LongValue;
                    break;
                case BehaviorPropertyType.Float:
                    config.FloatValue = FloatValue;
                    break;
                case BehaviorPropertyType.Double:
                    config.DoubleValue = DoubleValue;
                    break;
                case BehaviorPropertyType.String:
                    config.StringValue = StringValue;
                    break;
                default:
                    break;
            }

            return config;
        }
    }
} 