# 事件系统

## 概述

EventModule 提供框架内的事件发布-订阅机制，实现模块间松耦合通信。

## 核心文件

| 文件 | 路径 |
|------|------|
| EventModule | `Assets/HoweFramework/Event/EventModule.cs` |
| GameEventArgs | `Assets/HoweFramework/Event/Core/GameEventArgs.cs` |
| IEventDispatcher | `Assets/HoweFramework/Event/Core/IEventDispatcher.cs` |
| EventDispatcher | `Assets/HoweFramework/Event/Internal/EventDispatcher.cs` |
| ThreadSafeEventDispatcher | `Assets/HoweFramework/Event/Internal/ThreadSafeEventDispatcher.cs` |

## GameEventArgs

所有事件参数继承自 `GameEventArgs`：

```csharp
public abstract class GameEventArgs : EventArgs, IReference
{
    public abstract int Id { get; }

    // 触发后是否自动归还池（默认 true）
    public virtual bool IsReleaseAfterFire => true;

    public abstract void Clear();
}

public delegate void GameEventHandler(object sender, GameEventArgs e);
```

## 创建自定义事件

```csharp
public sealed class PlayerHealthChangedEventArgs : GameEventArgs
{
    // 使用类型哈希作为事件 ID
    public static readonly int EventId = typeof(PlayerHealthChangedEventArgs).GetHashCode();
    public override int Id => EventId;

    public int PlayerId { get; set; }
    public int NewHealth { get; set; }
    public int OldHealth { get; set; }

    public override void Clear()
    {
        PlayerId = 0;
        NewHealth = 0;
        OldHealth = 0;
    }

    // 工厂方法（配合 ReferencePool 使用）
    public static PlayerHealthChangedEventArgs Create(int playerId, int newHealth, int oldHealth)
    {
        var eventArgs = ReferencePool.Acquire<PlayerHealthChangedEventArgs>();
        eventArgs.PlayerId = playerId;
        eventArgs.NewHealth = newHealth;
        eventArgs.OldHealth = oldHealth;
        return eventArgs;
    }
}
```

## EventModule API

```csharp
public sealed class EventModule : ModuleBase<EventModule>
{
    public IEventDispatcher EventDispatcher { get; }
    public IThreadSafeEventDispatcher ThreadSafeEventDispatcher { get; }

    // 订阅/取消订阅
    public void Subscribe(int id, GameEventHandler handler);
    public void Unsubscribe(int id, GameEventHandler handler);

    // 同步分发（线程安全）
    public void Dispatch(object sender, GameEventArgs eventArgs);

    // 线程安全分发（可从工作线程调用）
    public void ThreadSafeDispatch(object sender, GameEventArgs eventArgs);

    // 创建独立的分发器
    public IEventDispatcher CreateEventDispatcher();
    public IThreadSafeEventDispatcher CreateThreadSafeEventDispatcher();
    public IPriorityEventDispatcher CreatePriorityEventDispatcher();
}
```

## 基本用法

### 订阅事件

```csharp
// 在 OnInit 或构造函数中订阅
EventModule.Instance.Subscribe(PlayerHealthChangedEventArgs.EventId, OnHealthChanged);

private void OnHealthChanged(object sender, GameEventArgs e)
{
    var args = (PlayerHealthChangedEventArgs)e;
    Debug.Log($"Player {args.PlayerId} health: {args.OldHealth} -> {args.NewHealth}");
}
```

### 发布事件

```csharp
// 发布事件（会自动归还到池如果 IsReleaseAfterFire = true）
EventModule.Instance.Dispatch(this, PlayerHealthChangedEventArgs.Create(1, 80, 100));
```

### 取消订阅

```csharp
// 重要：在 OnDestroy 中取消订阅防止内存泄漏
EventModule.Instance.Unsubscribe(PlayerHealthChangedEventArgs.EventId, OnHealthChanged);
```

## 线程安全

### 普通分发

```csharp
// 仅在主线程调用
EventModule.Instance.Dispatch(sender, eventArgs);
```

### 线程安全分发

```csharp
// 可从任何线程调用，事件会被放入队列
EventModule.Instance.ThreadSafeDispatch(sender, eventArgs);

// 在主线程 Update 中处理队列（EventModule 已自动处理）
```

## 独立事件分发器

如果需要隔离的事件系统：

```csharp
// 创建独立分发器
var dispatcher = EventModule.Instance.CreateEventDispatcher();

// 订阅到独立分发器
dispatcher.Subscribe(MyEventArgs.EventId, handler);

// 分发事件（仅影响该分发器）
dispatcher.Dispatch(sender, eventArgs);

// 销毁
dispatcher.Dispose();
```

## 事件分发模式

```csharp
[Flags]
public enum EventDispatcherMode
{
    AllowMultiHandler = 1 << 0,       // 允许多个处理器
    AllowDuplicateHandler = 1 << 1,   // 允许重复处理器
    AllowNoHandler = 1 << 2,          // 无处理器时不抛异常
    AlwaysInvokeDefaultHandler = 1 << 3, // 总是调用默认处理器
}
```

## ReferencePool 集成

`GameEventArgs` 默认实现 `IsReleaseAfterFire = true`，分发后自动归还到池：

```csharp
// 自动归还（默认）
EventModule.Instance.Dispatch(this, PlayerHealthChangedEventArgs.Create(1, 80, 100));

// 手动管理（设置 IsReleaseAfterFire = false）
var args = PlayerHealthChangedEventArgs.Create(1, 80, 100);
args.SetIsReleaseAfterFire(false);
EventModule.Instance.Dispatch(this, args);
// 手动归还
ReferencePool.Release(args);
```

## 最佳实践

### 1. 始终取消订阅

```csharp
public class MyService
{
    private GameEventHandler _handler;

    public void Init()
    {
        _handler = OnHealthChanged;
        EventModule.Instance.Subscribe(PlayerHealthChangedEventArgs.EventId, _handler);
    }

    public void Destroy()
    {
        EventModule.Instance.Unsubscribe(PlayerHealthChangedEventArgs.EventId, _handler);
    }

    private void OnHealthChanged(object sender, GameEventArgs e) { }
}
```

### 2. 使用工厂方法创建事件

```csharp
// 推荐：使用工厂方法
var eventArgs = PlayerHealthChangedEventArgs.Create(playerId, newHealth, oldHealth);
EventModule.Instance.Dispatch(this, eventArgs);

// 不推荐：手动 new
var eventArgs = new PlayerHealthChangedEventArgs { PlayerId = playerId, ... };
```

### 3. 在 OnDestroy 中清理

```csharp
public class MyService : IDisposable
{
    protected virtual void OnDestroy()
    {
        EventModule.Instance.Unsubscribe(PlayerHealthChangedEventArgs.EventId, OnHealthChanged);
    }

    public void Dispose()
    {
        OnDestroy();
    }
}
```

## 常见问题

### Q: 事件不触发？

检查：
1. 是否正确订阅（使用正确的 EventId）
2. 是否在正确的分发器上订阅
3. 订阅时机是否在发布之前

### Q: 内存泄漏？

检查：
1. 是否所有订阅都有对应的取消订阅
2. `this` 是否被正确引用（可能需要弱引用）