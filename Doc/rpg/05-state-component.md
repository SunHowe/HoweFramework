# 05 · StateComponent —— 玩法底层的"集合"基础设施

> **架构澄清**(本章核心):`StateComponent` 是**玩法底层基础设施**,**不**等于 buff。它只关心"我当下有什么 stateId";持续时间 / 效果 / 驱散 —— 都是 **Provider 的属性**,不是 StateComponent 的。
>
> 典型 Provider 的完整模式见 [`06-buff-system.md`](06-buff-system.md)。

---

## 一、StateComponent 的本质

### 1.1 三句话总结

1. `StateComponent` 是实体上的"当前哪些 `stateId` 处于活跃"的**集合(set)成员关系追踪机制**。
2. 它**只**做 3 件事:(stateId, provider) 复合键的集合成员关系、按 provider 引用计数、状态切换通知。
3. 它**不**做持续时间、不做 tick、不做驱散规则 —— 这些都是 **Provider** 的事。

### 1.2 核心定位:基础设施 vs 业务对象

| 维度 | StateComponent(基础设施) | Provider(业务对象,典型 = Buff) |
|------|------------------------|------------------------------|
| **职责** | "我当下有什么 stateId" | "为什么这个 state 处于活跃 + 持续多久 + 什么效果" |
| **持有数据** | (stateId, provider) 集合 | duration / tick / dispellable / source / level |
| **生命周期** | 实体存活期间 | Provider 自己持有,持续时间到 / 被驱散 / owner 死亡 → 自己决定退出 |
| **典型代表** | 唯一:StateComponent | 多样:Buff / SkillEffect / DeathFlag / AuraEffect / EquipmentPassive |

### 1.3 为什么要这样分离

**反例**(一切皆状态):
- 试图把"持续 N 回合"塞进 `StateComponent` → StateComponent 膨胀,失去"集合机制"的纯粹性
- 把"buff 效果 / Tick / 驱散"全塞进 StateComponent → StateComponent 变成万能类,难以维护

**正解**(底层 + Provider):
- `StateComponent` 保持纯粹的"集合机制",只回答"我当下有什么 stateId"
- 每个 stateId 的来源(buff / skill effect / flag / 光环 / …)都是 **Provider**
- Provider 自己持有 duration / tick / dispellable / source / level
- Provider 决定何时调 `AddState / RemoveState`

---

## 二、StateComponent 的 API(底层机制)

### 2.1 关键 API(只关心集合成员关系 + 切换通知)

```csharp
// 1. 检查
bool CheckState(int stateId);

// 2. 增删(始终带 provider)
void AddState(int stateId, object provider);
void RemoveState(int stateId, object provider);

// 3. 订阅切换
void Subscribe(int stateId, Action<bool> handler, bool notifyImmediately = false);
void Unsubscribe(int stateId, Action<bool> handler);

// 4. 查询
IEnumerable<int> GetAllStates();
```

### 2.2 API 不做的事

StateComponent **不持有、不暴露**:
- ❌ 持续时间(duration / remaining rounds)
- ❌ Tick 逻辑(中毒扣血 / 灼烧伤害)
- ❌ 驱散规则(dispellable / purgeable)
- ❌ owner / source(谁施加的 / 施加在谁身上)
- ❌ 等级(普通中毒 / 高级中毒)
- ❌ 任何业务字段

**这些全部归 Provider**。见 [`06-buff-system.md`](06-buff-system.md) 的典型 Provider 实现。

---

## 三、引用计数语义

### 3.1 (stateId, provider) 复合键

StateComponent 的内部状态用 **(stateId, provider)** 复合键追踪,**不**是只用 stateId。

```csharp
// 内部大致结构(伪代码)
Dictionary<(int stateId, object provider), int> m_RefCounts;
```

### 3.2 引用计数的精确语义

```
1. AddState(Poison, buffA)   → Poison 在 StateComponent 引用计数 1
2. AddState(Poison, buffB)   → Poison 在 StateComponent 引用计数 2(stateId 仍活跃)
3. RemoveState(Poison, buffA) → Poison 在 StateComponent 引用计数 1(stateId 仍活跃,buffB 还在)
4. RemoveState(Poison, buffB) → Poison 在 StateComponent 引用计数 0(stateId 真正退出)
```

### 3.3 引用计数语义的设计意图

**意图**:**区分施加者** —— Provider 是状态来源 / 主人,不是匿名 boolean。

**典型场景**(必须用引用计数):
- 玩家 A 给目标加中毒(buffA),玩家 B 也给同一目标加中毒(buffB)→ 目标处于中毒状态。
- buffA 被驱散 → 目标仍中毒(buffB 还在)。
- buffB 被驱散 → 目标真正退出中毒。

**反例**(如果只用 stateId,不带 provider):
- "RemoveState(Poison)" → 不知道是谁施加的,要么全部删除(误删 buffB 的中毒),要么全部保留(buffA 驱散失败)。

### 3.4 引用计数的常见误用

| 误用 | 问题 |
|------|------|
| ❌ 不带 provider 调 `RemoveState(stateId)` | 误删其他 provider 加的状态 |
| ❌ 用 `Dictionary<int, int>` 存"buff 名 → 持续回合" | 丢失"施加者是谁"信息 |
| ❌ 把 buff 用 `bool` 表示 | 不同等级无法区分 |
| ❌ 在 StateComponent 内置 `_durations:Dictionary<int,int>` | StateComponent 失去纯粹性,变成 buff 实现 |

---

## 四、Provider 模式(Provider Pattern)

### 4.1 Provider 是什么

Provider = **"为什么这个 state 处于活跃"的来源 / 主人**。

| Provider 类别 | 典型场景 | 是不是 state |
|--------------|---------|--------------|
| **Buff 实例** | 中毒 3 回合、隐身 5 回合、金刚护体 | ✅ state |
| **Skill Effect** | 法术命中灼烧、感电、冰冻 | ✅ state |
| **Death Flag** | 倒地、死亡、鬼魂标记 | ✅ state |
| **Equipment Passive** | 装备光环、套装效果 | ✅ state |
| **Aura Effect** | 范围光环(队伍 / 区域)| ✅ state |
| **World Effect** | 地形 / 天气 / 时段 | ✅ state |
| **裸数据 flag** | 已攻击过、已对话过 | ❌ 不是 state(用普通 bool 字段)|

### 4.2 Provider 必须自管理的属性

| 属性 | 描述 | 是不是 StateComponent 的事 |
|------|------|---------------------------|
| **持续时间**(`RemainingRounds` / `RemainingSeconds`)| 到 0 时 Provider 自己调 `RemoveState` | ❌ Provider 的事 |
| **Tick 逻辑**(每回合做什么)| Provider 自己实现 `OnTick` | ❌ Provider 的事 |
| **能否被驱散**(`Dispellable`)| 净化技能扫 Provider 时,Provider 自己判断 | ❌ Provider 的事 |
| **owner / source** | Provider 自己持有谁施加、施加在谁身上 | ❌ Provider 的事 |
| **等级**(普通 / 高级中毒)| Provider 自己区分(stateId 仍可共享)| ❌ Provider 的事 |
| **触发条件**(何时施加)| 业务逻辑决定 | ❌ Provider 的事 |
| **stateId 选取** | 业务逻辑决定施加哪个 stateId | ❌ Provider 的事 |

### 4.3 Provider 的接口(典型)

```csharp
public interface IBuffProvider {
    // 必须的属性
    IGameEntity Target { get; }
    object Caster { get; }
    int RemainingRounds { get; }
    bool Dispellable { get; }
    int StateId { get; }  // 这个 Provider 注册到 StateComponent 时用的 stateId

    // 必须的方法
    void OnApply(BattleContext ctx);   // 初始化:调 AddState
    void OnTick(BattleContext ctx);    // 每回合:自己减计时 + 自己做效果
    void OnRemove(BattleContext ctx);  // 退出:调 RemoveState
}
```

### 4.4 Provider 与 StateComponent 的协作流程

```
1. 创建 Provider 实例:new BuffPoison(target, caster, duration=3)
2. Provider.OnApply(ctx)
   → target.StateComponent.AddState(StateId.Poison, this)
3. 业务层"回合开始"事件:
   → buff.OnTick(ctx)
       → RemainingRounds--;
       → target.ResourceComponent.Modify(HP, -10);
       → if (RemainingRounds <= 0) buff.OnRemove(ctx)
4. Provider.OnRemove(ctx)
   → target.StateComponent.RemoveState(StateId.Poison, this)
```

### 4.5 Provider 的具体实现示例

```csharp
public class BuffPoison : IBuffProvider {
    public IGameEntity Target { get; }
    public object Caster { get; }
    public int RemainingRounds { get; private set; }
    public bool Dispellable => true;
    public int StateId => StatusId.Poison;

    public BuffPoison(IGameEntity target, object caster, int duration) {
        Target = target;
        Caster = caster;
        RemainingRounds = duration;
    }

    public void OnApply(BattleContext ctx) {
        // 注册到 StateComponent
        Target.GetComponent<StateComponent>().AddState(StateId, this);
    }

    public void OnTick(BattleContext ctx) {
        // 自己减计时
        RemainingRounds--;
        // 自己做效果(中毒扣血)
        var damage = ctx.PoisonDamageFormula();  // 业务公式
        Target.GetComponent<ResourceComponent>().Modify(ResourceId.HP, -damage);
    }

    public void OnRemove(BattleContext ctx) {
        // 自己决定退出,注销
        Target.GetComponent<StateComponent>().RemoveState(StateId, this);
    }
}
```

**关键观察**:`BuffPoison` **完全感知不到 StateComponent 内部**。StateComponent 也完全感知不到 `BuffPoison`。两边只通过 `(StateId, this)` 这一对复合键协作。

### 4.6 Provider 的常见变体

| Provider 类别 | 描述 | 典型 |
|--------------|------|------|
| **Buff Provider** | 持续 N 回合 / 秒 | 中毒 / 隐身 / 金刚护体 |
| **Skill Effect Provider** | 法术命中后附加的持续效果 | 灼烧 / 感电 / 冰冻 |
| **Death Flag Provider** | 死亡 / 倒地 / 鬼魂标记 | 倒地旗 / 死亡旗 |
| **Aura Effect Provider** | 范围(队伍 / 区域)的光环 | 法师光环 / 战吼 / 治疗结界 |
| **Equipment Passive Provider** | 装备触发的状态 | 装备灼烧光环 / 套装效果 |
| **World Effect Provider** | 地形 / 天气 / 时段 | 雨天加速 / 雪地减速 / 战场 buff |

**所有 Provider 的共同模式**:
- 自己持有 `RemainingRounds` / `RemainingSeconds`
- 自己实现 `OnApply / OnTick / OnRemove`
- 自己决定何时调 `AddState / RemoveState`
- 自己持有 owner / source / level / dispellable

---

## 五、Provider 池(可选优化)

### 5.1 Provider 也需要池化

跟组件一样,Provider 是引用类型,**业务量大时需要池化**。

### 5.2 Provider 池的接口

```csharp
public class BuffPool {
    private readonly Dictionary<Type, Stack<IBuffProvider>> m_Pools = new();

    public T Acquire<T>(IGameEntity target, object caster, int duration) where T : IBuffProvider {
        var stack = m_Pools.GetOrAdd(typeof(T), _ => new Stack<IBuffProvider>());
        if (stack.Count > 0) {
            // 从池里取
            var buff = (T)stack.Pop();
            buff.Reset(target, caster, duration);  // 复用:重置数据
            return buff;
        }
        return (T)Activator.CreateInstance(typeof(T), target, caster, duration);
    }

    public void Release(IBuffProvider buff) {
        // 归还到池
        buff.OnRemove(null);  // 注销
        var stack = m_Pools.GetOrAdd(buff.GetType(), _ => new Stack<IBuffProvider>());
        stack.Push(buff);
    }
}
```

### 5.3 Provider 池的注意事项

- Provider **必须**实现 `Reset(target, caster, duration)` 用于复用
- `OnRemove` 调用前要先注销(`StateComponent.RemoveState`)
- 池大小根据业务量调整(常见 50-200 个 Provider / 类型)

---

## 六、StateComponent 的常见反模式

### 6.1 反模式:一切皆状态

**症状**:
- 把 buff 效果 / 持续时间 / Tick 全塞进 `StateComponent`
- 把"技能命中灼烧"做成 state
- 把"装备光环"做成 state
- 结果:`StateComponent` 膨胀成万能类,失去"集合机制"的纯粹性

**正解**:
- `StateComponent` 只关心"我当下有什么 stateId"
- 每个 stateId 的来源都是 **Provider**
- Provider 自己持有 duration / tick / dispellable / source

### 6.2 反模式:持续时间塞进 StateComponent

**症状**:
- 在 StateComponent 内置 `_durations:Dictionary<int,int>`
- 试图在 `AddState` 时传 `duration` 参数
- 试图在 StateComponent 内置 "Tick()" 方法

**正解**:
- 持续时间是 **Provider 自己持有**的(`RemainingRounds` / `RemainingSeconds`)
- Provider 自己实现 `OnTick` / `OnRemove`
- StateComponent **完全不感知**持续时间

### 6.3 反模式:Provider 用 bool / 匿名对象

**症状**:
- `target.GetComponent<StateComponent>().AddState(PoisonId, null)` — 用 null 当 provider
- `target.GetComponent<StateComponent>().AddState(PoisonId, true)` — 用 bool 当 provider

**正解**:
- Provider **必须**是引用类型,**永远不要用 null / bool / int / string 当 provider**
- Provider 必须能回答"我施加了这个 stateId 多久 / 谁是我 owner"

### 6.4 反模式:Provider 不持有 owner / source

**症状**:
- Provider 实例化时不知道 target / caster
- Provider 内部 hardcode "对所有敌人施加"
- 持续时间到了但不知道"该对谁 RemoveState"

**正解**:
- Provider 构造时**必须**接收 target / caster
- Provider 自己持有 `Target / Caster` 引用
- Provider 自己决定何时调 `RemoveState(stateId, this)`

---

## 七、StateComponent 的常见边界

### 7.1 StateComponent vs 资源(ResourceComponent)

| 维度 | StateComponent | ResourceComponent |
|------|----------------|-------------------|
| **职责** | "在 / 不在"(集合成员关系) | "当前值 / 最大值"(可消耗)|
| **典型** | 中毒 / 隐身 / 倒地 | HP / MP / 愤怒 |
| **API 风格** | `CheckState / AddState / RemoveState` | `this[id] / Set / Modify / Cost` |
| **是否持有时间** | ❌ 不持有 | ❌ 不持有 |
| **是否持有数值** | ❌ 不持有 | ✅ 持有 Current / Max |

### 7.2 StateComponent vs 属性(NumericComponent)

| 维度 | StateComponent | NumericComponent |
|------|----------------|------------------|
| **职责** | "在 / 不在"(布尔)| 数值(可计算)|
| **典型** | 中毒 / 隐身 / 倒地 | 攻击 / 防御 / 速度 |
| **API 风格** | `CheckState / AddState / RemoveState` | `Set / Get / Modify(id, subType)` |
| **是否支持链式** | ❌ 不支持 | ✅ 链式公式(`Final` 自动算) |

### 7.3 StateComponent vs 倒计时(GameTimerManager)

| 维度 | StateComponent | GameTimerManager |
|------|----------------|------------------|
| **职责** | "在 / 不在"(持续多回合 / 多秒)| "倒计时 N 秒触发"(短倒计时) |
| **典型** | 中毒 3 回合 / 隐身 5 回合 | 30 秒指令窗口 / 技能冷却 |
| **API 风格** | `CheckState / AddState / RemoveState` | `AddTimer(interval, callback, count)` |
| **是否绑定实体** | ✅ 绑定实体 | ❌ 全局 / Context 级别 |
| **是否绝对时间** | ❌ 不持有时间 | ✅ 绝对时间 |

---

## 八、本章沉淀的设计模式

沉淀的不是具体某款 RPG 的状态数值,**而是 StateComponent 的设计骨架**:

1. **StateComponent = 底层基础设施**(不等于 buff)—— 只关心"我当下有什么 stateId"。
2. **Provider = 状态来源 / 主人** —— 自己持有 duration / tick / dispellable / source / level。
3. **buff 是 Provider 的一种** —— 不是 state 本身。
4. **(stateId, provider) 复合键 + 引用计数** —— 区分施加者,自然处理多 buff 叠加 + 各自取消。
5. **Provider 自己决定何时 AddState / RemoveState** —— 持续时间到 / 被驱散 / owner 死亡都是 Provider 的事。
6. **业务层"回合开始"事件遍历所有活跃 Provider** —— 每个 Provider 自己减 / 检持续时间,不遍历 StateComponent。
7. **StateComponent 保持纯粹** —— 不持有时间、不持有效果、不持有等级。

具体数值(中毒 3 回合、隐身 5 回合)跟着业务代码走,**不**沉淀到这里。

---

## 参考来源

沉淀自 2026-06 HoweFramework 框架分析 + 2026-06 用户架构澄清(StateComponent 是底层基础设施,buff 是 Provider 的一种)。**不指向任何具体游戏的数值表**,只沉淀通用设计骨架。