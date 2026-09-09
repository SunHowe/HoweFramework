# BaseModule

## 职责

注册运行时程序集；提供 Json 与文本模板辅助器入口。另含 TypeId、黑板、序列化基类等基础原语（同目录，不一定都经 BaseModule 转发）。

## 关键类型

- `BaseModule`：`SetJsonHelper`、`SetTextTemplateHelper`；`OnInit` 里 `AssemblyUtility.RegisterRuntimeAssembly`
- `JsonUtility` / `IJsonHelper`（`UseUnityJsonHelper`）
- `TextUtility` / `ITextTemplateHelper`（`UseDefaultTextTemplateHelper`）：模板 `{name=默认}` 
- `TypeId` / `TypeId<T>.Id`：运行时递增类型 id
- `Blackboard` / `BlackboardComponent`：实体与场景物体桥接（Gameplay 用 `"GameEntity"` 键）
- `SerializerBase`、`ILoadable`

`GameApp` 对 BaseModule 链式调用了 Unity Json 与默认文本模板。

## 用法

```csharp
TextUtility.ParseTemplate(template, dictionary);
var id = TypeId<MyType>.Id;
```

## 扩展点

`SetJsonHelper` 换 Newtonsoft 等。`TypeId` 是运行时分配，不要当协议号持久化。

## 约束与坑

- 模块销毁会 Dispose Json/文本 Helper 并 `AssemblyUtility.Clear`。
- `GameEventArgs.Id` 与静态 `EventId` 都走 `TypeId`，订阅用 `TypeId<T>.Id` 或 `T.EventId`，不要再用 `GetHashCode()`。
- **模块生命周期容错**：`ModuleBase.Init` 中 `OnInit`/`IOC Register` 抛异常会回滚 `Instance`；`Destroy` 中 `OnDestroy` 抛异常也会清空 `Instance`。`GameApp` 构造失败会销毁已成功模块；`GameApp.Destroy` 逐模块 try-catch，单个模块销毁异常不中断销毁链。
- `LoadableGroup.LoadAsync` 的进度回调按各任务权重加权（`WeightRate`），修复了旧版本绕过权重包装导致进度失真的问题。
- 空 `LoadableGroup`（未添加任务）`LoadAsync` 直接完成并回报进度 1，不会除零。

## 相关源码

- `Client/Assets/HoweFramework/Base/BaseModule.cs`
- `Client/Assets/HoweFramework/Base/TypeId.cs`
- `Client/Assets/HoweFramework/Utility/JsonUtility.cs`
- `Client/Assets/HoweFramework/Utility/TextUtility.cs`
- `Client/Assets/HoweFramework/Base/Blackboard.cs`
