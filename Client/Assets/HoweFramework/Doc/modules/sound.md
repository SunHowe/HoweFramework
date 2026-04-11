# 声音系统

## 概述

SoundModule 管理游戏音频播放，支持多音效组和音量控制。

## 核心文件

| 文件 | 路径 |
|------|------|
| SoundModule | `Assets/HoweFramework/Sound/SoundModule.cs` |

## SoundModule API

```csharp
public sealed class SoundModule : ModuleBase<SoundModule>
{
    // 主音量
    public float Volume { get; set; }

    // 设置音效助手
    public void SetSoundHelper(ISoundHelper soundHelper);

    // 创建音效组
    public void CreateSoundGroup(int groupId, string groupName, float volume = 1f, int soundLimit = 8);

    // 销毁音效组
    public void DestroySoundGroup(int groupId);

    // 播放音效
    public int PlaySound(int groupId, string soundAssetName, float volume = 1f, bool loop = false);

    // 停止音效
    public void StopSound(int serialId);
    public void StopSound(string soundAssetName, int groupId = 0);
    public void StopAllSounds(int groupId = 0);

    // 暂停/恢复
    public void PauseSound(int serialId);
    public void ResumeSound(int serialId);

    // 检查状态
    public bool IsLoadingSound(int serialId);
}
```

## ISoundHelper 接口

```csharp
public interface ISoundHelper : IDisposable
{
    int PlaySound(string soundAssetName, float volume, bool loop);
    void StopSound(int serialId);
    void PauseSound(int serialId);
    void ResumeSound(int serialId);
    bool IsLoadingSound(int serialId);
    void SetGroupVolume(int groupId, float volume);
}
```

## 基本用法

### 1. 初始化

```csharp
// 设置音效助手（通常在 ProcedureSplash）
SoundModule.Instance.SetSoundHelper(new MySoundHelper());

// 创建音效组
SoundModule.Instance.CreateSoundGroup(0, "BGM", 0.8f, 2);
SoundModule.Instance.CreateSoundGroup(1, "SFX", 1.0f, 8);
```

### 2. 播放音效

```csharp
// 播放 BGM
int bgmId = SoundModule.Instance.PlaySound(0, "Sounds/BGM/main_theme", 0.5f, true);

// 播放音效
int sfxId = SoundModule.Instance.PlaySound(1, "Sounds/SFX/click", 1.0f, false);

// 获取返回的 serialId 用于后续控制
```

### 3. 停止音效

```csharp
// 停止指定音效
SoundModule.Instance.StopSound(bgmId);

// 通过名称停止
SoundModule.Instance.StopSound("Sounds/BGM/main_theme", 0);

// 停止组内所有音效
SoundModule.Instance.StopAllSounds(0); // 停止所有 BGM
```

### 4. 暂停/恢复

```csharp
// 暂停
SoundModule.Instance.PauseSound(sfxId);

// 恢复
SoundModule.Instance.ResumeSound(sfxId);
```

## 完整示例

### 音频管理器

```csharp
public class AudioManager
{
    private const int BGM_GROUP = 0;
    private const int SFX_GROUP = 1;

    public void Initialize()
    {
        SoundModule.Instance.CreateSoundGroup(BGM_GROUP, "BGM", 0.6f, 1);
        SoundModule.Instance.CreateSoundGroup(SFX_GROUP, "SFX", 1.0f, 8);
    }

    public void PlayBGM(string assetName)
    {
        SoundModule.Instance.PlaySound(BGM_GROUP, assetName, 0.6f, true);
    }

    public void StopBGM()
    {
        SoundModule.Instance.StopAllSounds(BGM_GROUP);
    }

    public void PlaySFX(string assetName)
    {
        SoundModule.Instance.PlaySound(SFX_GROUP, assetName, 1.0f, false);
    }

    public void PlayUISound(string assetName)
    {
        // UI 音效使用独立组
        SoundModule.Instance.PlaySound(2, assetName, 0.8f, false);
    }
}
```

### 静音功能

```csharp
public void MuteAll()
{
    SoundModule.Instance.Volume = 0f;
}

public void UnmuteAll()
{
    SoundModule.Instance.Volume = 1f;
}

public void MuteSFXOnly()
{
    SoundModule.Instance.StopAllSounds(SFX_GROUP);
}
```

## 最佳实践

### 1. 创建多个音效组

```csharp
// BGM 组
SoundModule.Instance.CreateSoundGroup(0, "BGM", 0.7f, 1);

// 音效组
SoundModule.Instance.CreateSoundGroup(1, "SFX", 1.0f, 10);

// UI 音效组
SoundModule.Instance.CreateSoundGroup(2, "UISFX", 0.8f, 5);

// 环境音组
SoundModule.Instance.CreateSoundGroup(3, "Ambient", 0.5f, 2);
```

### 2. 资源命名规范

```csharp
// 推荐命名
"Sounds/BGM/battle_theme"
"Sounds/SFX/weapon/shoot"
"Sounds/SFX/ui/click"
```

### 3. 使用 serialId 管理

```csharp
private int _currentBGMId;

public void PlayBattleBGM()
{
    // 停止之前的
    if (_currentBGMId != 0)
        SoundModule.Instance.StopSound(_currentBGMId);

    // 播放新的
    _currentBGMId = SoundModule.Instance.PlaySound(BGM_GROUP, "battle", 0.7f, true);
}

public void StopAll()
{
    if (_currentBGMId != 0)
        SoundModule.Instance.StopSound(_currentBGMId);
}
```