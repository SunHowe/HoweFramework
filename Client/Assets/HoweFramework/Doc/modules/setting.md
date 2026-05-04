# 设置系统

## 概述

SettingModule 提供持久化设置存储，基于 PlayerPrefs 实现。

## 核心文件

| 文件 | 路径 |
|------|------|
| SettingModule | `Assets/HoweFramework/Setting/SettingModule.cs` |

## SettingModule API

```csharp
public sealed class SettingModule : ModuleBase<SettingModule>
{
    // 设置助手
    public void SetSettingHelper(ISettingHelper settingHelper);

    // 加载/保存
    public bool Load();
    public bool Save();

    // 检查/删除
    public bool HasSetting(string settingName);
    public bool RemoveSetting(string settingName);
    public void RemoveAllSettings();

    // Bool
    public bool GetBool(string settingName, bool defaultValue = false);
    public void SetBool(string settingName, bool value);

    // Int
    public int GetInt(string settingName, int defaultValue = 0);
    public void SetInt(string settingName, int value);

    // Float
    public float GetFloat(string settingName, float defaultValue = 0f);
    public void SetFloat(string settingName, float value);

    // String
    public string GetString(string settingName, string defaultValue = "");
    public void SetString(string settingName, string value);

    // Object（JSON 序列化）
    public T GetObject<T>(string settingName, T defaultObj = default);
    public void SetObject<T>(string settingName, T obj);
}
```

## 基本用法

### 1. 设置值

```csharp
// 布尔
SettingModule.Instance.SetBool("SoundEnabled", true);
SettingModule.Instance.SetInt("QualityLevel", 2);
SettingModule.Instance.SetFloat("MusicVolume", 0.8f);
SettingModule.Instance.SetString("PlayerName", "Player1");
```

### 2. 获取值

```csharp
bool soundEnabled = SettingModule.Instance.GetBool("SoundEnabled", true);
int quality = SettingModule.Instance.GetInt("QualityLevel", 1);
float volume = SettingModule.Instance.GetFloat("MusicVolume", 0.5f);
string name = SettingModule.Instance.GetString("PlayerName", "Unknown");
```

### 3. 对象存储

```csharp
// 存储对象（自动 JSON 序列化）
var playerData = new PlayerData { Level = 10, Gold = 1000 };
SettingModule.Instance.SetObject("PlayerData", playerData);

// 读取对象
var loaded = SettingModule.Instance.GetObject("PlayerData", new PlayerData());
```

### 4. 持久化

```csharp
// 保存到磁盘
SettingModule.Instance.Save();

// 加载
SettingModule.Instance.Load();
```

### 5. 删除

```csharp
// 删除单个设置
SettingModule.Instance.RemoveSetting("TempData");

// 删除所有设置
SettingModule.Instance.RemoveAllSettings();
```

## 完整示例

### 设置管理器

```csharp
public class SettingsManager
{
    private const string KEY_SOUND = "Settings.SoundEnabled";
    private const string KEY_MUSIC = "Settings.MusicVolume";
    private const string KEY_QUALITY = "Settings.Quality";
    private const string KEY_LANGUAGE = "Settings.Language";
    private const string KEY_PLAYER_DATA = "Settings.PlayerData";

    public bool SoundEnabled { get; private set; }
    public float MusicVolume { get; private set; }
    public int QualityLevel { get; private set; }
    public string Language { get; private set; }
    public PlayerData PlayerData { get; private set; }

    public void Load()
    {
        SoundEnabled = SettingModule.Instance.GetBool(KEY_SOUND, true);
        MusicVolume = SettingModule.Instance.GetFloat(KEY_MUSIC, 0.7f);
        QualityLevel = SettingModule.Instance.GetInt(KEY_QUALITY, 1);
        Language = SettingModule.Instance.GetString(KEY_LANGUAGE, "en");
        PlayerData = SettingModule.Instance.GetObject(KEY_PLAYER_DATA, new PlayerData());
    }

    public void Save()
    {
        SettingModule.Instance.SetBool(KEY_SOUND, SoundEnabled);
        SettingModule.Instance.SetFloat(KEY_MUSIC, MusicVolume);
        SettingModule.Instance.SetInt(KEY_QUALITY, QualityLevel);
        SettingModule.Instance.SetString(KEY_LANGUAGE, Language);
        SettingModule.Instance.SetObject(KEY_PLAYER_DATA, PlayerData);
        SettingModule.Instance.Save();
    }

    public void SetSoundEnabled(bool enabled)
    {
        SoundEnabled = enabled;
        // 立即生效
        AudioManager.Instance.SetSoundEnabled(enabled);
    }

    public void SetMusicVolume(float volume)
    {
        MusicVolume = Mathf.Clamp01(volume);
        AudioManager.Instance.SetMusicVolume(MusicVolume);
    }
}
```

### 游戏退出时保存

```csharp
public class GameEntry : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        SettingsManager.Instance.Save();
    }
}
```

## 最佳实践

### 1. 使用常量定义键名

```csharp
private static class SettingKeys
{
    public const string SoundEnabled = "Audio.SoundEnabled";
    public const string MusicVolume = "Audio.MusicVolume";
    public const string Quality = "Graphics.Quality";
}

// 使用
SettingModule.Instance.SetBool(SettingKeys.SoundEnabled, true);
```

### 2. 避免频繁 Save

```csharp
// 推荐：退出时保存
private void OnApplicationPause(bool pauseStatus)
{
    if (pauseStatus)
        SettingsManager.Instance.Save();
}

// 不推荐：每次修改都保存
SettingModule.Instance.SetInt("Score", score);
SettingModule.Instance.Save(); // 频繁 IO
```

### 3. 提供默认值

```csharp
// 有默认值
int quality = SettingModule.Instance.GetInt("Quality", 1);

// 无默认值
int value = SettingModule.Instance.GetInt("SomeKey"); // 可能为 0
```