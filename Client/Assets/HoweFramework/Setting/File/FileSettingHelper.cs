﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 基于文件流的游戏配置辅助器。
    /// </summary>
    public sealed class FileSettingHelper : ISettingHelper
    {
        private const string SettingFileName = "GameSetting.dat";

        private string m_FilePath = null;
        private FileSetting m_Settings = null;
        private FileSettingSerializer m_Serializer = null;

        /// <summary>
        /// 获取游戏配置项数量。
        /// </summary>
        public int Count
        {
            get
            {
                return m_Settings != null ? m_Settings.Count : 0;
            }
        }

        /// <summary>
        /// 获取游戏配置存储文件路径。
        /// </summary>
        public string FilePath
        {
            get
            {
                return m_FilePath;
            }
        }

        /// <summary>
        /// 获取游戏配置。
        /// </summary>
        public FileSetting Setting
        {
            get
            {
                return m_Settings;
            }
        }

        /// <summary>
        /// 获取游戏配置序列化器。
        /// </summary>
        public FileSettingSerializer Serializer
        {
            get
            {
                return m_Serializer;
            }
        }

        /// <summary>
        /// 加载游戏配置。
        /// </summary>
        /// <returns>是否加载游戏配置成功。</returns>
        public bool Load()
        {
            try
            {
                if (!File.Exists(m_FilePath))
                {
                    return true;
                }

                using (FileStream fileStream = new FileStream(m_FilePath, FileMode.Open, FileAccess.Read))
                {
                    m_Serializer.Deserialize(fileStream);
                    return true;
                }
            }
            catch (Exception exception)
            {
                Log.Warning($"Load settings failure with exception '{exception}'.");
                return false;
            }
        }

        /// <summary>
        /// 保存游戏配置。
        /// </summary>
        /// <returns>是否保存游戏配置成功。</returns>
        public bool Save()
        {
            try
            {
                using (FileStream fileStream = new FileStream(m_FilePath, FileMode.Create, FileAccess.Write))
                {
                    return m_Serializer.Serialize(fileStream, m_Settings);
                }
            }
            catch (Exception exception)
            {
                Log.Warning($"Save settings failure with exception '{exception}'.");
                return false;
            }
        }

        /// <summary>
        /// 获取所有游戏配置项的名称。
        /// </summary>
        /// <returns>所有游戏配置项的名称。</returns>
        public string[] GetAllSettingNames()
        {
            return m_Settings.GetAllSettingNames();
        }

        /// <summary>
        /// 获取所有游戏配置项的名称。
        /// </summary>
        /// <param name="results">所有游戏配置项的名称。</param>
        public void GetAllSettingNames(List<string> results)
        {
            m_Settings.GetAllSettingNames(results);
        }

        /// <summary>
        /// 检查是否存在指定游戏配置项。
        /// </summary>
        /// <param name="settingName">要检查游戏配置项的名称。</param>
        /// <returns>指定的游戏配置项是否存在。</returns>
        public bool HasSetting(string settingName)
        {
            return m_Settings.HasSetting(settingName);
        }

        /// <summary>
        /// 移除指定游戏配置项。
        /// </summary>
        /// <param name="settingName">要移除游戏配置项的名称。</param>
        /// <returns>是否移除指定游戏配置项成功。</returns>
        public bool RemoveSetting(string settingName)
        {
            return m_Settings.RemoveSetting(settingName);
        }

        /// <summary>
        /// 清空所有游戏配置项。
        /// </summary>
        public void RemoveAllSettings()
        {
            m_Settings.RemoveAllSettings();
        }

        /// <summary>
        /// 从指定游戏配置项中读取布尔值。
        /// </summary>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <param name="defaultValue">当指定的游戏配置项不存在时，返回此默认值。</param>
        /// <returns>读取的布尔值。</returns>
        public bool GetBool(string settingName, bool defaultValue = false)
        {
            return m_Settings.GetBool(settingName, defaultValue);
        }

        /// <summary>
        /// 向指定游戏配置项写入布尔值。
        /// </summary>
        /// <param name="settingName">要写入游戏配置项的名称。</param>
        /// <param name="value">要写入的布尔值。</param>
        public void SetBool(string settingName, bool value)
        {
            m_Settings.SetBool(settingName, value);
        }

        /// <summary>
        /// 从指定游戏配置项中读取整数值。
        /// </summary>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <param name="defaultValue">当指定的游戏配置项不存在时，返回此默认值。</param>
        /// <returns>读取的整数值。</returns>
        public int GetInt(string settingName, int defaultValue = 0)
        {
            return m_Settings.GetInt(settingName, defaultValue);
        }

        /// <summary>
        /// 向指定游戏配置项写入整数值。
        /// </summary>
        /// <param name="settingName">要写入游戏配置项的名称。</param>
        /// <param name="value">要写入的整数值。</param>
        public void SetInt(string settingName, int value)
        {
            m_Settings.SetInt(settingName, value);
        }

        /// <summary>
        /// 从指定游戏配置项中读取浮点数值。
        /// </summary>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <param name="defaultValue">当指定的游戏配置项不存在时，返回此默认值。</param>
        /// <returns>读取的浮点数值。</returns>
        public float GetFloat(string settingName, float defaultValue = 0f)
        {
            return m_Settings.GetFloat(settingName, defaultValue);
        }

        /// <summary>
        /// 向指定游戏配置项写入浮点数值。
        /// </summary>
        /// <param name="settingName">要写入游戏配置项的名称。</param>
        /// <param name="value">要写入的浮点数值。</param>
        public void SetFloat(string settingName, float value)
        {
            m_Settings.SetFloat(settingName, value);
        }

        /// <summary>
        /// 从指定游戏配置项中读取字符串值。
        /// </summary>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <param name="defaultValue">当指定的游戏配置项不存在时，返回此默认值。</param>
        /// <returns>读取的字符串值。</returns>
        public string GetString(string settingName, string defaultValue = "")
        {
            return m_Settings.GetString(settingName, defaultValue);
        }

        /// <summary>
        /// 向指定游戏配置项写入字符串值。
        /// </summary>
        /// <param name="settingName">要写入游戏配置项的名称。</param>
        /// <param name="value">要写入的字符串值。</param>
        public void SetString(string settingName, string value)
        {
            m_Settings.SetString(settingName, value);
        }

        /// <summary>
        /// 从指定游戏配置项中读取对象。
        /// </summary>
        /// <typeparam name="T">要读取对象的类型。</typeparam>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <param name="defaultObj">当指定的游戏配置项不存在时，返回此默认对象。</param>
        /// <returns>读取的对象。</returns>
        public T GetObject<T>(string settingName, T defaultObj = default)
        {
            string json = GetString(settingName, null);
            if (json == null)
            {
                return defaultObj;
            }

            return JsonUtility.ToObject<T>(json);
        }

        /// <summary>
        /// 从指定游戏配置项中读取对象。
        /// </summary>
        /// <param name="objectType">要读取对象的类型。</param>
        /// <param name="settingName">要获取游戏配置项的名称。</param>
        /// <param name="defaultObj">当指定的游戏配置项不存在时，返回此默认对象。</param>
        /// <returns>读取的对象。</returns>
        public object GetObject(Type objectType, string settingName, object defaultObj = null)
        {
            string json = GetString(settingName, null);
            if (json == null)
            {
                return defaultObj;
            }

            return JsonUtility.ToObject(objectType, json);
        }

        /// <summary>
        /// 向指定游戏配置项写入对象。
        /// </summary>
        /// <typeparam name="T">要写入对象的类型。</typeparam>
        /// <param name="settingName">要写入游戏配置项的名称。</param>
        /// <param name="obj">要写入的对象。</param>
        public void SetObject<T>(string settingName, T obj)
        {
            SetString(settingName, JsonUtility.ToJson(obj));
        }

        /// <summary>
        /// 向指定游戏配置项写入对象。
        /// </summary>
        /// <param name="settingName">要写入游戏配置项的名称。</param>
        /// <param name="obj">要写入的对象。</param>
        public void SetObject(string settingName, object obj)
        {
            SetString(settingName, JsonUtility.ToJson(obj));
        }

        public FileSettingHelper()
        {
            m_FilePath = PathUtility.GetRegularPath(Path.Combine(Application.persistentDataPath, SettingFileName));
            m_Settings = new FileSetting();
            m_Serializer = new FileSettingSerializer();
            m_Serializer.RegisterSerializeCallback(0, SerializeFileSettingCallback);
            m_Serializer.RegisterDeserializeCallback(0, DeserializeFileSettingCallback);
        }

        public void Dispose()
        {
            Save();
        }

        private bool SerializeFileSettingCallback(Stream stream, FileSetting fileSetting)
        {
            fileSetting.Serialize(stream);
            return true;
        }

        private FileSetting DeserializeFileSettingCallback(Stream stream)
        {
            m_Settings.Deserialize(stream);
            return m_Settings;
        }
    }
}
