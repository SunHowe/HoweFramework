# LocalizationModule

## 职责

当前语言、文本查找、多数据源。改语言会写入本地并派发 `LocalizationLanguageUpdateEventArgs`。

## 关键类型

- `LocalizationModule`：`Language`、`DefaultLanguage`（`ChineseSimplified`）、`SystemLanguage`、`GetText`、`AddText`、`ClearText`、`AddSource`/`RemoveSource`、`LoadAsync`
- `ILocalizationSource`（项目有 `LubanLocalizationSource`）

`Language` 不能设为 `Unspecified`。

## 用法

```csharp
var s = LocalizationModule.Instance.GetText("some.key");
LocalizationModule.Instance.Language = Language.English;
```

启动流程 `ProcedureLoadLocalization` 加载源。

## 扩展点

实现 `ILocalizationSource` 并 `AddSource`。运行时可用 `AddText` 覆盖/补充。

## 约束与坑

- 语言变化会先 `LoadAsync`（内部 `ClearText` 再加载源）再派发事件，订阅后立刻 `GetText` 能拿到新语言文本。
- `Language` setter 会 `SettingModule.Save()`，避免异常退出时语言未落盘。
- 缺 key 时 `GetText` 返回 `"<NoKey>{key}"`，不会抛错。
- 生成本地化表用 `DataTable/gen_client_localization.sh`，与主表脚本分开。
- `LoadAsync` 开始前会先 `ClearText`，重复加载/切换语言不会残留旧语言文本。
- 存档中的语言值经 `Enum.IsDefined` 校验，损坏时回退默认语言。

## 相关源码

- `Client/Assets/HoweFramework/Localization/LocalizationModule.cs`
- `Client/Assets/HoweFramework/Localization/LubanLocalizationSource.cs`
