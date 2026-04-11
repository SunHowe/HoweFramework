# 游戏上下文

## 概述

GameContext 是游戏逻辑的根容器，管理所有游戏管理器，提供统一的访问入口。

## 核心文件

| 文件 | 路径 |
|------|------|
| IGameContext | `Gameplay/Framework/IGameContext.cs` |
| GameContextBase | `Gameplay/Framework/GameContextBase.cs` |

## IGameContext

```csharp
public interface IGameContext
{
    // 事件分发器
    IEventDispatcher EventDispatcher { get; }

    // 对象池
    IGameObjectPool GameObjectPool { get; }

    // 资源加载器
    IResLoader ResLoader { get; }

    // 游戏状态
    GameStatus GameStatus { get; }

    // 生命周期
    void Awake();
    void StartGame();
    void PauseGame();
    void ResumeGame();
    void StopGame();

    // 获取管理器
    IGameManager GetManager(int managerType);
}
```

## GameStatus

```csharp
public enum GameStatus
{
    None,        // 初始状态
    Initialize,  // 已初始化
    Running,     // 运行中
    Pause,       // 暂停
    Stopped      // 已停止
}
```

## GameContextBase

```csharp
public abstract class GameContextBase : IGameContext
{
    // 事件分发器（可自定义）
    public IEventDispatcher EventDispatcher { get; set; }

    // 游戏对象池（可自定义）
    public IGameObjectPool GameObjectPool { get; set; }

    // 资源加载器（可自定义）
    public IResLoader ResLoader { get; set; }

    // 当前状态
    public GameStatus GameStatus { get; private set; }

    // 获取管理器
    public T GetManager<T>() where T : IGameManager;

    // Managed 模式（自动创建内部资源）
    public void UseManagedEventDispatcher();
    public void UseManagedResLoader();
    public void UseManagedGameObjectPool();

    // 生命周期
    public abstract void Awake();
    public void StartGame();
    public void PauseGame();
    public void ResumeGame();
    public void StopGame();
}
```

## 基本用法

### 1. 创建游戏上下文

```csharp
public class MyGameContext : GameContextBase
{
    protected override void Awake()
    {
        base.Awake();

        // 注册管理器
        RegisterManager(new GameEntityManager());
        RegisterManager(new GameUpdateManager());
        RegisterManager(new GameTimerManager());
        RegisterManager(new GameRandomManager());

        // 使用托管资源
        UseManagedEventDispatcher();
        UseManagedResLoader();
        UseManagedGameObjectPool();
    }
}
```

### 2. 启动游戏

```csharp
var context = new MyGameContext();
context.Awake();          // 初始化
context.StartGame();      // 开始
```

### 3. 获取管理器

```csharp
var entityManager = context.GetManager<GameEntityManager>();
var updateManager = context.GetManager<GameUpdateManager>();
```

### 4. 游戏状态控制

```csharp
context.PauseGame();   // 暂停
context.ResumeGame();  // 继续
context.StopGame();    // 停止
```

## GameManagerHelper

```csharp
// 扩展方法
public static T GetManager<T>(this IGameContext context) where T : IGameManager;

// 使用
var entityManager = context.GetManager<IGameEntityManager>();
var timerManager = context.GetManager<IGameTimerManager>();
```

## 生命周期

```
None → Initialize → Running ←→ Pause → Stopped
```

| 方法 | 状态转换 |
|------|----------|
| Awake() | None → Initialize |
| StartGame() | Initialize → Running |
| PauseGame() | Running → Pause |
| ResumeGame() | Pause → Running |
| StopGame() | Any → Stopped |

## 托管资源

GameContextBase 提供托管模式的资源管理：

### UseManagedEventDispatcher

```csharp
public void UseManagedEventDispatcher()
{
    EventDispatcher = EventModule.Instance.EventDispatcher;
}
```

### UseManagedResLoader

```csharp
public void UseManagedResLoader()
{
    ResLoader = ResModule.Instance.CreateResLoader();
}
```

### UseManagedGameObjectPool

```csharp
public void UseManagedGameObjectPool()
{
    GameObjectPool = GameObjectPoolModule.Instance.CreateGameObjectPool();
}
```

## 完整示例

### 创建游戏上下文

```csharp
public class BattleContext : GameContextBase
{
    protected override void Awake()
    {
        base.Awake();

        // 注册管理器
        RegisterManager(new GameEntityManager());
        RegisterManager(new GameUpdateManager());
        RegisterManager(new GameTimerManager());
        RegisterManager(new GameRandomManager());
        RegisterManager(new GameViewManager());
        RegisterManager(new ExpressionManager());

        // 使用托管资源
        UseManagedEventDispatcher();
        UseManagedResLoader();
        UseManagedGameObjectPool();
    }

    public IGameEntity CreateEntity(string prefab)
    {
        var entityManager = GetManager<GameEntityManager>();
        var entity = entityManager.CreateEntity();

        // 添加视图
        var view = entity.AddComponent<ViewComponent>();
        view.ResKey = prefab;

        return entity;
    }

    public void DestroyEntity(IGameEntity entity)
    {
        var entityManager = GetManager<GameEntityManager>();
        entityManager.DestroyEntity(entity.EntityId);
    }
}
```

### 使用上下文

```csharp
public class BattleSystem
{
    private BattleContext _context;

    public void Initialize()
    {
        _context = new BattleContext();
        _context.Awake();
        _context.StartGame();
    }

    public void SpawnEnemy()
    {
        var entity = _context.CreateEntity("Prefabs/Enemy");
    }

    public void Update(float deltaTime)
    {
        var updateManager = _context.GetManager<GameUpdateManager>();
        updateManager.Update(deltaTime);
    }

    public void Shutdown()
    {
        _context.StopGame();
    }
}
```

## 与框架模块的交互

GameContext 使用框架模块作为资源：

```csharp
protected override void Awake()
{
    base.Awake();

    // 使用框架的 EventModule
    EventDispatcher = EventModule.Instance.EventDispatcher;

    // 使用框架的 ResModule
    ResLoader = ResModule.Instance.CreateResLoader();

    // 使用框架的 GameObjectPoolModule
    GameObjectPool = GameObjectPoolModule.Instance.CreateGameObjectPool();

    // 注册管理器
    RegisterManager(new GameEntityManager());
}
```

## 最佳实践

### 1. 在 Awake 中初始化所有资源

```csharp
protected override void Awake()
{
    base.Awake();

    // 顺序很重要
    UseManagedEventDispatcher();          // 1. 事件分发器
    UseManagedResLoader();               // 2. 资源加载器
    UseManagedGameObjectPool();           // 3. 对象池

    RegisterManager(new GameEntityManager()); // 4. 管理器
}
```

### 2. 在 StopGame 中释放资源

```csharp
public override void StopGame()
{
    base.StopGame();

    // 释放托管资源
    ResLoader?.Dispose();
    GameObjectPool?.Dispose();
}
```

### 3. 使用泛型方法访问管理器

```csharp
// 好
var entityManager = context.GetManager<GameEntityManager>();

// 可用于接口
var manager = context.GetManager<IGameEntityManager>();
```

### 4. 避免直接引用

```csharp
// 好：通过接口访问
var manager = context.GetManager<IGameEntityManager>();

// 不好：直接依赖具体实现
private BattleContext _context; // 紧耦合
```