using System;

namespace HoweFramework
{
    /// <summary>
    /// 注入属性。会基于IOC容器自动注入对应类型已注册的实例。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class InjectAttribute : Attribute
    {
    }
}