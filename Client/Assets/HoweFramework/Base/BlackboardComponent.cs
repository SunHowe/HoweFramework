using System.Collections.Generic;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 黑板组件。用于挂载到游戏对象上，提供黑板功能。
    /// </summary>
    public sealed class BlackboardComponent : MonoBehaviour, IBlackboard
    {
        /// <summary>
        /// 值字典。
        /// </summary>
        private readonly Dictionary<string, object> m_ValueDict = new();

        public void Clear()
        {
            m_ValueDict.Clear();
        }

        public void Dispose()
        {
            Object.Destroy(this);
        }

        public object GetObject(string key)
        {
            if (m_ValueDict.TryGetValue(key, out var value))
            {
                return value;
            }

            return null;
        }

        public T GetValue<T>(string key, T defaultValue = default)
        {
            if (m_ValueDict.TryGetValue(key, out var value) && value is T t)
            {
                return t;
            }

            return defaultValue;
        }

        public bool HasValue(string key)
        {
            return m_ValueDict.ContainsKey(key);
        }

        public void RemoveObject(string key)
        {
            m_ValueDict.Remove(key);
        }

        public void RemoveValue(string key)
        {
            m_ValueDict.Remove(key);
        }

        public void SetObject(string key, object value)
        {
            m_ValueDict[key] = value;
        }

        public void SetValue<T>(string key, T value)
        {
            m_ValueDict[key] = value;
        }

        public bool TryGetObject(string key, out object value)
        {
            if (m_ValueDict.TryGetValue(key, out value))
            {
                return true;
            }

            value = default;
            return false;
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            if (m_ValueDict.TryGetValue(key, out var obj) && obj is T t)
            {
                value = t;
                return true;
            }

            value = default;
            return false;
        }

        public void CopyTo(IBlackboard blackboard)
        {
            foreach (var item in m_ValueDict)
            {
                blackboard.SetValue(item.Key, item.Value);
            }
        }
    }
}