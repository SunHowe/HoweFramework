using System;
using System.Collections.Generic;
using System.Reflection;

namespace HoweFramework
{
    /// <summary>
    /// 程序集辅助工具.
    /// </summary>
    public static class AssemblyUtility
    {
        /// <summary>
        /// 运行时类型字典。
        /// </summary>
        private static readonly Dictionary<string, Type> s_RuntimeTypeDict = new();

        /// <summary>
        /// 运行时程序集列表。
        /// </summary>
        private static readonly List<Assembly> s_RuntimeAssemblyList = new();

        /// <summary>
        /// 清空工具记录的数据。
        /// </summary>
        public static void Clear()
        {
            s_RuntimeTypeDict.Clear();
            s_RuntimeAssemblyList.Clear();
        }

        /// <summary>
        /// 注册运行时程序集。
        /// </summary>
        /// <param name="assembly">程序集。</param>
        public static void RegisterRuntimeAssembly(Assembly assembly)
        {
            if (s_RuntimeAssemblyList.Contains(assembly))
            {
                return;
            }

            s_RuntimeAssemblyList.Add(assembly);
        }

        /// <summary>
        /// 获取运行时类型。
        /// </summary>
        /// <param name="typeName">类型名。</param>
        /// <returns>运行时类型。</returns>
        public static Type GetRuntimeType(string typeName)
        {
            if (s_RuntimeTypeDict.TryGetValue(typeName, out var type))
            {
                return type;
            }

            foreach (var assembly in s_RuntimeAssemblyList)
            {
                type = assembly.GetType(typeName);
                if (type == null)
                {
                    continue;
                }

                s_RuntimeTypeDict.Add(typeName, type);
                return type;
            }

            return null;
        }

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
    
        /// <summary>
        /// 遍历继承自指定类型的所有类型。
        /// </summary>
        /// <param name="assembly">程序集。</param>
        /// <param name="baseType">基类型。</param>
        /// <param name="action">遍历方法。</param>
        /// <param name="includeAbstract">是否包含抽象类型。</param>
        public static void ForEachWithBaseType(this Assembly assembly, Type baseType, Action<Type> action, bool includeAbstract = false)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!includeAbstract && type.IsAbstract)
                {
                    continue;
                }

                if (!baseType.IsAssignableFrom(type))
                {
                    continue;
                }

                action(type);
            }
        }

        /// <summary>
        /// 遍历继承自指定类型且包含指定自定义属性的所有类型。
        /// </summary>
        /// <typeparam name="T">自定义属性类型。</typeparam>
        /// <param name="assembly">程序集。</param>
        /// <param name="baseType">基类型。</param>
        /// <param name="action">遍历方法。</param>
        /// <param name="includeAbstract">是否包含抽象类型。</param>
        public static void ForEachWithBaseTypeAndAttribute<T>(this Assembly assembly, Type baseType, Action<Type, T> action, bool includeAbstract = false) where T : Attribute
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!includeAbstract && type.IsAbstract)
                {
                    continue;
                }

                if (!baseType.IsAssignableFrom(type))
                {
                    continue;
                }

                var attribute = type.GetCustomAttribute<T>();
                if (attribute == null)
                {
                    continue;
                }

                action(type, attribute);
            }
        }
    }
}
