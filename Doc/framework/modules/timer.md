# TimerModule

## 职责

全局帧定时器与秒定时器；也可创建局部 `ITimerDispatcher`。

## 关键类型

- `TimerModule`：实现 `ITimerCore`（转发内部 dispatcher）；`CreateTimerDispatcher`
- `ITimerCore`：`AddFrameTimer`、`AddTimer`、`RemoveTimer`
- `ITimerDispatcher`：另加 `Update`，需 `Dispose`

回调异常打 `Log.Fatal`，不会撑死调度。

## 用法

```csharp
int id = TimerModule.Instance.AddTimer(1f, OnTick);
TimerModule.Instance.RemoveTimer(id);

var local = TimerModule.Instance.CreateTimerDispatcher();
// 自己在合适的地方 Update；用完 Dispose
```

玩法侧另有 `GameTimerManager`，优先用 Context 内计时，避免和暂停状态打架。

## 扩展点

局部 dispatcher 用于需要独立时间轴或随对象释放的场景。

## 约束与坑

- 帧间隔最小为 1，秒间隔必须大于 0，否则抛 `InvalidParam`（旧版本 interval≤0 会导致每帧触发或时间倒退）。
- **暂停语义**：`elapseSeconds <= 0`（如 timeScale=0）时定时器整体不前进，帧定时器也不再每帧触发。
- 局部调度器不跟模块 Update，忘了 `Update` 就不会触发。
- 用完必须 `Dispose`（`Clear`/`Dispose` 会把存活的 `TimerInfo` 归还引用池）。
- `ITimerCore.Delay` 的取消回调注册句柄在完成/取消时注销，长寿命 token 源不会累积泄漏。

## 相关源码

- `Client/Assets/HoweFramework/Timer/TimerModule.cs`
- `Client/Assets/HoweFramework/Timer/ITimerCore.cs`
- `Client/Assets/HoweFramework/Timer/TimerDispatcher.cs`
