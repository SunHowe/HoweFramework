# 计时器系统

## 概述

TimerModule 提供高效的定时器功能，支持帧定时器和时间定时器。

## 核心文件

| 文件 | 路径 |
|------|------|
| TimerModule | `Assets/HoweFramework/Timer/TimerModule.cs` |
| TimerDispatcher | `Assets/HoweFramework/Timer/TimerDispatcher.cs` |
| ITimerDispatcher | `Assets/HoweFramework/Timer/ITimerDispatcher.cs` |
| TimerInfo | `Assets/HoweFramework/Timer/TimerInfo.cs` |

## TimerCallback

```csharp
public delegate void TimerCallback();
```

## TimerModule API

```csharp
public sealed class TimerModule : ModuleBase<TimerModule>
{
    // 帧定时器（每帧触发）
    public int AddFrameTimer(TimerCallback callback);
    public int AddFrameTimer(int interval, TimerCallback callback);
    public int AddFrameTimer(int interval, int repeatTimes, TimerCallback callback);

    // 时间定时器
    public int AddTimer(float interval, TimerCallback callback);
    public int AddTimer(float interval, int repeatTimes, TimerCallback callback);

    // 移除定时器
    public void RemoveTimer(int timerId);

    // 创建独立的定时器分发器
    public ITimerDispatcher CreateTimerDispatcher();
}
```

## ITimerDispatcher

```csharp
public interface ITimerDispatcher : IReference
{
    // 帧定时器
    int AddFrameTimer(TimerCallback callback);
    int AddFrameTimer(TimerCallback callback, object userData);
    int AddFrameTimer(int interval, TimerCallback callback);
    int AddFrameTimer(int interval, TimerCallback callback, object userData);
    int AddFrameTimer(int interval, int repeatTimes, TimerCallback callback);
    int AddFrameTimer(int interval, int repeatTimes, TimerCallback callback, object userData);

    // 时间定时器
    int AddTimer(float interval, TimerCallback callback);
    int AddTimer(float interval, TimerCallback callback, object userData);
    int AddTimer(float interval, int repeatTimes, TimerCallback callback);
    int AddTimer(float interval, int repeatTimes, TimerCallback callback, object userData);

    // 移除
    void RemoveTimer(int timerId);

    // 更新（需要手动调用或连接到框架 Update）
    void Update(float elapseSeconds);
}
```

## 基本用法

### 1. 帧定时器

```csharp
// 每帧触发
int id1 = TimerModule.Instance.AddFrameTimer(() => Debug.Log("Every frame"));

// 每 60 帧触发一次
int id2 = TimerModule.Instance.AddFrameTimer(60, () => Debug.Log("Every 60 frames"));

// 每 60 帧触发，共触发 10 次
int id3 = TimerModule.Instance.AddFrameTimer(60, 10, () => Debug.Log("Tick"));

// 移除
TimerModule.Instance.RemoveTimer(id3);
```

### 2. 时间定时器

```csharp
// 每秒触发
int id1 = TimerModule.Instance.AddTimer(1f, () => Debug.Log("Every second"));

// 每秒触发，共触发 10 次
int id2 = TimerModule.Instance.AddTimer(1f, 10, () => Debug.Log("Tick"));

// 移除
TimerModule.Instance.RemoveTimer(id2);
```

### 3. 带 UserData 的定时器

```csharp
public void StartTimer()
{
    int id = TimerModule.Instance.AddTimer(1f, OnTimerTick, "myData");
}

private void OnTimerTick(object userData)
{
    Debug.Log($"Timer tick: {userData}");
}
```

## TimerInfo（内部实现）

```csharp
internal sealed class TimerInfo : IReference
{
    public int TimerId { get; set; }
    public TimerCallback TimerCallback { get; set; }
    public object UserData { get; set; }
    public float TimerInterval { get; set; }
    public int TimerUpdateInterval { get; set; }
    public int TimerRepeatTimes { get; set; }
    public float PreviousInvokeTime { get; set; }
    public float NextInvokeTime { get; set; }
    public int NextInvokeUpdateTimes { get; set; }
    public int InvokeTimes { get; set; }
    public bool IsCancel { get; set; }

    public bool IsArrived(float elapsedTime, int updateTimes);
    public void Clear();
}
```

## 独立定时器分发器

```csharp
// 创建独立分发器
var dispatcher = TimerModule.Instance.CreateTimerDispatcher();

// 添加定时器
int id = dispatcher.AddTimer(1f, () => Debug.Log("Tick"));

// 手动更新（如果未连接到框架 Update）
dispatcher.Update(Time.deltaTime);

// 销毁
dispatcher.Dispose();
```

## 典型使用场景

### 1. 延迟执行

```csharp
// 延迟 2 秒后执行
TimerModule.Instance.AddTimer(2f, 1, () =>
{
    Debug.Log("Delayed action");
});
```

### 2. 循环执行带计数

```csharp
private int _tickCount = 0;
private int _timerId;

private void StartLoop()
{
    _tickCount = 0;
    _timerId = TimerModule.Instance.AddTimer(1f, OnTick);
}

private void OnTick()
{
    _tickCount++;
    if (_tickCount >= 10)
    {
        TimerModule.Instance.RemoveTimer(_timerId);
        Debug.Log("Loop complete");
    }
}
```

### 3. 帧同步循环

```csharp
private int _frameId;

private void StartFrameLoop()
{
    _frameId = TimerModule.Instance.AddFrameTimer(60, OnFrameTick); // 每秒约60帧
}

private void OnFrameTick()
{
    // 帧逻辑
}
```

### 4. 冷却时间

```csharp
private Dictionary<int, int> _cooldowns = new();

private bool CanUseSkill(int skillId)
{
    if (_cooldowns.TryGetValue(skillId, out int timerId))
    {
        return false; // 还在冷却
    }

    // 启动 5 秒冷却
    _cooldowns[skillId] = TimerModule.Instance.AddTimer(5f, 1, () =>
    {
        _cooldowns.Remove(skillId);
    });

    return true;
}
```

### 5. Unity Update 中手动更新

```csharp
public class MyComponent : MonoBehaviour
{
    private ITimerDispatcher _dispatcher;

    private void Awake()
    {
        _dispatcher = TimerModule.Instance.CreateTimerDispatcher();
    }

    private void Update()
    {
        _dispatcher.Update(Time.deltaTime);
    }

    private void OnDestroy()
    {
        _dispatcher.Dispose();
    }
}
```

## 最佳实践

### 1. 记得移除定时器

```csharp
private int _timerId;

public void Start()
{
    _timerId = TimerModule.Instance.AddTimer(1f, OnTick);
}

public void OnDestroy()
{
    if (_timerId != 0)
    {
        TimerModule.Instance.RemoveTimer(_timerId);
        _timerId = 0;
    }
}
```

### 2. 使用成员变量存储 TimerId

```csharp
public class MyClass
{
    private int _frameTimerId;
    private int _timeTimerId;

    public void Init()
    {
        _frameTimerId = TimerModule.Instance.AddFrameTimer(60, OnFrameTick);
        _timeTimerId = TimerModule.Instance.AddTimer(1f, OnTimeTick);
    }

    public void Cleanup()
    {
        if (_frameTimerId != 0)
        {
            TimerModule.Instance.RemoveTimer(_frameTimerId);
            _frameTimerId = 0;
        }
        if (_timeTimerId != 0)
        {
            TimerModule.Instance.RemoveTimer(_timeTimerId);
            _timeTimerId = 0;
        }
    }
}
```

### 3. repeatTimes = 1 表示只触发一次

```csharp
// 触发一次（相当于 Delay）
TimerModule.Instance.AddTimer(2f, 1, () => Debug.Log("Once"));

// 无限循环
TimerModule.Instance.AddTimer(1f, -1, () => Debug.Log("Forever"));
```