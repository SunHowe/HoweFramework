# 资源组件

## 概述

ResourceComponent 管理实体的可消耗资源（如 HP、MP、Energy），支持最大值绑定和自动消耗。

## 核心文件

| 文件 | 路径 |
|------|------|
| ResourceComponent | `Gameplay/Common/Resource/ResourceComponent.cs` |

## ResourceComponent

```csharp
[GameComponent(GameComponentType.Resource)]
public sealed class ResourceComponent : GameComponentBase
{
    // 索引器访问
    public long this[int resourceId] { get; set; }

    // 设置值
    public void Set(int resourceId, long value);

    // 修改值（可检查是否足够）
    public bool Cost(int resourceId, long value);
    public void Modify(int resourceId, long delta);

    // 绑定最大值到 NumericComponent
    public void BindNumericMax(int resourceId, int maxNumericId, NumericComponent numericComponent);

    // 订阅变化
    public void Subscribe(int resourceId, Action<long> handler, bool notifyImmediately = true);
    public void Unsubscribe(int resourceId, Action<long> handler);

    // 检查是否足够
    public bool IsEnough(int resourceId, long value);
}
```

## 基本用法

### 1. 添加组件

```csharp
var resource = entity.AddComponent<ResourceComponent>();
```

### 2. 设置资源值

```csharp
// 设置 HP
resource.Set(ResourceType.HP, 1000);

// 设置 MP
resource.Set(ResourceType.MP, 500);
```

### 3. 获取当前值

```csharp
long hp = resource[ResourceType.HP];
long mp = resource[ResourceType.MP];
```

### 4. 修改值

```csharp
// 受伤
resource.Modify(ResourceType.HP, -50);

// 治疗
resource.Modify(ResourceType.HP, 100);

// 消耗 MP
resource.Modify(ResourceType.MP, -30);
```

### 5. 消耗（检查足够）

```csharp
// 消耗 MP，但只在前足够时成功
if (resource.Cost(ResourceType.MP, 100))
{
    Debug.Log("Skill cast!");
}
else
{
    Debug.Log("Not enough MP!");
}
```

### 6. 绑定最大值

```csharp
var numeric = entity.GetComponent<NumericComponent>();

// HP 上限绑定到 MaxHP
resource.BindNumericMax(ResourceType.HP, NumericType.MaxHP, numeric);

// 当 MaxHP 变化时，HP 会自动调整（如果超限会降为上限）
```

### 7. 订阅变化

```csharp
resource.Subscribe(ResourceType.HP, OnHpChanged, notifyImmediately: true);

private void OnHpChanged(long newHp)
{
    Debug.Log($"HP: {newHp}");
}
```

## 资源类型定义

```csharp
public static class ResourceType
{
    public const int HP = 1;      // 生命值
    public const int MP = 2;      // 魔法值
    public const int Energy = 3; // 能量
    public const int Rage = 4;   // 怒气
}
```

## 自动上限限制

当设置了上限（通过绑定 Numeric 或手动设置）时，值会自动限制在 [0, MaxValue] 范围内：

```csharp
// HP 上限为 1000
resource.BindNumericMax(ResourceType.HP, NumericType.MaxHP, numeric);

// 设置超限的值会被自动截断
resource.Set(ResourceType.HP, 2000); // 自动变为 1000

// 扣除超限时也会截断到 0
resource.Modify(ResourceType.HP, -2000); // 自动变为 0
```

## 完整示例

### 角色资源管理

```csharp
public class CharacterResourceComponent : GameComponentBase
{
    private ResourceComponent _resource;
    private NumericComponent _numeric;

    public event Action<long, long> OnHPChanged; // (current, max)
    public event Action<long, long> OnMPChanged;
    public event Action OnDead;
    public event Action OnRevive;

    public long HP => _resource[ResourceType.HP];
    public long MaxHP => _numeric[NumericType.MaxHP];
    public long MP => _resource[ResourceType.MP];
    public long MaxMP => _numeric[NumericType.MaxMP];

    protected override void OnAwake()
    {
        _numeric = Entity.AddComponent<NumericComponent>();
        _resource = Entity.AddComponent<ResourceComponent>();

        // 初始化基础属性
        _numeric.Set(NumericType.MaxHP, NumericSubType.Basic, 1000);
        _numeric.Set(NumericType.MaxMP, NumericSubType.Basic, 500);

        // 初始化资源
        _resource.Set(ResourceType.HP, _numeric[NumericType.MaxHP]);
        _resource.Set(ResourceType.MP, _numeric[NumericType.MaxMP]);

        // 绑定上限
        _resource.BindNumericMax(ResourceType.HP, NumericType.MaxHP, _numeric);
        _resource.BindNumericMax(ResourceType.MP, NumericType.MaxMP, _numeric);

        // 订阅变化
        _resource.Subscribe(ResourceType.HP, OnHpChanged);
        _resource.Subscribe(ResourceType.MP, OnMpChanged);
    }

    public bool TakeDamage(long damage)
    {
        if (!_resource.Cost(ResourceType.HP, damage))
        {
            // HP 不足
            Debug.Log("Not enough HP!");
            return false;
        }

        if (_resource[ResourceType.HP] <= 0)
        {
            OnDead?.Invoke();
        }

        return true;
    }

    public void Heal(long amount)
    {
        _resource.Modify(ResourceType.HP, amount);
    }

    public bool UseSkill(long mpCost)
    {
        if (!_resource.Cost(ResourceType.MP, mpCost))
        {
            Debug.Log("Not enough MP!");
            return false;
        }
        return true;
    }

    public void Revive()
    {
        _resource.Set(ResourceType.HP, MaxHP);
        _resource.Set(ResourceType.MP, MaxMP);
        OnRevive?.Invoke();
    }

    private void OnHpChanged(long hp)
    {
        OnHPChanged?.Invoke(hp, MaxHP);
    }

    private void OnMpChanged(long mp)
    {
        OnMPChanged?.Invoke(mp, MaxMP);
    }

    protected override void OnDispose()
    {
        OnHPChanged = null;
        OnMPChanged = null;
        OnDead = null;
        OnRevive = null;
    }
}
```

### 使用

```csharp
var charResource = entity.AddComponent<CharacterResourceComponent>();

// 监听变化
charResource.OnHPChanged += (hp, maxHp) =>
    Debug.Log($"HP: {hp}/{maxHp}");

charResource.OnDead += () =>
    Debug.Log("Player died!");

// 受伤
charResource.TakeDamage(100);

// 治疗
charResource.Heal(50);

// 释放技能（消耗 MP）
if (charResource.UseSkill(30))
{
    // 技能释放成功
}
```

## 与 NumericComponent 的区别

| 特性 | ResourceComponent | NumericComponent |
|------|-------------------|-------------------|
| 数据类型 | long | long |
| 主要用途 | 可消耗资源 | 任意数值属性 |
| 最大值限制 | 有（自动限制） | 无（需要手动处理） |
| 消耗操作 | Cost（检查足够） | Modify |
| 上限绑定 | 支持 | 不支持 |
| 典型场景 | HP、MP、Energy | Attack、Defense、Speed |

## 最佳实践

### 1. 使用事件而非每帧检查

```csharp
// 好：订阅变化
resource.Subscribe(ResourceType.HP, OnHpChanged);

// 不好：每帧检查
private void Update()
{
    if (resource[ResourceType.HP] != _lastHp)
    {
        _lastHp = resource[ResourceType.HP];
        UpdateUI();
    }
}
```

### 2. 使用 Cost 而非直接扣除

```csharp
// 好：使用 Cost 检查
if (resource.Cost(ResourceType.MP, skillCost))
{
    CastSkill();
}

// 不好：直接修改后检查
resource.Modify(ResourceType.MP, -skillCost);
if (resource[ResourceType.MP] < 0) // 已经扣除了
{
    // 处理失败
}
```

### 3. 死亡检查在修改后进行

```csharp
public void TakeDamage(long damage)
{
    resource.Cost(ResourceType.HP, damage);

    // 在扣除后检查是否死亡
    if (resource[ResourceType.HP] <= 0)
    {
        OnDead?.Invoke();
    }
}
```