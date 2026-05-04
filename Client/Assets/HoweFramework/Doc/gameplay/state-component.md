# 状态组件

## 概述

StateComponent 管理实体的布尔状态，支持引用计数和变化订阅。

## 核心文件

| 文件 | 路径 |
|------|------|
| StateComponent | `Gameplay/Common/State/StateComponent.cs` |

## StateComponent

```csharp
[GameComponent(GameComponentType.State)]
public sealed class StateComponent : GameComponentBase
{
    // 检查状态是否存在
    public bool CheckState(int stateId);

    // 添加状态（可指定提供者，支持引用计数）
    public void AddState(int stateId, object provider = null);

    // 移除状态
    public void RemoveState(int stateId, object provider = null);

    // 订阅状态变化
    public void Subscribe(int stateId, Action<bool> handler, bool notifyImmediately = true);

    // 取消订阅
    public void Unsubscribe(int stateId, Action<bool> handler);

    // 获取所有状态
    public IEnumerable<int> GetAllStates();
}
```

## 基本用法

### 1. 添加状态

```csharp
var state = entity.AddComponent<StateComponent>();

// 添加状态（无提供者）
state.AddState(BuffState.Freeze);

// 添加状态（有提供者）
state.AddState(BuffState.Freeze, this);
```

### 2. 检查状态

```csharp
if (state.CheckState(BuffState.Freeze))
{
    Debug.Log("Frozen!");
}
```

### 3. 移除状态

```csharp
state.RemoveState(BuffState.Freeze, this);
```

### 4. 订阅变化

```csharp
state.Subscribe(BuffState.Freeze, OnFreezeChanged, notifyImmediately: true);

private void OnFreezeChanged(bool isFrozen)
{
    if (isFrozen)
    {
        Debug.Log("Now frozen");
    }
    else
    {
        Debug.Log("No longer frozen");
    }
}
```

## 引用计数

同一个状态可以被添加多次，只要有一个提供者存在，状态就是激活的：

```csharp
// 技能 A 添加 Freeze 状态
state.AddState(BuffState.Freeze, skillA);

// 技能 B 也添加 Freeze 状态
state.AddState(BuffState.Freeze, skillB);

// 检查状态（仍然 true）
Debug.Log(state.CheckState(BuffState.Freeze)); // true

// 技能 A 结束，移除
state.RemoveState(BuffState.Freeze, skillA);

// 状态仍然存在（技能 B 还在）
Debug.Log(state.CheckState(BuffState.Freeze)); // true

// 技能 B 也结束
state.RemoveState(BuffState.Freeze, skillB);

// 现在状态被移除
Debug.Log(state.CheckState(BuffState.Freeze)); // false
```

## 定义状态 ID

```csharp
// 状态类型枚举
public static class BuffState
{
    public const int None = 0;
    public const int Freeze = 1;
    public const int Poison = 2;
    public const int Burn = 3;
    public const int Stun = 4;
    public const int Invisible = 5;
}

public static class PlayerState
{
    public const int Idle = 0;
    public const int Moving = 1;
    public const int Attacking = 2;
    public const int Dead = 3;
    public const int Dialog = 4;
}
```

## 完整示例

### Buff 系统

```csharp
public class BuffComponent : GameComponentBase
{
    private StateComponent _state;
    private Dictionary<int, float> _remainingTimes = new();

    // Buff 类型
    public const int Freeze = 1;
    public const int Poison = 2;
    public const int Burn = 3;

    protected override void OnAwake()
    {
        _state = Entity.AddComponent<StateComponent>();

        // 订阅所有 Buff 变化
        _state.Subscribe(Freeze, OnFreezeChanged);
        _state.Subscribe(Poison, OnPoisonChanged);
        _state.Subscribe(Burn, OnBurnChanged);
    }

    public void ApplyBuff(int buffId, float duration, object provider)
    {
        _state.AddState(buffId, provider);
        _remainingTimes[buffId] = duration;
    }

    public void RemoveBuff(int buffId, object provider)
    {
        _state.RemoveState(buffId, provider);
        _remainingTimes.Remove(buffId);
    }

    private void Update(float elapseSeconds)
    {
        // 更新持续时间
        var expired = new List<int>();
        foreach (var kvp in _remainingTimes)
        {
            kvp.Value -= elapseSeconds;
            if (kvp.Value <= 0)
            {
                expired.Add(kvp.Key);
            }
        }

        // 移除过期 Buff
        foreach (var buffId in expired)
        {
            RemoveBuff(buffId, this);
        }
    }

    private void OnFreezeChanged(bool isFrozen)
    {
        // 冻结时禁止移动
        var movement = Entity.GetComponent<MovementComponent>();
        if (movement != null)
        {
            movement.enabled = !isFrozen;
        }
    }

    private void OnPoisonChanged(bool isPoisoned)
    {
        // 中毒逻辑
    }

    private void OnBurnChanged(bool isBurning)
    {
        // 燃烧逻辑
    }

    protected override void OnDispose()
    {
        _remainingTimes.Clear();
    }
}
```

### 玩家状态机

```csharp
public class PlayerStateComponent : GameComponentBase
{
    private StateComponent _state;

    public bool IsIdle => _state.CheckState(PlayerState.Idle);
    public bool IsMoving => _state.CheckState(PlayerState.Moving);
    public bool IsAttacking => _state.CheckState(PlayerState.Attacking);
    public bool IsDead => _state.CheckState(PlayerState.Dead);
    public bool IsInDialog => _state.CheckState(PlayerState.Dialog);

    protected override void OnAwake()
    {
        _state = Entity.AddComponent<StateComponent>();
        _state.AddState(PlayerState.Idle); // 默认 Idle
    }

    public void StartMove()
    {
        _state.RemoveState(PlayerState.Idle, this);
        _state.AddState(PlayerState.Moving, this);
    }

    public void StopMove()
    {
        _state.RemoveState(PlayerState.Moving, this);
        _state.AddState(PlayerState.Idle, this);
    }

    public void StartAttack()
    {
        _state.AddState(PlayerState.Attacking, this);
    }

    public void EndAttack()
    {
        _state.RemoveState(PlayerState.Attacking, this);
    }

    public void Die()
    {
        _state.RemoveState(PlayerState.Idle, this);
        _state.RemoveState(PlayerState.Moving, this);
        _state.RemoveState(PlayerState.Attacking, this);
        _state.AddState(PlayerState.Dead, this);
    }
}
```

## 与 NumericComponent 的区别

| 特性 | StateComponent | NumericComponent |
|------|----------------|------------------|
| 数据类型 | bool | long |
| 用途 | 状态标记 | 数值属性 |
| 引用计数 | 支持 | 不需要 |
| 子类型 | 无 | 有（Basic、Percent 等） |
| 示例 | Freeze、Stun | HP、Attack、Defense |

## 最佳实践

### 1. 使用常量定义状态 ID

```csharp
public static class Status
{
    public const int CanMove = 1;
    public const int CanAttack = 2;
    public const int Invincible = 3;
}
```

### 2. 提供者用于精确控制

```csharp
// 多个技能可能影响同一个状态
skillA.OnApply += () => state.AddState(Status.CanMove, skillA);
skillB.OnApply += () => state.AddState(Status.CanMove, skillB);

skillA.OnRemove += () => state.RemoveState(Status.CanMove, skillA);
skillB.OnRemove += () => state.RemoveState(Status.CanMove, skillB);
```

### 3. 记得清理

```csharp
protected override void OnDispose()
{
    _state.Unsubscribe(Freeze, OnFreezeChanged);
}
```