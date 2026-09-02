using System;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 管理器辅助工具.
    /// </summary>
    public static class GameManagerHelper
    {
        /// <summary>
        /// 获取管理器类型.
        /// </summary>
        public static int GetManagerType(Type type)
        {
            return TypeId.GetIdByType(ResolveManagerIdentityType(type));
        }

        /// <summary>
        /// 获取管理器类型.
        /// </summary>
        public static int GetManagerType<T>() where T : IGameManager
        {
            var type = typeof(T);
            if (type.IsInterface)
            {
                if (type == typeof(IGameManager))
                {
                    throw new Exception(string.Format("Type '{0}' is not a game manager type.", type.FullName));
                }

                return TypeId<T>.Id;
            }

            return GetManagerType(type);
        }

        /// <summary>
        /// 管理器运行时 id 以对外接口为准：实现类映射到继承 <see cref="IGameManager"/> 的最根基接口，
        /// 以便 <c>AddManager</c> 与 <c>GetManager&lt;IXxxManager&gt;</c> 使用同一 TypeId。
        /// </summary>
        private static Type ResolveManagerIdentityType(Type type)
        {
            if (type == null)
            {
                throw new Exception("Type is not a game manager type.");
            }

            if (type.IsInterface)
            {
                if (type == typeof(IGameManager) || !typeof(IGameManager).IsAssignableFrom(type))
                {
                    throw new Exception(string.Format("Type '{0}' is not a game manager type.", type.FullName));
                }

                return type;
            }

            Type identity = null;
            foreach (var iface in type.GetInterfaces())
            {
                if (iface == typeof(IGameManager) || !typeof(IGameManager).IsAssignableFrom(iface))
                {
                    continue;
                }

                if (identity == null || iface.IsAssignableFrom(identity))
                {
                    identity = iface;
                }
                else if (!identity.IsAssignableFrom(iface))
                {
                    throw new Exception(string.Format("Type '{0}' implements multiple unrelated game manager interfaces.", type.FullName));
                }
            }

            if (identity == null)
            {
                throw new Exception(string.Format("Type '{0}' is not a game manager type.", type.FullName));
            }

            return identity;
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