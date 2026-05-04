# 数值系统

## 概述

NumericComponent 提供游戏数值/属性的管理，支持最终值计算和变化订阅。

## 核心文件

| 文件 | 路径 |
|------|------|
| NumericComponent | `Gameplay/Common/Numeric/NumericComponent.cs` |
| INumeric | `Gameplay/Common/Numeric/INumeric.cs` |
| NumericSubType | `Gameplay/Common/Numeric/NumericSubType.cs` |
| NumericHelper | `Gameplay/Common/Numeric/NumericHelper.cs` |
| NumericComponentExtensions | `Gameplay/Common/Numeric/NumericComponentExtensions.cs` |

## NumericSubType

数值计算公式：

```
FinalValue = (BasicValue * (1 + BasicPercent) + BasicConstAdd) * (1 + FinalPercent) + FinalConstAdd
```

```csharp
public enum NumericSubType
{
    Basic = 0,       // 基础值
    BasicPercent = 1, // 基础百分比加成
    BasicConstAdd = 2, // 基础固定加成
    FinalPercent = 3, // 最终百分比加成
    FinalConstAdd = 4, // 最终固定加成
    Final = 5,       // 最终值（计算结果）
}
```

## NumericComponent

```csharp
[GameComponent(GameComponentType.Numeric)]
public sealed class NumericComponent : GameComponentBase
{
    // 索引器访问
    public long this[int numericId] { get; set; }

    // 获取值（指定子类型）
    public long Get(int numericId, NumericSubType subType);

    // 设置值（指定子类型）
    public void Set(int numericId, NumericSubType subType, long value);

    // 修改值
    public void Modify(int numericId, NumericSubType subType, long delta);

    // 订阅变化
    public void Subscribe(int numericId, NumericSubType subType, SimpleEventHandler<long> handler, bool notifyImmediately = true);

    // 取消订阅
    public void Unsubscribe(int numericId, NumericSubType subType, SimpleEventHandler<long> handler);

    // 快照
    public INumeric TakeSnapshot();
    public void RestoreSnapshot(INumeric snapshot);
}
```

## INumeric

```csharp
public interface INumeric
{
    long this[int numericId] { get; set; }
    long Get(int numericId, NumericSubType subType);
    bool Contains(int numericId);
}
```

## NumericHelper

```csharp
public static class NumericHelper
{
    // 编码复合键
    public static int EncodeNumericKey(int id, NumericSubType subType);

    // 解码 ID
    public static int DecodeNumericId(int compositeKey);

    // 解码子类型
    public static NumericSubType DecodeSubType(int compositeKey);
}
```

## 基本用法

### 1. 定义数值 ID

```csharp
// 数值类型枚举
public enum NumericType
{
    // 生命值
    HP = 1,
    MaxHP = 2,

    // 攻击
    Attack = 10,
    CriticalRate = 11,
    CriticalDamage = 12,

    // 防御
    Defense = 20,
}
```

### 2. 设置基础值

```csharp
var numeric = entity.AddComponent<NumericComponent>();

// 设置最大生命
numeric.Set(NumericType.MaxHP, NumericSubType.Basic, 1000);

// 设置当前生命（满血）
numeric.Set(NumericType.HP, NumericSubType.Basic, 1000);

// 设置攻击力
numeric.Set(NumericType.Attack, NumericSubType.Basic, 100);
```

### 3. 添加百分比加成

```csharp
// 装备提供 +20% 攻击力
numeric.Set(NumericType.Attack, NumericSubType.BasicPercent, 2000); // 2000 = 20%

// 最终攻击力 = 100 * (1 + 0.2) = 120
```

### 4. 获取最终值

```csharp
// 获取最终生命值（自动计算所有加成）
long hp = numeric[NumericType.HP];

// 或明确获取
long hpFinal = numeric.Get(NumericType.HP, NumericSubType.Final);
```

### 5. 订阅变化

```csharp
numeric.Subscribe(NumericType.HP, NumericSubType.Final, OnHpChanged, notifyImmediately: true);

private void OnHpChanged(long newHp)
{
    Debug.Log($"HP changed to {newHp}");
}
```

### 6. 修改值

```csharp
// 受伤
numeric.Modify(NumericType.HP, NumericSubType.Basic, -50);

// 治疗
numeric.Modify(NumericType.HP, NumericSubType.Basic, 100);
```

## 快照和恢复

```csharp
// 保存快照
var snapshot = numeric.TakeSnapshot();

// 修改
numeric.Set(NumericType.HP, NumericSubType.Basic, 100);

// 恢复
numeric.RestoreSnapshot(snapshot);

// 现在 HP 又回到快照时的值
```

## 完整示例

### 角色属性组件

```csharp
[GameComponent(GameComponentType.Numeric)]
public sealed class CharacterNumericComponent : GameComponentBase
{
    public event Action<long> OnHPChanged;
    public event Action<long> OnMPChanged;

    private NumericComponent _numeric;

    public long HP => _numeric[NumericType.HP];
    public long MaxHP => _numeric[NumericType.MaxHP];
    public long MP => _numeric[NumericType.MP];
    public long MaxMP => _numeric[NumericType.MaxMP];
    public long Attack => _numeric[NumericType.Attack];
    public long Defense => _numeric[NumericType.Defense];

    protected override void OnAwake()
    {
        _numeric = Entity.AddComponent<NumericComponent>();

        // 初始化基础属性
        _numeric.Set(NumericType.MaxHP, NumericSubType.Basic, 1000);
        _numeric.Set(NumericType.HP, NumericSubType.Basic, 1000);
        _numeric.Set(NumericType.MaxMP, NumericSubType.Basic, 500);
        _numeric.Set(NumericType.MP, NumericSubType.Basic, 500);
        _numeric.Set(NumericType.Attack, NumericSubType.Basic, 100);
        _numeric.Set(NumericType.Defense, NumericSubType.Basic, 50);

        // 订阅变化
        _numeric.Subscribe(NumericType.HP, NumericSubType.Final, OnHpChanged);
        _numeric.Subscribe(NumericType.MP, NumericSubType.Final, OnMpChanged);
    }

    public void TakeDamage(long damage)
    {
        // 简化计算：实际应该考虑防御
        _numeric.Modify(NumericType.HP, NumericSubType.Basic, -damage);
    }

    public void Heal(long amount)
    {
        _numeric.Modify(NumericType.HP, NumericSubType.Basic, amount);
    }

    public void SpendMP(long cost)
    {
        _numeric.Modify(NumericType.MP, NumericSubType.Basic, -cost);
    }

    private void OnHpChanged(long hp)
    {
        OnHPChanged?.Invoke(hp);
    }

    private void OnMpChanged(long mp)
    {
        OnMPChanged?.Invoke(mp);
    }

    protected override void OnDispose()
    {
        OnHPChanged = null;
        OnMPChanged = null;
    }
}
```

### 使用

```csharp
var charNumeric = entity.AddComponent<CharacterNumericComponent>();

// 监听变化
charNumeric.OnHPChanged += hp => Debug.Log($"HP: {hp}/{charNumeric.MaxHP}");

// 受伤
charNumeric.TakeDamage(100);

// 治疗
charNumeric.Heal(50);
```

## 数值计算公式详解

```
FinalValue = (BasicValue * (1 + BasicPercent/10000) + BasicConstAdd) * (1 + FinalPercent/10000) + FinalConstAdd
```

示例：
- Basic = 1000
- BasicPercent = 2000 (20%)
- BasicConstAdd = 50
- FinalPercent = 1000 (10%)
- FinalConstAdd = 0

计算：
```
Step1 = 1000 * (1 + 0.2) + 50 = 1250
Final = 1250 * (1 + 0.1) + 0 = 1375
```

## 最佳实践

### 1. 使用常量定义数值类型

```csharp
public static class NumericType
{
    public const int HP = 1;
    public const int MaxHP = 2;
    public const int Attack = 10;
}
```

### 2. 统一使用 10000 作为百分比基数

```csharp
// 20% -> 2000
numeric.Set(NumericType.Attack, NumericSubType.BasicPercent, 2000);
```

### 3. 记得取消订阅

```csharp
protected override void OnDispose()
{
    _numeric.Unsubscribe(NumericType.HP, NumericSubType.Final, OnHpChanged);
}
```

### 4. 使用快照实现状态保存

```csharp
// 存档
var saveData = new CharacterSaveData();
saveData.NumericSnapshot = charNumeric.TakeSnapshot();

// 读档
charNumeric.RestoreSnapshot(saveData.NumericSnapshot);
```