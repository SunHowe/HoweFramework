# EventModule

## 职责

全局线程安全事件调度；也可创建局部调度器。事件参数走引用池。

## 关键类型

- `EventModule`：`Subscribe` / `Unsubscribe` / `Dispatch` / `ThreadSafeDispatch`；`CreateEventDispatcher`、`CreateThreadSafeEventDispatcher`、`CreatePriorityEventDispatcher`
- `GameEventArgs`：`Id`（具体类型的运行时 `TypeId`，基类构造时写入）、`Clear`（复位 `IsReleaseAfterFire` 后调 `OnClear`）、`SetIsReleaseAfterFire` / `IsReleaseAfterFire`（默认 true），实现 `IReference`
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

事件类通常 `ReferencePool.Acquire`。静态 `EventId` 写成 `TypeId<MyEventArgs>.Id`，与实例 `Id` 同一套。订阅也可以直接 `TypeId<MyEventArgs>.Id`。派生类只重写 `OnClear` 清字段，不要重写 `Clear`，也不要再给 `Id` 赋值。

## 扩展点

局部玩法用 Context 上的 `IEventDispatcher`，不要把玩法事件打到全局除非确实是应用级。

## 约束与坑

- 局部调度器必须 `Dispose`。
- `IsReleaseAfterFire` 为 true 时派发后回收，持有事件引用会变成已 Clear 的对象。需要在 Handler 之后继续持有实例时（例如 Packet 当远程响应），调用 `SetIsReleaseAfterFire(false)`；`Clear` 会把该标记复位为 true。
- `Packet` 继承 `GameEventArgs`，可用同一套调度；`Packet.Id` 同样是 TypeId。
- 优先级调度器（`CreatePriorityEventDispatcher`）按优先级从高到低派发，同优先级按订阅先后（FIFO）。其底层 `MultiSortedDictionary` 的排序插入曾整体失效（[5,3] 插 4 得 [4,5,3]），已修复；升级框架后若业务曾依赖错误顺序需复查。
- `Unsubscribe(id, handler)` 在派发进行中调用是安全的（缓存节点延迟跳过）；同一委托订阅多个不同事件时，退订只影响指定 id 的事件，不会跨事件误伤。
- `ThreadSafeEventDispatcher` 单事件派发异常只记日志并继续处理队列剩余事件，不上抛主循环；`ClearEvents`/`Dispose` 会把队列中未处理的事件项与事件参数一并归还引用池。
- `SimpleEvent` 在最后一个订阅者的 Handler 内 `Unsubscribe` 自身是安全的（一次性订阅写法）。

## 相关源码

- `Client/Assets/HoweFramework/Event/EventModule.cs`
- `Client/Assets/HoweFramework/Event/Core/GameEventArgs.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Framework/Event/GameStartEventArgs.cs`
