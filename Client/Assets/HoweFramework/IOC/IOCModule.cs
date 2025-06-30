using System;
using System.Collections.Generic;
using System.Reflection;

namespace HoweFramework
{
    /// <summary>
    /// IOC 模块。为全局单例提供统一注册和获取的接口。
    /// 注意：IOC模块不负责管理实例的生命周期，实例的生命周期由调用者管理。
    /// </summary>
    public sealed class IOCModule : ModuleBase<IOCModule>
    {
        /// <summary>
        /// 已注册的实例字典。
        /// </summary>
        private readonly Dictionary<Type, object> m_RegistedDict = new();

        /// <summary>
        /// 缓存的类型需要注入的成员信息。
        /// </summary>
        private readonly Dictionary<Type, List<MemberInfo>> m_InjectMemberDict = new();

        /// <summary>
        /// 向 IOC 容器中注册实例。若已存在，则覆盖。
        /// </summary>
        /// <typeparam name="T">实例类型。</typeparam>
        /// <param name="instance">实例。</param>
        public void Register<T>(T instance)
        {
            m_RegistedDict[typeof(T)] = instance;
        }

        /// <summary>
        /// 从 IOC 容器中移除实例。
        /// </summary>
        /// <typeparam name="T">实例类型。</typeparam>
        public void UnRegister<T>()
        {
            m_RegistedDict.Remove(typeof(T));
        }

        /// <summary>
        /// 从 IOC 容器中移除实例（会进行实例的引用检查）。
        /// </summary>
        /// <typeparam name="T">实例类型。</typeparam>
        /// <param name="instance">实例。</param>
        public void UnRegister<T>(T instance)
        {
            if (m_RegistedDict.TryGetValue(typeof(T), out var value) && ReferenceEquals(value, instance))
            {
                m_RegistedDict.Remove(typeof(T));
            }
        }

        /// <summary>
        /// 从 IOC 容器中移除实例。
        /// </summary>
        /// <param name="type">实例类型。</param>
        public void UnRegister(Type type)
        {
            m_RegistedDict.Remove(type);
        }

        /// <summary>
        /// 从 IOC 容器中移除所有实例。
        /// </summary>
        public void UnRegisterAll()
        {
            m_RegistedDict.Clear();
        }

        /// <summary>
        /// 从 IOC 容器中获取实例。
        /// </summary>
        /// <typeparam name="T">实例类型。</typeparam>
        /// <returns>实例。</returns>
        public T Get<T>()
        {
            var type = typeof(T);
            if (m_RegistedDict.TryGetValue(type, out var value))
            {
                return (T)value;
            }

            return default;
        }

        /// <summary>
        /// 从 IOC 容器中获取实例。
        /// </summary>
        /// <param name="type">实例类型。</param>
        /// <returns>实例。</returns>
        public object Get(Type type)
        {
            if (m_RegistedDict.TryGetValue(type, out var value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// 为实例注入IOC容器中的实例。
        /// </summary>
        public void Inject(object instance)
        {
            var type = instance.GetType();
            if (!m_InjectMemberDict.TryGetValue(type, out var memberInfos))
            {
                var attributeType = typeof(InjectAttribute);
                memberInfos = new List<MemberInfo>();

                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var field in fields)
                {
                    if (!field.IsDefined(attributeType))
                    {
                        continue;
                    }

                    memberInfos.Add(field);
                }

                var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var property in properties)
                {
                    if (property.SetMethod == null)
                    {
                        continue;
                    }

                    if (!property.IsDefined(attributeType))
                    {
                        continue;
                    }

                    memberInfos.Add(property);
                }

                m_InjectMemberDict[type] = memberInfos;
            }

            foreach (var memberInfo in memberInfos)
            {
                var value = Get(memberInfo.DeclaringType);
                if (value == null)
                {
                    continue;
                }

                if (memberInfo is FieldInfo fieldInfo)
                {
                    fieldInfo.SetValue(instance, value);
                }
                else if (memberInfo is PropertyInfo propertyInfo)
                {
                    propertyInfo.SetValue(instance, value);
                }
            }
        }

        protected override void OnInit()
        {
        }

        protected override void OnDestroy()
        {
            m_RegistedDict.Clear();
            m_InjectMemberDict.Clear();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}