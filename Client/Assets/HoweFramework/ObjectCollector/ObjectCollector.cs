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
                m_ObjectDictionary.Add(ObjectNameList[i], ObjectList[i]);
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