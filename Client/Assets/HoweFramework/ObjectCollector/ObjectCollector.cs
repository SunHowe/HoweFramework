using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HoweFramework
{
    /// <summary>
    /// 对象收集器组件。挂载某个GameObject，用于配置供逻辑层快速获取的子节点组件使用。
    /// </summary>
    public sealed class ObjectCollector : MonoBehaviour
    {
        /// <summary>
        /// 对象名称列表。
        /// </summary>
        public List<string> ObjectNameList;

        /// <summary>
        /// 对象实例列表。
        /// </summary>
        public List<Object> ObjectList;

        /// <summary>
        /// 转换后的字典实例。
        /// </summary>
        private readonly Dictionary<string, Object> m_ObjectDictionary = new Dictionary<string, Object>();

        private void Awake()
        {
            if (ObjectNameList == null || ObjectList == null)
            {
                return;
            }

            var count = Math.Min(ObjectNameList.Count, ObjectList.Count);
            for (var i = 0; i < count; i++)
            {
                var objectName = ObjectNameList[i];
                if (string.IsNullOrEmpty(objectName))
                {
                    continue;
                }

                // 重名键容错：保留先出现的一项并告警，避免 Awake 直接抛异常中断组件初始化。
                if (!m_ObjectDictionary.TryAdd(objectName, ObjectList[i]))
                {
                    Log.Warning($"ObjectCollector on '{gameObject.name}' has duplicate object name '{objectName}', the later one is ignored.");
                }
            }
        }

        /// <summary>
        /// 获取指定名字的对象。
        /// </summary>
        public Object GetObject(string objectName)
        {
            return m_ObjectDictionary.TryGetValue(objectName, out var widget) ? widget : null;
        }

        /// <summary>
        /// 获取指定名字指定类型的对象。
        /// </summary>
        public T Get<T>(string objectName) where T : Object
        {
            return GetObject(objectName) as T;
        }
    }
}