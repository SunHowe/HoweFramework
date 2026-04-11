# 实体-组件系统

## 概述

Entity-Component 架构将游戏对象的数据（Component）和逻辑（通过 System 处理）分离，提供灵活的组合模式。

## 核心文件

| 文件 | 路径 |
|------|------|
| IGameEntity | `Gameplay/Framework/Entity/IGameEntity.cs` |
| GameEntityManager | `Gameplay/Framework/Entity/GameEntityManager.cs` |
| GameEntityManager.GameEntity | `Gameplay/Framework/Entity/GameEntityManager.GameEntity.cs` |
| IGameComponent | `Gameplay/Framework/Entity/IGameComponent.cs` |
| GameComponentBase | `Gameplay/Framework/Entity/GameComponentBase.cs` |
| GameComponentAttribute | `Gameplay/Framework/Entity/GameComponentAttribute.cs` |
| GameComponentType | `Gameplay/Framework/Entity/GameComponentType.cs` |

## IGameEntity

```csharp
public interface IGameEntity
{
    // 唯一标识
    int EntityId { get; }

    // 所属上下文
    IGameContext Context { get; }

    // 初始化
    void Awake(int entityId, IGameEntityManager entityManager);

    // 组件操作
    void AddComponent(IGameComponent component);
    IGameComponent GetComponent(int componentType);
    void RemoveComponent(int componentType);
    void GetComponents(List<IGameComponent> components);

    // 销毁
    void Dispose();
}
```

## IGameComponent

```csharp
public interface IGameComponent
{
    // 组件类型（唯一）
    int ComponentType { get; }

    // 组件实例 ID
    int ComponentId { get; }

    // 所属实体
    IGameEntity Entity { get; }

    // 初始化
    void Awake(int componentId, IGameEntity entity);

    // 销毁
    void Dispose();
}
```

## GameComponentBase

```csharp
public abstract class GameComponentBase : IGameComponent
{
    public int ComponentType { get; }
    public int ComponentId { get; private set; }
    public IGameEntity Entity { get; private set; }

    // 初始化回调
    protected virtual void OnAwake() { }

    // 销毁回调
    protected virtual void OnDispose() { }

    // 内部方法
    internal void DisposeFromEntity();

    // IGameComponent 实现
    public void Dispose();
}
```

## GameComponentAttribute

```csharp
[AttributeUsage(AttributeTargets.Class)]
public sealed class GameComponentAttribute : Attribute
{
    public int ComponentType { get; }

    public GameComponentAttribute(GameComponentType componentType)
    {
        ComponentType = (int)componentType;
    }
}
```

## GameComponentType 枚举

```csharp
public enum GameComponentType
{
    Transform = 1,
    View = 2,
    ViewTransformSync = 3,
    Numeric = 4,
    State = 5,
    Resource = 6,
}
```

## 基本用法

### 1. 创建自定义组件

```csharp
[GameComponent(GameComponentType.Numeric)]
public sealed class NumericComponent : GameComponentBase
{
    public event SimpleEventHandler<long> OnValueChanged;

    private long _value;

    public long Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }
    }

    protected override void OnAwake()
    {
        _value = 0;
    }

    protected override void OnDispose()
    {
        OnValueChanged = null;
    }
}
```

### 2. 创建实体

```csharp
var entityManager = context.GetManager<IGameEntityManager>();
var entity = entityManager.CreateEntity();
```

### 3. 添加组件

```csharp
entity.AddComponent<TransformComponent>();
entity.AddComponent<NumericComponent>();
```

### 4. 获取组件

```csharp
var transform = entity.GetComponent<TransformComponent>();
var numeric = entity.GetComponent<NumericComponent>();
```

### 5. 获取多个组件

```csharp
var components = new List<IGameComponent>();
entity.GetComponents(components);

foreach (var component in components)
{
    Debug.Log($"Component: {component.ComponentType}");
}
```

### 6. 销毁实体

```csharp
entityManager.DestroyEntity(entity.EntityId);
// 或
entity.Dispose();
```

## GameEntityHelper 扩展方法

```csharp
// 添加组件
T AddComponent<T>(this IGameEntity entity) where T : GameComponentBase, new();

// 获取组件
T GetComponent<T>(this IGameEntity entity) where T : GameComponentBase;

// 移除组件
void RemoveComponent<T>(this IGameEntity entity) where T : GameComponentBase;

// 获取位置
Vector3 GetPosition(this IGameEntity entity);

// 获取目标方向
Vector3 GetTargetDirection(this IGameEntity entity, IGameEntity targetEntity);
```

## 使用扩展方法

```csharp
var entity = context.GetManager<IGameEntityManager>().CreateEntity();

// 添加组件（更简洁）
entity.AddComponent<TransformComponent>();
entity.AddComponent<NumericComponent>();

// 获取组件
var transform = entity.GetComponent<TransformComponent>();
var numeric = entity.GetComponent<NumericComponent>();

// 移除组件
entity.RemoveComponent<NumericComponent>();
```

## 实体生命周期

```
CreateEntity()
    ↓
Entity.Awake()
    ↓
Entity.AddComponent(Component)
    ↓
Component.Awake() → OnAwake()
    ↓
... 使用中 ...
    ↓
DestroyEntity() 或 Entity.Dispose()
    ↓
所有组件的 Dispose()
    ↓
实体归还到池
```

## 完整示例

### 定义组件

```csharp
[GameComponent(GameComponentType.Transform)]
public sealed class TransformComponent : GameComponentBase
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
    public Vector3 Scale { get; set; } = Vector3.one;

    protected override void OnAwake()
    {
        Position = Vector3.zero;
        Rotation = Quaternion.identity;
        Scale = Vector3.one;
    }
}
```

### 创建游戏对象

```csharp
public class Example
{
    public void CreatePlayer()
    {
        var entityManager = context.GetManager<IGameEntityManager>();
        var entity = entityManager.CreateEntity();

        // 添加组件
        entity.AddComponent<TransformComponent>();
        entity.AddComponent<ViewComponent>();
        entity.AddComponent<NumericComponent>();

        // 设置数值
        var numeric = entity.GetComponent<NumericComponent>();
        numeric.SetValue(NumericType.HP, 100);

        return entity;
    }
}
```

## 最佳实践

### 1. 组件数据要可重置

```csharp
protected override void OnAwake()
{
    // 初始化默认值
    _health = 0;
    _mana = 0;
    _target = null;
}
```

### 2. 事件要用可空类型

```csharp
public event SimpleEventHandler<long> OnHealthChanged;

// 在 Dispose 时清空
protected override void OnDispose()
{
    OnHealthChanged = null;
}
```

### 3. 避免在组件间直接引用

```csharp
// 不好：组件 A 直接引用组件 B
public class ComponentA : GameComponentBase
{
    public ComponentB B { get; set; } // 耦合

    public void DoSomething()
    {
        B.DoSomething(); // 组件间紧耦合
    }
}

// 好：通过事件通信
public class ComponentA : GameComponentBase
{
    public event Action OnDoSomething;

    public void DoSomething()
    {
        OnDoSomething?.Invoke();
    }
}
```

### 4. 使用对象池

实体管理器内部使用对象池，组件也应该实现可复用：

```csharp
protected override void OnAwake()
{
    // 初始化
}

protected override void OnDispose()
{
    // 清理（但不要释放，实体管理器会复用）
}
```

## Reference 安全

### GameEntityRef

安全引用，实体销毁后变为 null：

```csharp
GameEntityRef entityRef = new GameEntityRef(entity);

if (entityRef.GameEntity != null)
{
    // 安全使用
}
```

### GameComponentRef

```csharp
GameComponentRef<TransformComponent> componentRef =
    entity.GetComponent<TransformComponent>();

if (componentRef.GameComponent != null)
{
    // 安全使用
}
```