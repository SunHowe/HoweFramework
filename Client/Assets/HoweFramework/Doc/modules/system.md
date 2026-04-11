# 系统管理

## 概述

SystemModule 提供游戏系统的注册和管理，与 IOC 模块配合实现依赖注入。

## 核心文件

| 文件 | 路径 |
|------|------|
| SystemModule | `Assets/HoweFramework/System/SystemModule.cs` |
| ISystem | `Assets/HoweFramework/System/ISystem.cs` |
| SystemBase | `Assets/HoweFramework/System/SystemBase.cs` |
| SystemFacade | `Assets/HoweFramework/System/SystemFacade.cs` |

## ISystem 接口

```csharp
public interface ISystem
{
    void Init();
    void Destroy();
}
```

## SystemBase

```csharp
public abstract class SystemBase : ISystem, IEventSubscribe
{
    protected abstract void OnInit();
    protected abstract void OnDestroy();

    // IEventSubscribe 实现
    void Subscribe(int id, GameEventHandler handler);
    void Unsubscribe(int id, GameEventHandler handler);
}
```

## SystemFacade

```csharp
public abstract class SystemFacade<T> where T : ISystem
{
    public static T Instance { get; }
}
```

## SystemModule API

```csharp
public sealed class SystemModule : ModuleBase<SystemModule>
{
    public event Action<ISystem> OnSystemDestroyed;

    // 获取系统
    public T GetSystem<T>() where T : ISystem;

    // 创建并注册系统（无接口）
    public T RegisterSystem<T>() where T : ISystem, new();

    // 创建并注册系统（带接口）
    public TInterface RegisterSystem<TInterface, TSystem>()
        where TInterface : ISystem
        where TSystem : TInterface, new();

    // 注册已有系统实例
    public void RegisterSystem<T>(T system) where T : ISystem;

    // 销毁系统
    public void DestroySystem<T>() where T : ISystem;
}
```

## 基本用法

### 1. 定义系统接口

```csharp
public interface ILoginSystem : ISystem
{
    int UserId { get; }
    string Username { get; }

    UniTask LoginAsync(string username, string password);
    void Logout();
}
```

### 2. 实现系统

```csharp
public sealed class OfflineLoginSystem : SystemBase, ILoginSystem
{
    public int UserId { get; private set; }
    public string Username { get; private set; }

    protected override void OnInit()
    {
        Debug.Log("OfflineLoginSystem initialized");
    }

    protected override void OnDestroy()
    {
        Debug.Log("OfflineLoginSystem destroyed");
    }

    public async UniTask LoginAsync(string username, string password)
    {
        await UniTask.Delay(500); // 模拟网络延迟

        UserId = UnityEngine.Random.Range(1000, 9999);
        Username = username;
    }

    public void Logout()
    {
        UserId = 0;
        Username = null;
    }
}
```

### 3. 注册系统

```csharp
// 在 ProcedureInitSystem 中
SystemModule.Instance.RegisterSystem<ILoginSystem, OfflineLoginSystem>();
```

### 4. 访问系统

```csharp
// 方式一：通过 SystemModule
var loginSystem = SystemModule.Instance.GetSystem<ILoginSystem>();

// 方式二：通过 Facade（如果已定义）
var loginSystem = LoginSystemFacade.Instance;
```

### 5. 创建 Facade

```csharp
public sealed class LoginSystemFacade : SystemFacade<ILoginSystem>
{
}
```

## 完整示例

### 系统模块定义

```csharp
// IGameSystem.cs
public interface IGameSystem : ISystem
{
    GameContext Context { get; set; }
}

// BattleSystem.cs
public sealed class BattleSystem : SystemBase, IGameSystem
{
    public GameContext Context { get; set; }

    private List<IGameEntity> _entities = new();

    protected override void OnInit()
    {
        // 订阅事件
        Subscribe(EntitySpawnedEventArgs.EventId, OnEntitySpawned);
    }

    protected override void OnDestroy()
    {
        Unsubscribe(EntitySpawnedEventArgs.EventId, OnEntitySpawned);
        _entities.Clear();
    }

    private void OnEntitySpawned(object sender, GameEventArgs e)
    {
        var args = (EntitySpawnedEventArgs)e;
        _entities.Add(args.Entity);
    }
}
```

### 系统管理器

```csharp
public static class GameSystems
{
    public static T Get<T>() where T : ISystem
    {
        return SystemModule.Instance.GetSystem<T>();
    }

    public static ILoginSystem LoginSystem => Get<ILoginSystem>();
    public static IGameSystem BattleSystem => Get<IGameSystem>();
}
```

### 在 Procedure 中注册

```csharp
public class ProcedureInitSystem : ProcedureBase
{
    public override int Id => (int)ProcedureId.InitSystem;

    protected override void OnEnter()
    {
        // 注册游戏系统
        SystemModule.Instance.RegisterSystem<ILoginSystem, OfflineLoginSystem>();
        SystemModule.Instance.RegisterSystem<IGameSystem, BattleSystem>();
    }

    protected override void OnLeave()
    {
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        ChangeNextProcedure();
    }
}
```

## 与 IOC 的区别

| 特性 | SystemModule | IOCModule |
|------|--------------|-----------|
| 用途 | 游戏业务系统 | 基础服务 |
| 生命周期 | Init/Destroy | 无内置 |
| 事件订阅 | 支持（通过 SystemBase） | 不支持 |
| 适用场景 | 登录系统、战斗系统等 | UI 模块、资源模块等 |

## 最佳实践

### 1. 使用 Facade 简化访问

```csharp
public sealed class LoginSystemFacade : SystemFacade<ILoginSystem>
{
}

// 使用
var loginSystem = LoginSystemFacade.Instance;
```

### 2. 在 OnDestroy 中清理

```csharp
protected override void OnDestroy()
{
    // 取消所有事件订阅
    // 释放所有资源
    // 清空所有缓存
}
```

### 3. 避免循环依赖

```csharp
// 不好：系统 A 依赖系统 B，系统 B 又依赖系统 A
public class SystemA : SystemBase, ISystemA
{
    [Inject]
    private ISystemB _systemB;
}

public class SystemB : SystemBase, ISystemB
{
    [Inject]
    private ISystemA _systemA;
}
```