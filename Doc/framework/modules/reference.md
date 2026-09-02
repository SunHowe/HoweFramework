# ReferencePool

## 职责

静态引用池。`Acquire` / `Release`（Release 会先 `Clear`）。事件、请求、实体、组件都依赖它。

## 关键类型

- `ReferencePool`：`Acquire<T>`、`Release`、`ClearCache<T>`、`ClearAllCache`
- `IReference.Clear`
- `IReferenceWithId` / `ReferenceRef` 等

Editor 或 Development 下 Acquire 会检查类型必须是 class 且实现 `IReference`。

## 用法

```csharp
var e = ReferencePool.Acquire<MyEventArgs>();
ReferencePool.Release(e);
```

`RequestBase.Execute` 的 `finally` 会 Release 自身。

## 扩展点

实现 `IReference`，提供无参构造。`Clear` 必须把字段复位到可再次 Acquire 的状态。

## 约束与坑

- Release 后不要再用该实例。
- `Clear` 若漏掉集合/事件，会脏数据串实例。
- 这是静态池，不是 Module；`GameApp` 销毁不会自动 `ClearAllCache`。

## 相关源码

- `Client/Assets/HoweFramework/Reference/ReferencePool.cs`
