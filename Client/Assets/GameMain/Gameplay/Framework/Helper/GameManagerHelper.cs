using System;
using System.Collections.Generic;
using System.Reflection;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 管理器辅助工具.
    /// </summary>
    public static class GameManagerHelper
    {
        private static readonly Dictionary<Type, int> s_ManagerTypeDict = new();

        public static void Clear()
        {
            s_ManagerTypeDict.Clear();
        }

        /// <summary>
        /// 获取管理器类型.
        /// </summary>
        public static int GetManagerType(Type type)
        {
            if (s_ManagerTypeDict.TryGetValue(type, out var managerType))
            {
                return managerType;
            }

            var attribute = AssemblyUtility.GetCustomAttribute<GameManagerAttribute>(type, true);
            if (attribute == null)
            {
                throw new Exception(string.Format("Type '{0}' is not a game manager type.", type.FullName));
            }
            
            managerType = attribute.ManagerType;
            s_ManagerTypeDict.Add(type, managerType);

            return managerType;
        }

        /// <summary>
        /// 获取管理器类型.
        /// </summary>
        public static int GetManagerType<T>() where T : IGameManager
        {
            return GetManagerType(typeof(T));
        }

        /// <summary>
        /// 获取管理器实例.
        /// </summary>
        public static T GetManager<T>(this IGameContext context) where T : IGameManager
        {
            var managerType = GetManagerType<T>();
            var manager = context.GetManager(managerType);
            return (T)manager;
        }
    }
}