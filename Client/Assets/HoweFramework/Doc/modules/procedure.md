# 流程系统

## 概述

ProcedureModule 管理游戏的主要流程状态机，处理从启动到登录再到游戏主循环的流程切换。

## 核心文件

| 文件 | 路径 |
|------|------|
| ProcedureModule | `Assets/HoweFramework/Procedure/ProcedureModule.cs` |
| ProcedureBase | `Assets/HoweFramework/Procedure/ProcedureBase.cs` |
| IProcedureController | `Assets/HoweFramework/Procedure/IProcedureController.cs` |

## 流程启动

```csharp
// GameEntry.Start()
var procedures = new ProcedureBase[]
{
    new ProcedureSplash(),        // Id = 1
    new ProcedureLoadDataTable(), // Id = 2
    new ProcedureLoadLocalization(), // Id = 3
    new ProcedureInitSystem(),    // Id = 4
    new ProcedureLogin(),         // Id = 5
};

ProcedureModule.Instance.Launch((int)ProcedureId.Splash, procedures);
```

## ProcedureBase

所有游戏流程继承 `ProcedureBase`：

```csharp
public abstract class ProcedureBase
{
    public abstract int Id { get; }

    // 生命周期
    public void Enter();   // 内部调用 OnEnter 和初始化 controllers
    public void Leave();    // 清理 controllers 和调用 OnLeave
    public void Update(float elapseSeconds, float realElapseSeconds);

    // Controller 管理
    protected void AddController(IProcedureController controller);
    protected void AddController<T>() where T : IProcedureController, new();

    // 流程切换
    protected void ChangeProcedure(int procedureId);
    protected void ChangeNextProcedure(); // 使用 Id + 1

    protected abstract void OnEnter();
    protected abstract void OnLeave();
    protected abstract void OnUpdate(float elapseSeconds, float realElapseSeconds);
}
```

## ProcedureModule API

```csharp
public sealed class ProcedureModule : ModuleBase<ProcedureModule>
{
    public int ProcedureId { get; }
    public ProcedureBase Procedure { get; }

    // 启动流程机
    public void Launch(int procedureId, params ProcedureBase[] procedures);

    // 停止流程机
    public void Stop();

    // 内部：切换流程（由 ProcedureBase 调用）
    internal void ChangeProcedure(int procedureId);
}
```

## 创建自定义流程

### 1. 定义 ProcedureId

```csharp
public enum ProcedureId
{
    Splash = 1,
    LoadDataTable,
    LoadLocalization,
    InitSystem,
    Login,
    MyNewProcedure, // 添加新流程
}
```

### 2. 创建 Procedure 类

```csharp
public sealed class ProcedureMyNew : ProcedureBase
{
    public override int Id => (int)ProcedureId.MyNew;

    private bool _isComplete;

    protected override void OnEnter()
    {
        _isComplete = false;
        Debug.Log("ProcedureMyNew OnEnter");

        // 启动异步操作
        DoWorkAsync().ContinueWith(() => _isComplete = true).Forget();
    }

    protected override void OnLeave()
    {
        Debug.Log("ProcedureMyNew OnLeave");
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        // 检查是否完成
        if (_isComplete)
        {
            // 切换到下一个流程
            ChangeNextProcedure();

            // 或者切换到指定流程
            // ChangeProcedure((int)ProcedureId.Login);
        }
    }

    private async UniTaskVoid DoWorkAsync()
    {
        await UniTask.Delay(1000);
        Debug.Log("Work completed");
    }
}
```

### 3. 注册新流程

```csharp
// GameEntry.Start()
var procedures = new ProcedureBase[]
{
    new ProcedureSplash(),
    new ProcedureLoadDataTable(),
    new ProcedureLoadLocalization(),
    new ProcedureInitSystem(),
    new ProcedureLogin(),
    new ProcedureMyNew(), // 添加到列表
};

ProcedureModule.Instance.Launch((int)ProcedureId.Splash, procedures);
```

## 流程控制器

流程控制器用于在流程中管理子状态：

```csharp
public interface IProcedureController
{
    void Initialize();
    void Dispose();
}

// 使用方式
public sealed class MyProcedure : ProcedureBase
{
    protected override void OnEnter()
    {
        AddController<MyController>();
        // 或
        AddController(new MyController());
    }
}
```

## 异步操作模式

流程中常使用异步操作：

```csharp
protected override void OnEnter()
{
    // 方式一：使用 UniTask
    LoadDataAsync().Forget();

    // 方式二：使用 ContinueWith
    DoSomethingAsync().ContinueWith(() => _isComplete = true).Forget();

    // 方式三：在 OnUpdate 中检查
    _operation = LoadSomethingAsync();
}

protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
{
    if (_operation != null && _operation.IsCompleted)
    {
        _isComplete = true;
    }
}
```

## 流程切换

### 切换到下一个流程

```csharp
ChangeNextProcedure(); // 使用 Id + 1
```

### 切换到指定流程

```csharp
ChangeProcedure((int)ProcedureId.Login);
```

## 已有流程说明

### ProcedureSplash

启动流程，负责：
- 初始化 YooAsset
- 初始化 FairyGUI
- 加载 UI Package

### ProcedureLoadDataTable

加载配置表数据：
- 添加 DataTableSource
- 预加载数据

### ProcedureLoadLocalization

加载本地化文本：
- 添加 LocalizationSource
- 异步加载

### ProcedureInitSystem

初始化游戏系统：
- 注册 SystemModule 系统

### ProcedureLogin

登录流程：
- 加载登录场景

## 生命周期

```
Launch(id, procedures)
    ↓
Procedure_1.OnEnter()
    ↓
Procedure_1.OnUpdate() ← 每帧调用
    ↓
ChangeNextProcedure() 或 ChangeProcedure(id)
    ↓
Procedure_1.OnLeave()
Procedure_2.OnEnter()
    ↓
... 重复 ...
    ↓
Stop()
    ↓
当前 Procedure.OnLeave()
```

## 最佳实践

### 1. 使用标志位管理状态

```csharp
public sealed class MyProcedure : ProcedureBase
{
    private enum State
    {
        Init,
        Loading,
        Complete
    }

    private State _state = State.Init;
    private bool _isComplete;

    protected override void OnEnter()
    {
        _state = State.Loading;
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        switch (_state)
        {
            case State.Loading:
                if (_isComplete) _state = State.Complete;
                break;
            case State.Complete:
                ChangeNextProcedure();
                break;
        }
    }
}
```

### 2. 在 OnLeave 中清理

```csharp
protected override void OnLeave()
{
    _operation = null;
    EventModule.Instance.Unsubscribe(MyEventArgs.EventId, OnHandler);
}
```

### 3. 异步加载资源

```csharp
private async UniTaskVoid LoadAssetsAsync()
{
    await ResModule.Instance.LoadAssetAsync<Texture2D>("texture");
    await ResModule.Instance.LoadAssetAsync<AudioClip>("sound");
    _isComplete = true;
}
```