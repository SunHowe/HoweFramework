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

- 语言变化走事件，UI 需要订阅才能刷新。
- 缺 key 时 `GetText` 返回 `"<NoKey>{key}"`，不会抛错。
- 生成本地化表用 `DataTable/gen_client_localization.sh`，与主表脚本分开。

## 相关源码

- `Client/Assets/HoweFramework/Localization/LocalizationModule.cs`
- `Client/Assets/HoweFramework/Localization/LubanLocalizationSource.cs`
