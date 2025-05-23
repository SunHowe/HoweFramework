﻿using System.Collections.Generic;
using System.IO;

namespace HoweFramework
{
    /// <summary>
    /// 序列化器基类。
    /// </summary>
    /// <typeparam name="T">要序列化的数据类型。</typeparam>
    public abstract class SerializerBase<T>
    {
        private readonly Dictionary<byte, SerializeCallback> m_SerializeCallbacks;
        private readonly Dictionary<byte, DeserializeCallback> m_DeserializeCallbacks;
        private readonly Dictionary<byte, TryGetValueCallback> m_TryGetValueCallbacks;
        private byte m_LatestSerializeCallbackVersion;

        /// <summary>
        /// 初始化游戏框架序列化器基类的新实例。
        /// </summary>
        public SerializerBase()
        {
            m_SerializeCallbacks = new Dictionary<byte, SerializeCallback>();
            m_DeserializeCallbacks = new Dictionary<byte, DeserializeCallback>();
            m_TryGetValueCallbacks = new Dictionary<byte, TryGetValueCallback>();
            m_LatestSerializeCallbackVersion = 0;
        }

        /// <summary>
        /// 序列化回调函数。
        /// </summary>
        /// <param name="stream">目标流。</param>
        /// <param name="data">要序列化的数据。</param>
        /// <returns>是否序列化数据成功。</returns>
        public delegate bool SerializeCallback(Stream stream, T data);

        /// <summary>
        /// 反序列化回调函数。
        /// </summary>
        /// <param name="stream">指定流。</param>
        /// <returns>反序列化的数据。</returns>
        public delegate T DeserializeCallback(Stream stream);

        /// <summary>
        /// 尝试从指定流获取指定键的值回调函数。
        /// </summary>
        /// <param name="stream">指定流。</param>
        /// <param name="key">指定键。</param>
        /// <param name="value">指定键的值。</param>
        /// <returns>是否从指定流获取指定键的值成功。</returns>
        public delegate bool TryGetValueCallback(Stream stream, string key, out object value);

        /// <summary>
        /// 注册序列化回调函数。
        /// </summary>
        /// <param name="version">序列化回调函数的版本。</param>
        /// <param name="callback">序列化回调函数。</param>
        public void RegisterSerializeCallback(byte version, SerializeCallback callback)
        {
            if (callback == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "Serialize callback is invalid.");
            }

            m_SerializeCallbacks[version] = callback;
            if (version > m_LatestSerializeCallbackVersion)
            {
                m_LatestSerializeCallbackVersion = version;
            }
        }

        /// <summary>
        /// 注册反序列化回调函数。
        /// </summary>
        /// <param name="version">反序列化回调函数的版本。</param>
        /// <param name="callback">反序列化回调函数。</param>
        public void RegisterDeserializeCallback(byte version, DeserializeCallback callback)
        {
            if (callback == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "Deserialize callback is invalid.");
            }

            m_DeserializeCallbacks[version] = callback;
        }

        /// <summary>
        /// 注册尝试从指定流获取指定键的值回调函数。
        /// </summary>
        /// <param name="version">尝试从指定流获取指定键的值回调函数的版本。</param>
        /// <param name="callback">尝试从指定流获取指定键的值回调函数。</param>
        public void RegisterTryGetValueCallback(byte version, TryGetValueCallback callback)
        {
            if (callback == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, "Try get value callback is invalid.");
            }

            m_TryGetValueCallbacks[version] = callback;
        }

        /// <summary>
        /// 序列化数据到目标流中。
        /// </summary>
        /// <param name="stream">目标流。</param>
        /// <param name="data">要序列化的数据。</param>
        /// <returns>是否序列化数据成功。</returns>
        public bool Serialize(Stream stream, T data)
        {
            if (m_SerializeCallbacks.Count <= 0)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "No serialize callback registered.");
            }

            return Serialize(stream, data, m_LatestSerializeCallbackVersion);
        }

        /// <summary>
        /// 序列化数据到目标流中。
        /// </summary>
        /// <param name="stream">目标流。</param>
        /// <param name="data">要序列化的数据。</param>
        /// <param name="version">序列化回调函数的版本。</param>
        /// <returns>是否序列化数据成功。</returns>
        public bool Serialize(Stream stream, T data, byte version)
        {
            byte[] header = GetHeader();
            stream.WriteByte(header[0]);
            stream.WriteByte(header[1]);
            stream.WriteByte(header[2]);
            stream.WriteByte(version);
            SerializeCallback callback = null;
            if (!m_SerializeCallbacks.TryGetValue(version, out callback))
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, $"Serialize callback '{version}' is not exist.");
            }

            return callback(stream, data);
        }

        /// <summary>
        /// 从指定流反序列化数据。
        /// </summary>
        /// <param name="stream">指定流。</param>
        /// <returns>反序列化的数据。</returns>
        public T Deserialize(Stream stream)
        {
            byte[] header = GetHeader();
            byte header0 = (byte)stream.ReadByte();
            byte header1 = (byte)stream.ReadByte();
            byte header2 = (byte)stream.ReadByte();
            if (header0 != header[0] || header1 != header[1] || header2 != header[2])
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, $"Header is invalid, need '{header[0]}{header[1]}{header[2]}', current '{header0}{header1}{header2}'.");
            }

            byte version = (byte)stream.ReadByte();
            DeserializeCallback callback = null;
            if (!m_DeserializeCallbacks.TryGetValue(version, out callback))
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, $"Deserialize callback '{version}' is not exist.");
            }

            return callback(stream);
        }

        /// <summary>
        /// 尝试从指定流获取指定键的值。
        /// </summary>
        /// <param name="stream">指定流。</param>
        /// <param name="key">指定键。</param>
        /// <param name="value">指定键的值。</param>
        /// <returns>是否从指定流获取指定键的值成功。</returns>
        public bool TryGetValue(Stream stream, string key, out object value)
        {
            value = null;
            byte[] header = GetHeader();
            byte header0 = (byte)stream.ReadByte();
            byte header1 = (byte)stream.ReadByte();
            byte header2 = (byte)stream.ReadByte();
            if (header0 != header[0] || header1 != header[1] || header2 != header[2])
            {
                return false;
            }

            byte version = (byte)stream.ReadByte();
            TryGetValueCallback callback = null;
            if (!m_TryGetValueCallbacks.TryGetValue(version, out callback))
            {
                return false;
            }

            return callback(stream, key, out value);
        }

        /// <summary>
        /// 获取数据头标识。
        /// </summary>
        /// <returns>数据头标识。</returns>
        protected abstract byte[] GetHeader();
    }
}
