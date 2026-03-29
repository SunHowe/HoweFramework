# Gameplay Framework

游戏玩法框架，提供实体-组件 (Entity-Component) 架构的游戏对象管理。

## 目录

- [架构概览](#架构概览)
- [核心概念](#核心概念)
- [GameContext](#gamecontext)
- [GameManager](#gamemanager)
- [GameEntity](#gameentity)
- [GameComponent](#gamecomponent)
- [GameUpdateManager](#gameupdatemanager)
- [GameTimerManager](#gametimermanager)
- [GameRandomManager](#gamerandommanager)
- [GameSceneManager](#gamescenemanager)
- [ExpressionManager](#expressionmanager)
- [View 系统](#view-系统)
- [MonoConverter](#monoconverter)
- [数值系统](#数值系统)
- [StateComponent](#statecomponent)
- [ResourceComponent](#resourcecomponent)
- [代码规范](#代码规范)

---

## 架构概览

```
GameContext (游戏上下文)
    ├── GameEntityManager (实体管理器)
    │   └── GameEntity (游戏实体)
    │       └── GameComponent (游戏组件)
    │           ├── TransformComponent
    │           ├── ViewComponent
    │           ├── NumericComponent
    │           └── StateComponent
    ├── GameViewManager (视图管理器)
    │   └── ViewObject (视图对象)
    ├── GameUpdateManager (更新管理器)
    ├── GameSceneManager (场景管理器)
    └── ...
```

### 目录结构

```
GameMain/Scripts/Gameplay/
├── Framework/          # 框架核心
│   ├── Entity/         # 实体系统
│   ├── View/          # 视图系统
│   ├── Update/        # 更新系统
│   ├── Scene/         # 场景系统
│   ├── Timer/         # 计时器系统
│   ├── Random/        # 随机数系统
│   ├── Expression/    # 表达式系统
│   ├── Event/         # 事件
│   └── Helper/        # 辅助工具
├── Common/            # 通用组件
│   ├── Transform/     # Transform组件
│   ├── View/         # 视图组件
│   ├── Numeric/       # 数值组件
│   └── State/        # 状态组件
├── Mono/             # Unity组件转换器
└── Doc/              # 文档
```

---

## 核心概念

### GameComponentType (组件类型枚举)

定义在 `GameComponentType.cs`:

```csharp
public enum GameComponentType
{
    Transform = 1,         // Transform组件
    View = 2,             // 视图组件
    ViewTransformSync = 3, // 视图Transform同步组件
    Numeric = 4,           // 数值组件
    State = 5,             // 状态组件
    Resource = 6,          // 资源组件
}
```

### GameManagerType (管理器类型枚举)

定义在 `GameManagerType.cs`:

```csharp
public enum GameManagerType
{
    Update = 1,
    Random = 2,
    Scene = 3,
    View = 4,
    Timer = 5,
    Expression = 6,
    Entity = 100,  // 实体管理器
}
```

---

## GameContext

`GameContext` 是游戏玩法的根容器，管理所有游戏管理器和实体。

### 接口定义

```csharp
public interface IGameContext : IDisposable
{
    IEventDispatcher EventDispatcher { get; set; }
    IGameObjectPool GameObjectPool { get; set; }
    IResLoader ResLoader { get; set; }
    GameStatus GameStatus { get; }

    void Awake();
    void StartGame();
    void PauseGame();
    void ResumeGame();
    void StopGame();
    IGameManager GetManager(int managerType);
}
```

### 生命周期

```
None → Initialize → Running ↔ Pause → Stopped
```

### 创建上下文

```csharp
public class MyGameContext : GameContextBase
{
    protected override void OnAwake()
    {
        AddManager(new GameEntityManager());
        AddManager(new GameViewManager());
        AddManager(new GameUpdateManager());
        // 添加更多管理器...
    }

    protected override void OnAfterAwake()
    {
    }

    protected override void OnDispose()
    {
    }
}
```

### 使用上下文

```csharp
// 创建
var context = new MyGameContext();
context.EventDispatcher = eventDispatcher;
context.ResLoader = resLoader;
context.Awake();

// 启动游戏
context.StartGame();

// 暂停/恢复
context.PauseGame();
context.ResumeGame();

// 停止
context.StopGame();

// 销毁
context.Dispose();
```

**核心文件**: `Framework/GameContextBase.cs`, `Framework/IGameContext.cs`

---

## GameManager

管理器基类，提供管理器生命周期管理。

### 接口定义

```csharp
public interface IGameManager : IDisposable
{
    int ManagerType { get; }
    IGameContext Context { get; }
    void Awake(IGameContext context);
}
```

### 基类实现

```csharp
public abstract class GameManagerBase : IGameManager
{
    public int ManagerType => GameManagerHelper.GetManagerType(GetType());
    public IGameContext Context { get; private set; }

    public void Awake(IGameContext context)
    {
        Context = context;
        OnAwake();
    }

    public void Dispose()
    {
        OnDispose();
        Context = null;
    }

    protected abstract void OnAwake();
    protected abstract void OnDispose();
}
```

### 创建自定义管理器

```csharp
[GameManager]
public class MyCustomManager : GameManagerBase
{
    // 自动生成 ManagerType

    protected override void OnAwake()
    {
        // 初始化
    }

    protected override void OnDispose()
    {
        // 清理
    }
}
```

### 注册管理器

在 GameContext 中注册管理器：

```csharp
// 方式1：泛型注册（自动实例化）
AddManager<GameUpdateManager>();

// 方式2：实例注册
AddManager(new GameEntityManager());
AddManager(new GameViewManager());
```

**核心文件**: `Framework/GameManagerBase.cs`, `Framework/IGameManager.cs`, `Framework/GameManagerAttribute.cs`

---

## GameEntity

游戏实体，组件的容器。

### 接口定义

```csharp
public interface IGameEntity : IDisposable
{
    int EntityId { get; }
    IGameContext Context { get; }

    void Awake(int entityId, IGameEntityManager entityManager);
    void AddComponent(IGameComponent component);
    IGameComponent GetComponent(int componentType);
    void RemoveComponent(int componentType);
    void GetComponents(List<IGameComponent> components);
}
```

### 创建实体

```csharp
// 通过实体管理器
var entity = context.GetManager<GameEntityManager>().CreateEntity();

// 添加组件
entity.AddComponent<TransformComponent>();
entity.AddComponent<NumericComponent>();
entity.AddComponent<ViewComponent>();

// 获取组件
var transform = entity.GetComponent<TransformComponent>();
var numeric = entity.GetComponent<NumericComponent>();

// 销毁实体
context.GetManager<GameEntityManager>().DestroyEntity(entity.EntityId);
```

### 实体生命周期

1. **CreateEntity()** - 创建实体
2. **Awake()** - 初始化
3. **AddComponent()** - 添加组件
4. **组件 Update()** - 组件更新
5. **DestroyEntity()** - 销毁实体

**核心文件**: `Framework/Entity/IGameEntity.cs`, `Framework/Entity/GameEntityManager.cs`

---

## GameComponent

游戏组件，封装具体功能数据。

### 基类定义

```csharp
public abstract class GameComponentBase : IGameComponent, IReference
{
    public int ComponentType { get; }
    public int ComponentId { get; private set; }
    public IGameEntity Entity { get; private set; }
    public IGameContext Context { get; private set; }

    public void Awake(int componentId, IGameEntity entity);
    public void Dispose();
    public void Clear();  // IReference 实现

    protected abstract void OnAwake();
    protected abstract void OnDispose();
}
```

### 定义组件

```csharp
[GameComponent(GameComponentType.Transform)]
public class TransformComponent : GameComponentBase
{
    public Vector3 Position { get; set; }
    public Vector3 EulerAngles { get; set; }
    public Vector3 Scale { get; set; }

    protected override void OnAwake()
    {
        // 初始化
    }

    protected override void OnDispose()
    {
        // 清理
    }
}
```

### 内置组件

| 组件 | 类型值 | 说明 |
|------|--------|------|
| `TransformComponent` | 1 | 位置、旋转、缩放 |
| `ViewComponent` | 2 | 视图对象绑定 |
| `ViewTransformSyncComponent` | 3 | 视图Transform同步 |
| `NumericComponent` | 4 | 数值属性 |
| `StateComponent` | 5 | 状态机 |
| `ResourceComponent` | 6 | 资源管理 |

### 组件扩展方法

```csharp
// 添加组件
entity.AddComponent<TransformComponent>();

// 获取组件
var transform = entity.GetComponent<TransformComponent>();

// 移除组件
entity.RemoveComponent<TransformComponent>();

// 获取所有组件
var components = entity.GetComponents();
```

**核心文件**: `Framework/Entity/GameComponentBase.cs`, `Framework/Entity/GameComponentAttribute.cs`

---

## GameUpdateManager

游戏更新管理器，管理 Update、FixedUpdate、LateUpdate 的注册和调度。

### API

```csharp
// 游戏时间
float GameTime { get; }
float GameFixedTime { get; }
int GameFrame { get; }
float GameFixedDeltaTime { get; }
int GameFrameRate { get; }

// 注册更新回调（每帧）
void RegisterUpdate(object target, GameUpdateDelegate updateDelegate);
void UnregisterUpdate(object target, GameUpdateDelegate updateDelegate);

// 注册固定更新回调（固定帧率）
void RegisterFixedUpdate(object target, GameUpdateDelegate updateDelegate);
void UnregisterFixedUpdate(object target, GameUpdateDelegate updateDelegate);

// 注册延迟更新回调（每帧，在 Update 之后）
void RegisterLateUpdate(object target, GameUpdateDelegate updateDelegate);
void UnregisterLateUpdate(object target, GameUpdateDelegate updateDelegate);

// 注册延迟固定更新回调
void RegisterLateFixedUpdate(object target, GameUpdateDelegate updateDelegate);
void UnregisterLateFixedUpdate(object target, GameUpdateDelegate updateDelegate);

// 取消目标的所有更新回调
void UnregisterByTarget(object target);
```

### 使用示例

```csharp
public class MySystem
{
    private MySystem() { }

    public static MySystem Create()
    {
        var system = new MySystem();
        var updateManager = Context.GetManager<GameUpdateManager>();
        updateManager.RegisterUpdate(system, system.OnUpdate);
        return system;
    }

    public void OnUpdate(float elapseSeconds)
    {
        // 每帧更新逻辑
    }

    public void Dispose()
    {
        var updateManager = Context.GetManager<GameUpdateManager>();
        updateManager.UnregisterByTarget(this);
    }
}
```

**核心文件**: `Framework/Update/GameUpdateManager.cs`

---

## GameTimerManager

游戏定时器管理器，基于时间的定时器调度。

### API

```csharp
// 添加帧定时器（每帧回调）
int AddFrameTimer(TimerCallback callback);

// 添加帧定时器（间隔帧数）
int AddFrameTimer(int interval, TimerCallback callback);

// 添加帧定时器（间隔帧数 + 重复次数）
int AddFrameTimer(int interval, int repeatTimes, TimerCallback callback);

// 添加定时器（按时间间隔）
int AddTimer(float interval, TimerCallback callback);

// 添加定时器（时间间隔 + 重复次数）
int AddTimer(float interval, int repeatTimes, TimerCallback callback);

// 移除定时器
void RemoveTimer(int timerId);
```

### 使用示例

```csharp
// 每1秒执行
int timerId = Context.GetManager<GameTimerManager>().AddTimer(1f, () =>
{
    Debug.Log("每秒执行");
});

// 执行5次，每2秒一次
int repeatTimerId = Context.GetManager<GameTimerManager>().AddTimer(2f, 5, () =>
{
    Debug.Log("重复执行");
});

// 手动停止
Context.GetManager<GameTimerManager>().RemoveTimer(timerId);
```

**核心文件**: `Framework/Timer/GameTimerManager.cs`

---

## GameRandomManager

游戏随机数管理器。

### API

```csharp
// 获取随机整数 [min, max)
int GetRandom(int min, int max);

// 获取随机整数 [0, max)
int GetRandom(int max);

// 设置随机数种子
void SetSeed(int seed);
```

### 使用示例

```csharp
var randomManager = Context.GetManager<GameRandomManager>();

// 设置种子（用于复现）
randomManager.SetSeed(12345);

// 获取随机数
int value = randomManager.GetRandom(0, 100);  // 0-99
int value2 = randomManager.GetRandom(10);    // 0-9
```

**核心文件**: `Framework/Random/GameRandomManager.cs`

---

## GameSceneManager

游戏场景管理器，管理场景中的游戏对象。

### API

```csharp
// 场景根节点
Transform SceneRoot { get; }

// 视图根节点
Transform ViewRoot { get; }

// 获取场景对象
Object GetSceneObject(string objectName);
T GetSceneObject<T>(string objectName) where T : Object;
```

### 使用示例

```csharp
var sceneManager = Context.GetManager<GameSceneManager>();

// 获取场景中的对象
var player = sceneManager.GetSceneObject<Transform>("Player");
var data = sceneManager.GetSceneObject<MyData>("GameData");
```

**核心文件**: `Framework/Scene/GameSceneManager.cs`

---

## ExpressionManager

表达式管理器，用于解析和计算数学表达式。

### API

```csharp
// 注册标记解析器
void RegisterTokenParser(TokenParser parser);

// 计算表达式
double Evaluate(string expression, object userData);
```

### 使用示例

```csharp
var expressionManager = Context.GetManager<ExpressionManager>();

// 注册自定义函数（需要实现 TokenParser）
expressionManager.RegisterTokenParser(new MyFunctionParser());

// 计算表达式
double result = expressionManager.Evaluate("a + b * c", new { a = 1, b = 2, c = 3 });
// 结果: 7
```

**核心文件**: `Framework/Expression/ExpressionManager.cs`

---

## View 系统

视图系统负责管理 GameObject 与实体的绑定。

### GameViewManager

视图管理器，管理所有视图对象的加载和卸载。

```csharp
// 获取视图对象
var viewObject = context.GetManager<GameViewManager>().GetViewObject(entityId);

// 卸载视图
context.GetManager<GameViewManager>().UnloadView(entityId);
```

**核心文件**: `Framework/View/GameViewManager.cs`

### ViewObject

视图对象，管理 GameObject 实例：

```csharp
public interface IViewObject : IDisposable
{
    string ResKey { get; }
    GameObject GameObject { get; }
    Transform Transform { get; }
    Transform ParentTransform { get; set; }
    Vector3 Position { get; set; }
    Vector3 EulerAngles { get; set; }
    Vector3 Scale { get; set; }
    bool IsVisible { get; set; }
    bool IsLoaded { get; }

    event ViewObjectLoadedDelegate OnLoaded;
    event ViewObjectUnloadedDelegate OnUnloaded;

    void LoadGameObject(string resKey);
    void SetGameObject(GameObject gameObject);
    void UnloadGameObject();
}
```

### ViewComponent

视图组件，绑定 ViewObject 到实体：

```csharp
[GameComponent(GameComponentType.View)]
public class ViewComponent : GameComponentBase
{
    public IViewObject ViewObject { get; }

    public void LoadView(string resKey);
    public void SetView(GameObject gameObject);
    public void UnloadView();
}
```

### ViewTransformSyncComponent

自动同步 TransformComponent 到 ViewObject：

```csharp
[GameComponent(GameComponentType.ViewTransformSync)]
public class ViewTransformSyncComponent : GameComponentBase
{
    protected override void OnAwake();
    protected override void OnDispose();
}
```

### 使用流程

```csharp
// 1. 创建实体并添加组件
var entity = entityManager.CreateEntity();
entity.AddComponent<TransformComponent>();
var viewComponent = entity.AddComponent<ViewComponent>();
entity.AddComponent<ViewTransformSyncComponent>();

// 2. 加载视图
viewComponent.LoadView("Prefabs/Player");

// 3. TransformComponent 的变化会自动同步到 ViewObject
var transform = entity.GetComponent<TransformComponent>();
transform.Position = new Vector3(1, 2, 3);
```

**核心文件**: `Framework/View/GameViewManager.cs`, `Framework/View/IViewObject.cs`, `Common/View/ViewComponent.cs`

---

## MonoConverter

使用 Unity MonoBehaviour 快速转换实体。

### IGameComponentConverter

```csharp
public interface IGameComponentConverter
{
    int SortingOrder { get; }
    void Convert(IGameEntity entity);
}
```

### 使用示例

```csharp
public class PlayerConverter : MonoBehaviour
{
    [SerializeField]
    private int sortingOrder = 0;

    public int SortingOrder => sortingOrder;

    public void Convert(IGameEntity entity)
    {
        // 手动添加组件
        entity.AddComponent<TransformComponent>();

        var numeric = entity.AddComponent<NumericComponent>();
        numeric[(int)MyNumericType.Health] = 100;
    }
}

public class Player : MonoBehaviour
{
    public void Spawn(IGameEntity entity)
    {
        var converter = GetComponent<PlayerConverter>();
        converter.Convert(entity);
    }
}
```

**核心文件**: `Mono/GameEntityConverter.cs`, `Mono/IGameComponentConverter.cs`

---

## 数值系统

NumericComponent 提供完整的数值计算和事件系统。

### 数值子类型

```csharp
public enum NumericSubType
{
    Final = 0,          // 最终值 (只读)
    Basic = 1,          // 基础值
    BasicPercent = 2,   // 基础百分比
    BasicConstAdd = 3,  // 基础固定加成
    FinalPercent = 4,   // 最终百分比
    FinalConstAdd = 5,  // 最终固定加成
}
```

### 计算公式

```
Final = (Basic * (100 + BasicPercent) / 100 + BasicConstAdd) * (100 + FinalPercent) / 100 + FinalConstAdd
```

### 使用示例

```csharp
var numeric = entity.AddComponent<NumericComponent>();

// 设置基础值
numeric.Set(healthId, NumericSubType.Basic, 1000);

// 设置百分比加成
numeric.Set(healthId, NumericSubType.BasicPercent, 50);  // +50%

// 设置固定加成
numeric.Set(healthId, NumericSubType.BasicConstAdd, 100); // +100

// 获取最终值 (自动计算)
var finalHealth = numeric[healthId];  // (1000 * 150/100 + 100) = 1600

// 订阅数值变化
numeric.Subscribe(healthId, NumericSubType.Final, OnHealthChanged, notifyImmediately: true);

private void OnHealthChanged(long newValue)
{
    Debug.Log($"Health changed: {newValue}");
}
```

### 数值快照

```csharp
// 保存快照
var snapshot = numeric.TakeSnapshot();

// 恢复快照
numeric.RestoreSnapshot(snapshot);
```

**核心文件**: `Common/Numeric/NumericComponent.cs`, `Common/Numeric/INumeric.cs`

---

## StateComponent

状态组件，提供状态管理和状态变化事件。

### API

```csharp
// 检查状态是否存在
bool CheckState(int state);

// 添加状态
void AddState(int state, object provider);

// 移除状态（指定提供者）
void RemoveState(int state, object provider);

// 移除状态（所有提供者）
void RemoveState(int state);

// 订阅状态变化事件
void Subscribe(int state, SimpleEventHandler<bool> handler, bool notifyImmediately = false);

// 取消订阅
void Unsubscribe(int state, SimpleEventHandler<bool> handler);
```

### 使用示例

```csharp
var stateComponent = entity.AddComponent<StateComponent>();

// 添加状态
stateComponent.AddState(1, this);  // 状态1，提供者为此实体

// 检查状态
if (stateComponent.CheckState(1))
{
    Debug.Log("状态1存在");
}

// 订阅状态变化
stateComponent.Subscribe(1, (exists) =>
{
    Debug.Log($"状态1变为: {exists}");
}, notifyImmediately: true);

// 移除状态
stateComponent.RemoveState(1, this);
```

**核心文件**: `Common/State/StateComponent.cs`

---

## ResourceComponent

资源组件，用于管理血量、魔法值等资源数值，支持最大值绑定和事件通知。

### API

```csharp
// 获取/设置资源值
long this[int id] { get; set; }

// 获取资源值
long Get(int id);

// 设置资源值
void Set(int id, long value);

// 修改资源值（增量）
void Modify(int id, long value);

// 消耗资源（不足返回false）
bool Cost(int id, long value);
bool Cost(Dictionary<int, long> resourceDict);

// 恢复到最大值
void RecoverToMax(int id);

// 获取/设置最大值（-1表示无上限）
long GetMax(int id);
void SetMax(int id, long value);
void ModifyMax(int id, long value);

// 绑定最大值到属性
void BindNumericMax(int id, int numericId, NumericComponent numericComponent);
void UnbindNumericMax(int id);

// 订阅资源值变化
void Subscribe(int id, SimpleEventHandler<long> handler, bool notifyImmediately = false);
void Unsubscribe(int id, SimpleEventHandler<long> handler);

// 订阅最大值变化
void SubscribeMax(int id, SimpleEventHandler<long> handler, bool notifyImmediately = false);
void UnsubscribeMax(int id, SimpleEventHandler<long> handler);
```

### 使用示例

```csharp
var resourceComponent = entity.AddComponent<ResourceComponent>();

// 设置资源值
resourceComponent[(int)ResourceType.Hp] = 1000;
resourceComponent.SetMax((int)ResourceType.Hp, 1000);

// 设置无上限资源
resourceComponent.Set((int)ResourceType.Gold, 99999);
resourceComponent.SetMax((int)ResourceType.Gold, -1);

// 绑定最大值到属性（最大值随属性变化自动更新）
var numericComponent = entity.GetComponent<NumericComponent>();
resourceComponent.BindNumericMax((int)ResourceType.Hp, (int)MyNumericType.MaxHp, numericComponent);

// 消耗资源
if (resourceComponent.Cost((int)ResourceType.Hp, 50))
{
    Debug.Log("消耗成功");
}

// 订阅变化事件
resourceComponent.Subscribe((int)ResourceType.Hp, (newValue) =>
{
    Debug.Log($"HP变化: {newValue}");
}, notifyImmediately: true);
```

### 资源类型定义示例

```csharp
public enum ResourceType
{
    Hp = 1,
    Mp = 2,
    Gold = 3,
}
```

**核心文件**: `Common/Resource/ResourceComponent.cs`

---

## 代码规范

### 组件定义

```csharp
[GameComponent(GameComponentType.Custom)]
public class MyComponent : GameComponentBase
{
    protected override void OnAwake()
    {
        // 初始化
    }

    protected override void OnDispose()
    {
        // 清理资源
    }
}
```

### 管理器定义

```csharp
[GameManager]
public class MyManager : GameManagerBase
{
    protected override void OnAwake()
    {
        // 初始化
    }

    protected override void OnDispose()
    {
        // 清理资源
    }
}
```

### 实体扩展方法

```csharp
// 扩展方法定义在 GameEntityHelper
public static T AddComponent<T>(this IGameEntity entity) where T : GameComponentBase, new();
public static T GetComponent<T>(this IGameEntity entity) where T : GameComponentBase;
public static void RemoveComponent<T>(this IGameEntity entity) where T : GameComponentBase;
public static IGameComponent[] GetComponents(this IGameEntity entity);

// 使用
entity.AddComponent<TransformComponent>();
var transform = entity.GetComponent<TransformComponent>();
```

### 类型获取

```csharp
// 组件类型
int componentType = GameEntityHelper.GetComponentType<MyComponent>();

// 管理器类型
int managerType = GameManagerHelper.GetManagerType<MyManager>();

// 获取管理器
var entityManager = context.GetManager<GameEntityManager>();
```

### 射线检测

```csharp
// 射线检测实体
var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
var entity = GameEntityHelper.RaycastGameEntity(ray);

// 批量检测
var entities = new List<IGameEntity>();
GameEntityHelper.RaycastGameEntities(ray, entities);
```

---

## 快速开始

### 1. 创建 GameContext

```csharp
public class BattleContext : GameContextBase
{
    protected override void OnAwake()
    {
        AddManager<GameEntityManager>();
        AddManager<GameViewManager>();
        AddManager<GameUpdateManager>();
    }

    protected override void OnAfterAwake() { }
    protected override void OnDispose() { }
}
```

### 2. 创建实体

```csharp
var entity = battleContext.GetManager<GameEntityManager>().CreateEntity();
entity.AddComponent<TransformComponent>();
entity.AddComponent<NumericComponent>();
entity.AddComponent<ViewComponent>();
entity.AddComponent<ViewTransformSyncComponent>();
```

### 3. 加载视图

```csharp
var view = entity.GetComponent<ViewComponent>();
view.LoadView("Prefabs/Player");
```

### 4. 更新 Transform

```csharp
var transform = entity.GetComponent<TransformComponent>();
transform.Position = Vector3.one;
transform.EulerAngles = Vector3.zero;
transform.Scale = Vector3.one;
```
