# 游戏管理器

## 概述

Gameplay 框架提供多种管理器，负责特定功能的管理。

## 管理器列表

| 管理器 | 类型值 | 说明 |
|--------|--------|------|
| GameUpdateManager | 1 | 更新/帧循环管理 |
| GameRandomManager | 2 | 随机数生成 |
| GameEntityManager | 100 | 实体管理 |
| GameViewManager | 4 | 视图对象管理 |
| GameTimerManager | 5 | 计时器管理 |
| ExpressionManager | 6 | 表达式求值 |

## GameManagerType 枚举

```csharp
public enum GameManagerType
{
    Update = 1,
    Random = 2,
    Scene = 3,
    View = 4,
    Timer = 5,
    Expression = 6,
    Entity = 100,
}
```

## IGameManager

```csharp
public interface IGameManager
{
    int ManagerType { get; }
    void Initialize();
    void Update(float elapseSeconds, float realElapseSeconds);
    void Shutdown();
}
```

---

## GameUpdateManager

### 文件
| 文件 | 路径 |
|------|------|
| GameUpdateManager | `Gameplay/Framework/Update/GameUpdateManager.cs` |
| IGameUpdateManager | `Gameplay/Framework/Update/IGameUpdateManager.cs` |

### IGameUpdateManager

```csharp
public interface IGameUpdateManager : IGameManager
{
    // 游戏时间
    float GameTime { get; }
    float GameFixedTime { get; }
    int GameFrame { get; }
    int GameFrameRate { get; }
    float GameFixedDeltaTime { get; }

    // 注册更新回调
    void RegisterUpdate(object target, GameUpdateDelegate updateDelegate);
    void RegisterFixedUpdate(object target, GameUpdateDelegate updateDelegate);
    void RegisterLateUpdate(object target, GameUpdateDelegate updateDelegate);
    void RegisterLateFixedUpdate(object target, GameUpdateDelegate updateDelegate);

    // 注销
    void UnregisterUpdate(object target, GameUpdateDelegate updateDelegate);
    void UnregisterByTarget(object target);
}

public delegate void GameUpdateDelegate(float elapseSeconds, float realElapseSeconds);
```

### 用法

```csharp
var updateManager = context.GetManager<IGameUpdateManager>();

// 注册普通更新
updateManager.RegisterUpdate(this, OnUpdate);

// 注册固定更新
updateManager.RegisterFixedUpdate(this, OnFixedUpdate);

// 在 MonoBehaviour 中
private void OnUpdate(float elapse, float realElapse)
{
    // 更新逻辑
}
```

---

## GameRandomManager

### 文件
| 文件 | 路径 |
|------|------|
| GameRandomManager | `Gameplay/Framework/Random/GameRandomManager.cs` |
| IGameRandomManager | `Gameplay/Framework/Random/IGameRandomManager.cs` |

### IGameRandomManager

```csharp
public interface IGameRandomManager : IGameManager
{
    // 获取随机数
    int GetRandom(int min, int max);
    int GetRandom(int max); // 0 ~ max-1

    // 设置种子
    void SetSeed(int seed);
}
```

### 用法

```csharp
var randomManager = context.GetManager<GameRandomManager>();

// 获取随机数
int value = randomManager.GetRandom(0, 100); // 0-99
int value2 = randomManager.GetRandom(10);    // 0-9

// 设置种子（用于录像回放）
randomManager.SetSeed(12345);
```

---

## GameEntityManager

### 文件
| 文件 | 路径 |
|------|------|
| GameEntityManager | `Gameplay/Framework/Entity/GameEntityManager.cs` |

### IGameEntityManager

```csharp
public interface IGameEntityManager : IGameManager
{
    // 创建/获取/销毁实体
    IGameEntity CreateEntity();
    IGameEntity GetEntity(int entityId);
    void DestroyEntity(int entityId);

    // 生成组件 ID
    int SpawnComponentId();
}
```

### 用法

```csharp
var entityManager = context.GetManager<IGameEntityManager>();

// 创建实体
var entity = entityManager.CreateEntity();

// 添加组件
entity.AddComponent<TransformComponent>();

// 销毁实体
entityManager.DestroyEntity(entity.EntityId);
```

---

## GameTimerManager

### 用法

```csharp
var timerManager = context.GetManager<IGameTimerManager>();

// 帧定时器
int id1 = timerManager.AddFrameTimer(callback);
int id2 = timerManager.AddFrameTimer(interval: 5, callback);

// 时间定时器
int id3 = timerManager.AddTimer(interval: 1f, callback);
int id4 = timerManager.AddTimer(interval: 1f, repeatTimes: 10, callback);

// 移除
timerManager.RemoveTimer(id3);
```

---

## ExpressionManager

### 文件
| 文件 | 路径 |
|------|------|
| ExpressionManager | `Gameplay/Framework/Expression/ExpressionManager.cs` |

### 用法

```csharp
var expressionManager = context.GetManager<ExpressionManager>();

// 注册自定义变量
expressionManager.RegisterTokenParser((token, userData, out value) =>
{
    if (token == "Hp")
    {
        value = myEntity.GetComponent<NumericComponent>()["Hp"];
        return true;
    }
    return false;
});

// 求值表达式
double result = expressionManager.Evaluate("Hp * 2 + 100", userData);
```

---

## 完整示例

### 创建游戏上下文

```csharp
public class GameContextExample : GameContextBase
{
    protected override void Awake()
    {
        base.Awake();

        // 注册所有管理器
        RegisterManager(new GameUpdateManager());
        RegisterManager(new GameRandomManager());
        RegisterManager(new GameTimerManager());
        RegisterManager(new GameEntityManager());
        RegisterManager(new GameViewManager());
        RegisterManager(new ExpressionManager());

        UseManagedEventDispatcher();
        UseManagedResLoader();
    }

    public override void Update(float elapseSeconds, float realElapseSeconds)
    {
        base.Update(elapseSeconds, realElapseSeconds);

        // 更新所有管理器
        GetManager<GameUpdateManager>().Update(elapseSeconds, realElapseSeconds);
    }
}
```

### 使用

```csharp
public class ExampleUsage
{
    public void Demo()
    {
        var context = new GameContextExample();
        context.Awake();

        var entityManager = context.GetManager<IGameEntityManager>();
        var randomManager = context.GetManager<GameRandomManager>();
        var timerManager = context.GetManager<IGameTimerManager>();

        // 创建实体
        var entity = entityManager.CreateEntity();

        // 使用随机
        int roll = randomManager.GetRandom(1, 7); // 1-6

        // 设置定时器
        timerManager.AddTimer(1f, () => Debug.Log("Tick!"));
    }
}
```

---

## 最佳实践

### 1. 通过泛型方法获取

```csharp
// 好
var manager = context.GetManager<GameUpdateManager>();

// 不好
var manager = (GameUpdateManager)context.GetManager(1);
```

### 2. 在 Context 中统一更新

```csharp
public class MyContext : GameContextBase
{
    public override void Update(float elapseSeconds, float realElapseSeconds)
    {
        base.Update(elapseSeconds, realElapseSeconds);

        // 统一更新所有管理器
        GetManager<GameUpdateManager>().Update(elapseSeconds, realElapseSeconds);
    }
}
```

### 3. 正确清理

```csharp
public override void Shutdown()
{
    // 清理管理器
    GetManager<GameEntityManager>().Shutdown();
    GetManager<GameUpdateManager>().Shutdown();

    base.Shutdown();
}
```