using System;
using System.Collections.Generic;
using System.Reflection;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 游戏实体辅助工具。
    /// </summary>
    public static class GameEntityHelper
    {
        private static readonly Dictionary<Type, int> s_ComponentTypeDict = new();

        public static void Clear()
        {
            s_ComponentTypeDict.Clear();
        }

        /// <summary>
        /// 获取组件类型.
        /// </summary>
        public static int GetComponentType(Type type)
        {
            if (s_ComponentTypeDict.TryGetValue(type, out var componentType))
            {
                return componentType;
            }

            var attribute = type.GetCustomAttribute<GameComponentAttribute>();
            if (attribute == null)
            {
                throw new Exception(string.Format("Type '{0}' is not a game component type.", type.FullName));
            }

            componentType = attribute.ComponentType;
            s_ComponentTypeDict.Add(type, componentType);

            return componentType;
        }

        /// <summary>
        /// 获取组件类型.
        /// </summary>
        public static int GetComponentType<T>() where T : GameComponentBase
        {
            return GetComponentType(typeof(T));
        }

        /// <summary>
        /// 添加组件.
        /// </summary>
        public static T AddComponent<T>(this IGameEntity entity) where T : GameComponentBase, new()
        {
            var component = ReferencePool.Acquire<T>();
            entity.AddComponent(component);
            return component;
        }

        /// <summary>
        /// 获取组件实例。
        /// </summary>
        public static T GetComponent<T>(this IGameEntity entity) where T : GameComponentBase
        {
            return (T)entity.GetComponent(GetComponentType<T>());
        }

        /// <summary>
        /// 获取组件实例。
        /// </summary>
        public static IGameComponent GetComponent(this IGameEntity entity, Type type)
        {
            return entity.GetComponent(GetComponentType(type));
        }

        /// <summary>
        /// 移除组件.
        /// </summary>
        public static void RemoveComponent<T>(this IGameEntity entity) where T : GameComponentBase
        {
            entity.RemoveComponent(GetComponentType<T>());
        }

        /// <summary>
        /// 移除组件.
        /// </summary>
        public static void RemoveComponent(this IGameEntity entity, Type type)
        {
            entity.RemoveComponent(GetComponentType(type));
        }
    }
}
