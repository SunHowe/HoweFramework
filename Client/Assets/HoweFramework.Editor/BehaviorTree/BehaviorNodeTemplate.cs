using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 行为树节点类型
    /// </summary>
    public enum BehaviorNodeType
    {
        /// <summary>
        /// 行为节点
        /// </summary>
        Action,

        /// <summary>
        /// 复合节点
        /// </summary>
        Composite,

        /// <summary>
        /// 装饰节点
        /// </summary>
        Decor,

        /// <summary>
        /// 根节点
        /// </summary>
        Root
    }

    /// <summary>
    /// 抽象行为树节点模板
    /// </summary>
    public abstract class BehaviorNodeTemplate
    {
        /// <summary>
        /// 节点类型
        /// </summary>
        public abstract BehaviorNodeType NodeType { get; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public abstract string NodeName { get; }

        /// <summary>
        /// 节点描述
        /// </summary>
        public abstract string NodeDescription { get; }

        /// <summary>
        /// 节点图标路径
        /// </summary>
        public virtual string NodeIconPath => null;

        /// <summary>
        /// 节点类型名（对应运行时类型）
        /// </summary>
        public abstract string RuntimeTypeName { get; }

        /// <summary>
        /// 是否支持子节点
        /// </summary>
        public abstract bool SupportChildren { get; }

        /// <summary>
        /// 支持的子节点数量限制，-1代表无限制
        /// </summary>
        public virtual int MaxChildrenCount => -1;

        /// <summary>
        /// 节点默认属性
        /// </summary>
        public virtual List<BehaviorNodePropertyTemplate> DefaultProperties => new List<BehaviorNodePropertyTemplate>();

        /// <summary>
        /// 创建节点实例
        /// </summary>
        /// <returns>节点实例</returns>
        public virtual BehaviorNode CreateNode()
        {
            var node = new BehaviorNode(Guid.NewGuid().ToString(), NodeName, RuntimeTypeName)
            {
                NodeType = NodeType,
                SupportChildren = SupportChildren,
                MaxChildrenCount = MaxChildrenCount
            };

            // 初始化默认属性
            foreach (var propertyTemplate in DefaultProperties)
            {
                var property = new BehaviorNodeProperty(propertyTemplate.Id, propertyTemplate.PropertyName, propertyTemplate.ValueType, propertyTemplate.DefaultValue);
                node.Properties.Add(property);
            }

            return node;
        }

        /// <summary>
        /// 验证节点配置
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>验证结果</returns>
        public virtual BehaviorNodeValidationResult ValidateNode(BehaviorNode node)
        {
            var result = new BehaviorNodeValidationResult();

            // 基础验证
            if (string.IsNullOrEmpty(node.Name))
            {
                result.AddError("节点名称不能为空");
            }

            if (string.IsNullOrEmpty(node.TypeName))
            {
                result.AddError("节点类型名不能为空");
            }

            // 子节点数量验证
            if (!SupportChildren && node.ChildrenIds.Count > 0)
            {
                result.AddError("此节点类型不支持子节点");
            }

            if (SupportChildren && MaxChildrenCount > 0 && node.ChildrenIds.Count > MaxChildrenCount)
            {
                result.AddError($"子节点数量超过限制（最大{MaxChildrenCount}个）");
            }

            return result;
        }
    }

    /// <summary>
    /// 行为节点模板基类
    /// </summary>
    public abstract class BehaviorNodeTemplate_Action : BehaviorNodeTemplate
    {
        public override BehaviorNodeType NodeType => BehaviorNodeType.Action;
        public override bool SupportChildren => false;
    }

    /// <summary>
    /// 复合节点模板基类
    /// </summary>
    public abstract class BehaviorNodeTemplate_Composite : BehaviorNodeTemplate
    {
        public override BehaviorNodeType NodeType => BehaviorNodeType.Composite;
        public override bool SupportChildren => true;
    }

    /// <summary>
    /// 装饰节点模板基类
    /// </summary>
    public abstract class BehaviorNodeTemplate_Decor : BehaviorNodeTemplate
    {
        public override BehaviorNodeType NodeType => BehaviorNodeType.Decor;
        public override bool SupportChildren => true;
        public override int MaxChildrenCount => 1;
    }

    /// <summary>
    /// 行为树节点属性模板
    /// </summary>
    [Serializable]
    public class BehaviorNodePropertyTemplate
    {
        /// <summary>
        /// 属性唯一ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 属性名
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 属性值类型
        /// </summary>
        public BehaviorNodePropertyValueType ValueType { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">属性唯一ID</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="valueType">值类型</param>
        /// <param name="displayName">显示名</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="isRequired">是否必需</param>
        public BehaviorNodePropertyTemplate(int id, string propertyName, BehaviorNodePropertyValueType valueType, object defaultValue = null)
        {
            Id = id;
            PropertyName = propertyName;
            ValueType = valueType;
            DefaultValue = defaultValue;
        }
    }

    /// <summary>
    /// 节点验证结果
    /// </summary>
    public class BehaviorNodeValidationResult
    {
        private readonly List<string> m_Errors = new List<string>();
        private readonly List<string> m_Warnings = new List<string>();

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid => m_Errors.Count == 0;

        /// <summary>
        /// 错误列表
        /// </summary>
        public IReadOnlyList<string> Errors => m_Errors;

        /// <summary>
        /// 警告列表
        /// </summary>
        public IReadOnlyList<string> Warnings => m_Warnings;

        /// <summary>
        /// 添加错误
        /// </summary>
        /// <param name="error">错误信息</param>
        public void AddError(string error)
        {
            if (!string.IsNullOrEmpty(error))
                m_Errors.Add(error);
        }

        /// <summary>
        /// 添加警告
        /// </summary>
        /// <param name="warning">警告信息</param>
        public void AddWarning(string warning)
        {
            if (!string.IsNullOrEmpty(warning))
                m_Warnings.Add(warning);
        }
    }
} 