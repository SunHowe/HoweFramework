using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 黑板接口。
    /// </summary>
    public interface IBlackboard : IDisposable
    {
        /// <summary>
        /// 设置值。
        /// </summary>
        /// <typeparam name="T">值类型。</typeparam>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        void SetValue<T>(string key, T value);

        /// <summary>
        /// 获取值。
        /// </summary>
        /// <typeparam name="T">值类型。</typeparam>
        /// <param name="key">键。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <returns>返回值。</returns>
        T GetValue<T>(string key, T defaultValue = default);

        /// <summary>
        /// 是否存在值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>返回是否存在值。</returns>
        bool HasValue(string key);

        /// <summary>
        /// 移除值。
        /// </summary>
        /// <param name="key">键。</param>
        void RemoveValue(string key);

        /// <summary>
        /// 尝试获取值。
        /// </summary>
        /// <typeparam name="T">值类型。</typeparam>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <returns>返回是否成功。</returns>
        bool TryGetValue<T>(string key, out T value);

        /// <summary>
        /// 设置对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        void SetObject(string key, object value);

        /// <summary>
        /// 获取对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>返回对象。</returns>
        object GetObject(string key);

        /// <summary>
        /// 尝试获取对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <returns>返回是否成功。</returns>
        bool TryGetObject(string key, out object value);

        /// <summary>
        /// 移除对象。
        /// </summary>
        /// <param name="key">键。</param>
        void RemoveObject(string key);

        /// <summary>
        /// 清空。
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// 黑板实现。
    /// </summary>
    public sealed class Blackboard : IBlackboard, IReference
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
            ReferencePool.Release(this);
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

        /// <summary>
        /// 创建黑板实例。
        /// </summary>
        public static Blackboard Create()
        {
            return ReferencePool.Acquire<Blackboard>();
        }
    }
}