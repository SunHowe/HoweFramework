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
- **音量公式**：实际音量 = 全局音量 × 组音量 × 播放参数音量。`SoundModule.Volume` 只改全局音量（`SetGlobalVolume`），不会覆盖各组独立音量。
- **组编号 0 是全组通配**：`StopAllSounds()` / `StopSound(assetName)` 默认参数 0 表示遍历所有声音组。
- 播放中的声音计时使用真实流逝时间（`realElapseSeconds`），`timeScale=0` 暂停游戏不会截断声音播放。
- 加载中的声音可以 `PauseSound`/`ResumeSound`/`StopSound`，加载完成后生效或取消；加载失败（资源缺失）会静默清理并卸载空引用，不会残留记录。
- `DestroySoundGroup` 会先停止组内所有播放中的声音并归还资源引用。

## 相关源码

- `Client/Assets/HoweFramework/Sound/SoundModule.cs`
- `Client/Assets/HoweFramework/Sound/AudioClip/`
- `Client/Assets/GameMain/Scripts/Sound/`
