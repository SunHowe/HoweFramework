# EventModule

## 职责

全局线程安全事件调度；也可创建局部调度器。事件参数走引用池。

## 关键类型

- `EventModule`：`Subscribe` / `Unsubscribe` / `Dispatch` / `ThreadSafeDispatch`；`CreateEventDispatcher`、`CreateThreadSafeEventDispatcher`、`CreatePriorityEventDispatcher`
- `GameEventArgs`：`Id`、`Clear`、`IsReleaseAfterFire`（默认 true），实现 `IReference`
- `GameEventHandler`

全局调度器在 `OnInit` 设为 `AllowMultiHandler | AllowNoHandler`。`OnUpdate` 调用 `UpdateEvents`。

## 用法

```csharp
EventModule.Instance.Subscribe(MyEventArgs.EventId, OnMyEvent);
EventModule.Instance.Dispatch(this, MyEventArgs.Create());
EventModule.Instance.Unsubscribe(MyEventArgs.EventId, OnMyEvent);

var local = EventModule.Instance.CreateEventDispatcher();
// 用完
local.Dispose();
```

事件类通常 `ReferencePool.Acquire` + 静态 `EventId`。Gameplay 示例：`GameStartEventArgs` 用 `typeof(GameStartEventArgs).GetHashCode()` 作为 `EventId`。

## 扩展点

局部玩法用 Context 上的 `IEventDispatcher`，不要把玩法事件打到全局除非确实是应用级。

## 约束与坑

- 局部调度器必须 `Dispose`。
- `IsReleaseAfterFire` 为 true 时派发后回收，持有事件引用会变成已 Clear 的对象。
- `Packet` 继承 `GameEventArgs`，可用同一套调度。

## 相关源码

- `Client/Assets/HoweFramework/Event/EventModule.cs`
- `Client/Assets/HoweFramework/Event/Core/GameEventArgs.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Framework/Event/GameStartEventArgs.cs`
