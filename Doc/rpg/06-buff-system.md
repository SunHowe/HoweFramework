# 06 · Buff 系统 —— 典型 Provider 的完整实现

> **架构澄清**(承接 [`05-state-component.md`](05-state-component.md)):`StateComponent` 是底层基础设施,**不**等于 buff。buff 是 **Provider 的典型代表** —— 自己持有 duration / tick / dispellable / source / level,自己决定何时调 `StateComponent.AddState / RemoveState`。
>
> 本章沉淀 buff 系统作为典型 Provider 的完整实现 —— 不含具体游戏数值,只沉淀"为什么这样设计"。

---

## 一、buff 作为典型 Provider

### 1.1 为什么 buff 是 Provider 的"典型"

buff 系统是 Provider 模式最常碰到的业务场景:
- 有持续时间(回合数 / 秒数)
- 有 tick 逻辑(中毒扣血 / 灼烧 / 治疗)
- 有驱散规则(能否被净化)
- 有 owner / source(target / caster)
- 有等级(普通 / 高级)
- 有多个 buff 可叠加 / 覆盖

**所有这些属性都跟 Provider 模式天然契合**。其他 Provider(Skill Effect / Death Flag / Aura Effect)可以参考 buff 的模式实现。

### 1.2 buff ≠ state

| 维度 | buff | state |
|------|------|-------|
| **本质** | Provider(业务对象) | 集合成员关系(底层机制) |
| **持有数据** | duration / tick / dispellable / source / level | (stateId, provider) 集合 |
| **典型实现** | `class BuffPoison : IBuffProvider` | `StateComponent` |
| **生命周期** | 自己持有 RemainingRounds | 实体存活期间 |
| **典型行为** | "3 回合后我自己 RemoveState" | "我接受 Provider 的 Add/Remove,自己跟踪引用计数" |

**核心**:**buff 是 Provider 的典型代表,不是 state 本身**。state 是 StateComponent 里的"我当下有什么 stateId",buff 是"为什么这个 state 处于活跃"的来源。

---

## 二、buff 的数据结构

### 2.1 基础 buff 字段

```csharp
public interface IBuffProvider {
    int StateId { get; }              // 这个 buff 注册到 StateComponent 时用的 stateId
    IGameEntity Target { get; }       // buff 施加在谁身上
    object Caster { get; }            // 谁施加的 buff
    int RemainingRounds { get; }      // 剩余回合数(0 时退出)
    bool Dispellable { get; }         // 能否被净化
    BuffCategory Category { get; }    // buff 分类(增益 / 减益 / 控制 / 特殊)

    void OnApply(BattleContext ctx);  // 初始化:调 AddState
    void OnTick(BattleContext ctx);   // 每回合:减计时 + 做效果
    void OnRemove(BattleContext ctx); // 退出:调 RemoveState
}
```

### 2.2 进阶 buff 字段

```csharp
public interface IAdvancedBuffProvider : IBuffProvider {
    BuffStackingPolicy StackingPolicy { get; }  // 同类 buff 叠加规则
    int MaxStacks { get; }                      // 最多叠加层数
    bool RefreshDurationOnReapply { get; }     // 重复施加是否刷新持续时间
    BuffDispelType DispelType { get; }          // 驱散类型(净化 / 解除 / 自然)
}

public enum BuffStackingPolicy {
    Independent,        // 独立叠加(每个 Provider 实例独立)
    Refresh,            // 刷新持续时间(覆盖)
    Stack,              // 叠加层数
    Ignore,             // 同类忽略
}
```

### 2.3 buff 分类(业务维度)

```csharp
public enum BuffCategory {
    Buff,           // 增益 buff
    Debuff,         // 减益 debuff
    Control,        // 控制
    Special,        // 特殊(倒地 / 死亡 / 鬼魂)
}
```

---

## 三、buff 系统的实现模式

### 3.1 标准 buff 实现

```csharp
public class BuffPoison : IBuffProvider {
    public int StateId => StatusId.Poison;
    public IGameEntity Target { get; }
    public object Caster { get; }
    public int RemainingRounds { get; private set; }
    public bool Dispellable => true;
    public BuffCategory Category => BuffCategory.Debuff;

    public BuffPoison(IGameEntity target, object caster, int duration) {
        Target = target;
        Caster = caster;
        RemainingRounds = duration;
    }

    public void OnApply(BattleContext ctx) {
        Target.GetComponent<StateComponent>().AddState(StateId, this);
    }

    public void OnTick(BattleContext ctx) {
        // 自己减计时
        RemainingRounds--;
        // 自己做效果(中毒扣血)
        var damage = ComputePoisonDamage(ctx);  // 业务公式
        Target.GetComponent<ResourceComponent>().Modify(ResourceId.HP, -damage);
    }

    public void OnRemove(BattleContext ctx) {
        Target.GetComponent<StateComponent>().RemoveState(StateId, this);
    }
}
```

### 3.2 业务层"回合开始"事件

```csharp
public sealed class BattleManager : GameManagerBase {
    private readonly List<IBuffProvider> m_AllActiveBuffs = new();

    private void OnRoundStartEnter() {
        CurrentRound++;
        // 遍历所有活跃 buff,每个 Provider 自己减 / 检持续时间
        for (int i = m_AllActiveBuffs.Count - 1; i >= 0; i--) {
            var buff = m_AllActiveBuffs[i];
            buff.OnTick(m_Context);  // Provider 自己减计时 + 做效果
            if (buff.RemainingRounds <= 0) {
                buff.OnRemove(m_Context);  // Provider 自己注销
                m_AllActiveBuffs.RemoveAt(i);
                m_Context.BuffPool.Release(buff);  // 归还池
            }
        }
    }
}
```

### 3.3 buff 施加的标准流程

```
1. 业务逻辑决定施加 buff
   (例:玩家 A 用"中毒符"技能 → 对目标 B 施加中毒 buff)

2. 创建 buff 实例
   var poisonBuff = m_Context.BuffPool.Acquire<BuffPoison>(target, caster, duration=3);
   m_AllActiveBuffs.Add(poisonBuff);

3. 调用 OnApply
   poisonBuff.OnApply(ctx);
   → target.StateComponent.AddState(StatusId.Poison, this);

4. (回合开始时)遍历所有活跃 buff
   poisonBuff.OnTick(ctx);
   → RemainingRounds--;
   → target.ResourceComponent.Modify(HP, -10);

5. (持续时间到)调用 OnRemove
   if (RemainingRounds <= 0) {
       poisonBuff.OnRemove(ctx);
       → target.StateComponent.RemoveState(StatusId.Poison, this);
   }
```

---

## 四、buff 的"4 类分类"(业务维度,不是 StateComponent 维度)

### 4.1 为什么是这 4 类

业务上,buff 系统天然分成 4 类:

| 类别 | 描述 | 典型状态 | 持续 |
|------|------|---------|------|
| **增益 buff** | 对己方有利的状态 | 隐身、慧根、强力、金刚护体 | 1-5 回合 |
| **减益 debuff** | 对己方不利但非控制的状态 | 中毒、遗忘、冰冻、虚弱、灼烧 | 1-5 回合 |
| **控制** | 限制行动 | 混乱、睡眠、封印、定身、嘲讽 | 1-5 回合 |
| **特殊** | 不属于前 3 类的特殊标记 | 倒地、死亡、鬼魂、变身 | 战斗内 |

### 4.2 这 4 类都是 Provider(不是 state)

| 类别 | Provider 典型 | stateId 典型 |
|------|--------------|--------------|
| **增益 buff** | `BuffInvisible` / `BuffJinGang` | `StatusId.Invisible` / `StatusId.JinGang` |
| **减益 debuff** | `BuffPoison` / `BuffBurn` | `StatusId.Poison` / `StatusId.Burning` |
| **控制** | `BuffSeal` / `BuffSleep` | `StatusId.Seal` / `StatusId.Sleep` |
| **特殊** | `DeathFlag` / `GhostFlag` | `StatusId.Down` / `StatusId.Dead` / `StatusId.Ghost` |

### 4.3 buff 分类的设计意图

- 决策维度清晰(每类 buff 有不同的处理方式)
- 解除方式不同(净化只对减益 / 控制有效)
- UI 显示不同(头像右侧的图标颜色)
- AI 处理不同(净化技能只解除减益 / 控制)

---

## 五、buff 系统的常见模式

### 5.1 同类 buff 覆盖

**常见规则**(梦幻西游 / 大话西游):
- 同类 buff 多次施加 → 以**最后一次为准**(覆盖)
- 例:中毒 3 回合 + 中毒 5 回合 → 中毒 5 回合(覆盖,而不是累加)

**实现**:`BuffPoison.OnApply` 检查同状态是否已存在,如果有就**覆盖**持续时间:

```csharp
public void OnApply(BattleContext ctx) {
    // 检查同状态是否已存在
    if (Target.GetComponent<StateComponent>().CheckState(StateId)) {
        // 已存在:覆盖持续时间
        var existing = FindExistingBuffByStateId(StateId);
        existing?.Refresh(RemainingRounds);  // 刷新
        return;
    }
    // 不存在:正常施加
    Target.GetComponent<StateComponent>().AddState(StateId, this);
}
```

### 5.2 buff 叠加 vs 覆盖

| 模式 | 描述 | 例子 |
|------|------|------|
| **叠加** | 同一 buff 可叠加(中毒 Lv1 + 中毒 Lv2 = 中毒 Lv2 高伤害) | 不同等级中毒 |
| **覆盖** | 同类 buff 覆盖(中毒 3 回合 + 中毒 5 回合 = 中毒 5 回合) | 同等级中毒 |
| **共存** | 不同 buff 可共存(中毒 + 灼烧 = 各自 Tick) | 不同 Provider |

### 5.3 抗性机制

**常见模式**(高级神迹抗封):
- 某些 buff 有"抗性"(基础 80-95%)
- 命中时随机判定是否抵抗
- 抗性高的单位更难被施加

**实现**:在 `OnApply` 时调用抗性检查,失败则不施加:

```csharp
public void OnApply(BattleContext ctx) {
    // 抗性检查
    var resistance = Target.GetComponent<NumericComponent>()
        .Get(NumericId.ResistanceSeal, NumericSubType.Final);
    var randomValue = ctx.RandomManager.GetRandom(0, 100);
    if (randomValue < resistance) {
        // 抵抗成功,不施加
        return;
    }
    // 抵抗失败,正常施加
    Target.GetComponent<StateComponent>().AddState(StateId, this);
}
```

### 5.4 净化机制

**常见模式**(净化 / 解毒):
- 净化技能可以一次性移除所有减益
- 部分净化(解毒)只移除特定类型

**实现**:`BuffSystem.RemoveAll(target, category)` 实现"净化类别 X":

```csharp
public sealed class BuffSystem {
    public void RemoveAll(IGameEntity target, BuffCategory category, BattleContext ctx) {
        // 遍历目标的所有 buff,移除指定类别
        for (int i = m_AllActiveBuffs.Count - 1; i >= 0; i--) {
            var buff = m_AllActiveBuffs[i];
            if (buff.Target == target && buff.Category == category && buff.Dispellable) {
                buff.OnRemove(ctx);
                m_AllActiveBuffs.RemoveAt(i);
                ctx.BuffPool.Release(buff);
            }
        }
    }
}
```

### 5.5 buff 等级

**常见模式**:
- 同一 buff 有 Lv1-9,每级提升效果
- 同一 stateId,不同 Provider(每个 Provider 持自己的 level)
- 例:普通中毒(Provider Lv1) + 高级中毒(Provider Lv2) → 同一 stateId = `Poison`,StateComponent 引用计数 2

### 5.6 buff 状态图标 UI

| buff 类别 | UI 表现 |
|----------|---------|
| 增益 | 头像右侧绿色图标 + 倒计时数字 |
| 减益 | 头像右侧红色图标 + 倒计时数字 |
| 控制 | 头像右侧灰色图标 + 倒计时数字 |
| 特殊 | 头像底部"倒地 / 死亡 / 鬼魂"字样 |

---

## 六、特殊 buff(倒地 / 死亡 / 鬼魂)

### 6.1 倒地 vs 死亡 vs 鬼魂

| 状态 | 触发 | 行为 | 复活 |
|------|------|------|------|
| **倒地** | 玩家 HP=0 | 不能行动,本场可救起 | 复活技能 / 复活药品 |
| **死亡** | NPC / 召唤兽 HP=0 | 当回合不可再出战 | 一般不可复活 |
| **鬼魂** | 鬼魂系单位 HP=0 | 可复活(次数限制)| 战斗结束前复活不算死亡 |

### 6.2 特殊 buff 也是 Provider

```csharp
public class DeathFlag : IBuffProvider {
    public int StateId => StatusId.Dead;
    public IGameEntity Target { get; }
    public object Caster => "self";  // 自己造成
    public int RemainingRounds => -1;  // 战斗内持续,不用倒计时
    public bool Dispellable => false;  // 不可净化
    public BuffCategory Category => BuffCategory.Special;

    public DeathFlag(IGameEntity target) {
        Target = target;
    }

    public void OnApply(BattleContext ctx) {
        Target.GetComponent<StateComponent>().AddState(StateId, this);
    }

    public void OnTick(BattleContext ctx) {
        // 死亡不需要 tick
    }

    public void OnRemove(BattleContext ctx) {
        Target.GetComponent<StateComponent>().RemoveState(StateId, this);
    }
}
```

### 6.3 特殊 buff 的 UI 表现

| 状态 | UI |
|------|-----|
| **倒地** | 单位灰显 + "倒地"字样 |
| **死亡** | 单位消失 + 留下尸体 |
| **鬼魂** | 单位半透明 + 飘忽 |

---

## 七、buff 系统的常见边界

### 7.1 buff vs 资源

| buff | 资源(HP / MP)|
|-----|--------------|
| Provider(业务对象)| 数值(可计算)|
| 自己持有 RemainingRounds | 自己持有 Current / Max |
| 自己实现 OnTick | 自己实现 Modify / Cost |
| **底层** = StateComponent | **底层** = ResourceComponent |

### 7.2 buff vs 状态(state)

| buff | state |
|-----|------|
| Provider(业务对象)| 集合成员关系(底层机制)|
| 自己持有 duration | 不持有时间 |
| 自己实现 Tick / 驱散 | 不持有效果 |
| **典型 Provider** | **StateComponent 持有** |

### 7.3 buff vs 属性

| buff | 属性 |
|-----|------|
| Provider(业务对象)| 数值(可计算)|
| 自己持有 duration | 永久(直到改)|
| 自己实现 Tick | 自己实现计算 |
| **底层** = StateComponent | **底层** = NumericComponent |

### 7.4 buff vs 倒计时

| buff | 倒计时(GameTimerManager)|
|-----|------------------------|
| Provider(业务对象)| 倒计时触发器 |
| 自己持有 RemainingRounds | 持有 interval / callback |
| 自己实现 Tick | 倒计时到触发 callback |
| **典型 Provider** | **GameTimerManager 持有** |

---

## 八、本章沉淀的设计模式

沉淀的不是具体某款 RPG 的 buff 数值,**而是 buff 作为典型 Provider 的设计骨架**:

1. **buff = Provider 的典型代表** —— 自己持有 duration / tick / dispellable / source / level。
2. **buff ≠ state** —— buff 是 Provider,state 是 StateComponent 里的"我当下有什么 stateId"。
3. **Provider 自己决定何时 AddState / RemoveState** —— 持续时间到 / 被驱散 / owner 死亡都是 Provider 的事。
4. **业务层"回合开始"事件遍历所有活跃 Provider** —— 每个 Provider 自己减 / 检持续时间。
5. **同类 buff 覆盖** —— 同 buff 多次施加,以最后一次为准。
6. **抗性 / 净化机制** —— 在 OnApply 时检查抗性,在 BuffSystem.RemoveAll 实现净化。
7. **特殊 buff(倒地 / 死亡 / 鬼魂)** —— 也是 Provider,只是分类在 Special。
8. **buff 池化** —— Provider 也需要池化,典型实现 `BuffPool.Acquire<T> / Release`。

具体数值(中毒 3 回合、隐身 5 回合、金刚护体 2 回合)跟着业务代码走,**不**沉淀到这里。

---

## 参考来源

沉淀自 2026-06 HoweFramework 框架分析 + 2026-06 用户架构澄清(buff 是 Provider 的典型代表,不是 state 本身)。**不指向任何具体游戏的数值表**,只沉淀通用设计骨架。