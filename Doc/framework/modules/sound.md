# SoundModule

## 职责

按声音组播放/停止/暂停。`GameApp` 使用 `UseAudioClipSound()`。

## 关键类型

- `SoundModule`：`SetSoundHelper`、`CreateSoundGroup`、`DestroySoundGroup`、`PlaySound`（返回 serialId）、`StopSound`、`PauseSound`/`ResumeSound`、`StopAllSounds`
- `ISoundHelper` / `ISoundGroupHelper`
- 业务：`GameMain/Scripts/Sound/` 下 `SoundGroupId`、`SoundUtility`

组已存在：`SoundGroupAlreadyExists`。组/声音不存在：602/603。

## 用法

```csharp
int id = SoundModule.Instance.PlaySound(groupId, assetName);
SoundModule.Instance.StopSound(id);
```

## 扩展点

换 `ISoundHelper` 可接非 AudioClip 方案。默认音量与数量上限见 `SoundConstant`。

## 约束与坑

- 先建组再播放。
- 资源加载依赖 Res 管线已初始化。

## 相关源码

- `Client/Assets/HoweFramework/Sound/SoundModule.cs`
- `Client/Assets/HoweFramework/Sound/AudioClip/`
- `Client/Assets/GameMain/Scripts/Sound/`
