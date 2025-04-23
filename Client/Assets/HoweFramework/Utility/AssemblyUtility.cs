using System;
using System.Reflection;

namespace HoweFramework
{
    /// <summary>
    /// 程序集辅助工具.
    /// </summary>
    public static class AssemblyUtility
    {
        /// <summary>
        /// 获取自定义属性.
        /// </summary>
        /// <typeparam name="T">自定义属性类型。</typeparam>
        /// <param name="type">类型。</param>
        /// <param name="includeBaseType">是否包含基类。</param>
        /// <returns>自定义属性。</returns>
        public static T GetCustomAttribute<T>(Type type, bool includeBaseType = false) where T : Attribute
        {
            var attribute = type.GetCustomAttribute<T>();
            if (attribute != null)
            {
                return attribute;
            }

            if (!includeBaseType)
            {
                return null;
            }

            var baseType = type.BaseType;
            while (baseType != null)
            {
                attribute = baseType.GetCustomAttribute<T>();
                if (attribute != null)
                {
                    return attribute;
                }

                baseType = baseType.BaseType;
            }

            var interfaces = type.GetInterfaces();
            foreach (var interfaceType in interfaces)
            {
                attribute = interfaceType.GetCustomAttribute<T>();
                if (attribute != null)
                {
                    return attribute;
                }
            }

            return null;
        }
    }
}
