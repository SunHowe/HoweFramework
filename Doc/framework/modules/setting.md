# SettingModule

## 职责

键值设置。`GameApp` 使用 `UsePlayerPrefsSetting()`。支持 bool/int/float/string 与对象（经 Json 辅助器）。

## 关键类型

- `SettingModule`：`SetSettingHelper`、`Get*`/`Set*`、`RemoveAllSettings`、`Count`
- `ISettingHelper`（另有 `FileSetting` 文件实现）

## 用法

```csharp
SettingModule.Instance.SetInt("volume", 8);
var v = SettingModule.Instance.GetInt("volume", 5);
SettingModule.Instance.SetObject("cfg", myObj);
```

对象序列化依赖 `BaseModule` 已设置 Json Helper。

## 扩展点

`SetSettingHelper` 换成文件或自定义存储。语言模块会把语言存本地（与 Setting 配合，见 Localization 实现）。

## 约束与坑

- 未设 Helper 时调用会 NRE（源码直接转发 `m_SettingHelper`）。
- PlayerPrefs 不适合大数据。
- `FileSetting` 的数值读写使用 `InvariantCulture` 且 `TryParse` 容错：存档损坏时读取回退默认值并告警，不再抛 `FormatException`；浮点序列化不再受系统区域文化影响。

## 相关源码

- `Client/Assets/HoweFramework/Setting/SettingModule.cs`
- `Client/Assets/HoweFramework/Setting/PlayerPrefsSettingHelper.cs`
- `Client/Assets/HoweFramework/Setting/File/FileSetting.cs`
