# 05 · 状态系统

> 本章沉淀回合制 RPG 的状态系统设计 —— 不含具体游戏数值,只沉淀"为什么这样设计"。

---

## 一、状态系统的本质

### 1.1 状态 = "持续 N 回合的可标记效果"

- 增益 buff / 减益 debuff / 控制 / 特殊,都是"状态"。
- "在"或"不在",**不**是 bool(那是字段),**是** stateId + provider 引用计数。
- 状态**自带持续时间**,但**不知道回合**(由业务层维护)。

### 1.2 状态系统的设计目标

1. **可分类** —— 4 类状态清晰区分。
2. **可叠加** —— 多 provider 同时施加同一状态,引用计数处理。
3. **可移除** —— 最后 provider Remove 才真正消失。
4. **可订阅** —— 状态切换时通知 UI / AI / 录像。
5. **可序列化** —— 存档 / 录像需要持久化。

### 1.3 用框架 `StateComponent` 实现

`StateComponent` 是状态系统的标准实现:
- 状态 = `int stateId`(同状态不同等级用 provider 区分)
- 多 provider → 引用计数
- `AddState / RemoveState(stateId, provider)` —— **始终带 provider**
- `Subscribe(stateId, Action<bool>, notifyImmediately)` —— 监听切换

---

## 二、4 类状态分类

### 2.1 标准分类

| 类别 | 描述 | 典型状态 | 持续 |
|------|------|---------|------|
| **增益 buff** | 对己方有利的状态 | 隐身、慧根、强力、金刚护体 | 1-5 回合 |
| **减益 debuff** | 对己方不利但非控制的状态 | 中毒、遗忘、冰冻、虚弱、灼烧 | 1-5 回合 |
| **控制** | 限制行动 | 混乱、睡眠、封印、定身、嘲讽 | 1-5 回合 |
| **特殊** | 不属于前 3 类的特殊标记 | 倒地、死亡、鬼魂、变身 | 战斗内 |

### 2.2 为什么是这 4 类,不是 3 类或 5 类?

**设计意图**:这 4 类是"决策维度 + 解除方式"的最小完备集合。

| 类别 | 决策维度 | 解除方式 |
|------|---------|---------|
| **增益** | 主动使用 | 时间 / 自驱(主动 buff)|
| **减益** | 治疗 / 净化 | 时间 / 净化 / 抗性 |
| **控制** | 净化 / 抗封 | 时间 / 净化 / 抗封 |
| **特殊** | 不参与常规决策 | 复活 / 特定条件 |

**少了"特殊"** → 倒地 / 死亡 / 鬼魂混在普通状态里,无法特殊处理。
**少了"控制"** → 跟减益混在一起,无法做"抗封"机制。

### 2.3 常见变体

| 变体 | 描述 |
|------|------|
| **5 类** | 增益 / 减益 / 控制 / 特殊 / 被动(永久 / 触发) |
| **3 类** | 增益 / 减益 / 控制(合并"特殊") |
| **8 类** | 按 RPG 元素(金 / 木 / 水 / 火 / 土 / 风 / 雷 / 暗)分类 |

---

## 三、状态数据结构

### 3.1 状态定义(`StatusDefinition`)

```csharp
public class StatusDefinition {
    public int StatusId;           // 唯一 id
    public string Name;           // 名称
    public StatusCategory Category;// 4 类之一
    public int DurationRounds;    // 默认持续回合数(可被技能覆盖)
    public string Icon;           // 图标路径
    public string Description;    // 描述
    public Action<OnTickEventArgs> OnTick;  // 每回合 Tick 做什么(中毒扣血 / 灼烧 / 治疗)
    public bool Dispellable;      // 是否可被净化
    public bool Stackable;        // 是否可叠加(同状态不同等级)
}
```

### 3.2 状态实例(`StatusInstance`)

```csharp
public class StatusInstance {
    public int StatusId;
    public object Provider;       // 谁施加的(buff 来源)
    public int RemainingRounds;   // 剩余回合数
    public StatusDefinition Definition;
    public IGameEntity Target;    // 施加在谁身上
}
```

### 3.3 状态生命周期

```
施加状态:
    ApplyStatus(statusId, target, provider, duration, ctx)
        → StateComponent.AddState(statusId, provider)
        → 创建 StatusInstance,记录 provider + 剩余回合
        → 触发 OnApplied 事件(UI 显示状态图标)

每回合 Tick(在 RoundStart / RoundEnd):
    TickAll(ctx)
        → 遍历所有 StatusInstance
        → inst.RemainingRounds--
        → 触发 OnTick(中毒扣血 / 灼烧 / 治疗)
        → 如果 RemainingRounds <= 0:
            → StateComponent.RemoveState(statusId, provider)
            → 触发 OnRemoved 事件(UI 移除状态图标)
```

### 3.4 关键约束

- `StateComponent` 不知道回合 → 持续回合数由 `StatusEffectComponent` 外置。
- 业务层在 `RoundStart` / `RoundEnd` 状态触发 `TickAll`。
- 永远带 provider 调 `AddState / RemoveState`,否则会误删。

---

## 四、引用计数语义(关键设计)

### 4.1 标准引用计数

```csharp
// 两个 buff 同时施加"中毒"
target.AddState(StatusId.Poison, buffA);  // 引用计数 = 1
target.AddState(StatusId.Poison, buffB);  // 引用计数 = 2

// buffA 被驱散
target.RemoveState(StatusId.Poison, buffA);  // 引用计数 = 1
// 中毒仍然在(target 仍有 buffB 的中毒)

// buffB 被驱散
target.RemoveState(StatusId.Poison, buffB);  // 引用计数 = 0
// 中毒真正消失
```

### 4.2 为什么需要引用计数

**反例**(用 `Dictionary<int, bool>`):
- 丢失"是谁施加的"信息。
- 多个 buff 同时施加同一状态时,只能保留一个。
- buff 驱散时无法精确移除。

**正解**(用 `StateComponent`):
- 天然处理"多 buff 叠加 + 各自取消"。
- 不同等级中毒(普通 / 高级)用同一 `stateId` + 不同 provider,自然合并。

### 4.3 引用计数的常见误用

| 误用 | 问题 |
|------|------|
| 不带 provider 调 `RemoveState` | 误删其他 provider 加的状态 |
| 把"中毒"用 `bool` 表示 | 不同等级无法区分 |
| 用 `Dictionary<int, int>` 存"buff 名 → 持续回合" | 丢失"施加者是谁"信息 |

### 4.4 引用计数的边界

- 同一 provider 多次施加同一状态 → 引用计数只 +1(避免误增)
- 不同 provider 施加同一状态 → 引用计数累加
- `RemoveState` 不存在的 provider → 无效果

---

## 五、状态的"持续回合"设计

### 5.1 三种常见模式

| 模式 | 描述 | 典型 |
|------|------|------|
| **按回合数** | 状态持续 N 个回合 | 大多数 RPG |
| **按时间** | 状态持续 N 秒 | 实时游戏 |
| **按触发** | 状态持续到特定事件 | 复杂状态(变身、附身) |

### 5.2 回合数 vs 时间

| 维度 | 回合数 | 时间 |
|------|--------|------|
| 准确 | 受速度影响(快的人回合多)| 准确 |
| 体验 | "我用了 3 回合还能再 X" | "5 秒后再做 Y" |
| 实现 | 业务层 Tick | `GameTimerManager.AddTimer` |

### 5.3 持续回合的常见误用

| 误用 | 问题 |
|------|------|
| **持续回合数塞进 `StateComponent`** | `StateComponent` 不知道回合,会失败 |
| **不递减,只检查时间戳** | 不会自动失效,会"一直存在"|
| **每帧递减**(用 Update) | 玩家暂停游戏状态错乱 |
| **跨回合状态没保存回合数** | 战斗结束 → 重置 → 状态失效 |

### 5.4 推荐实现

```csharp
public sealed class StatusEffectComponent : GameComponentBase {
    private readonly Dictionary<int, StatusInstanceList> m_Active = new();

    // 施加
    public void ApplyStatus(int statusId, IGameEntity target, object provider, int duration, BattleContext ctx) {
        var stateComp = target.GetComponent<StateComponent>();
        stateComp.AddState(statusId, provider);

        if (!m_Active.TryGetValue(target.EntityId, out var list)) {
            list = new StatusInstanceList { Owner = target };
            m_Active[target.EntityId] = list;
        }
        list.Add(new StatusInstance {
            StatusId = statusId,
            Provider = provider,
            RemainingRounds = duration,
            Definition = ctx.StatusTable.Get(statusId),
        });
    }

    // Tick(每回合开始 / 结束)
    public void TickAll(BattleContext ctx) {
        foreach (var (entityId, list) in m_Active) {
            for (int i = list.Count - 1; i >= 0; i--) {
                var inst = list[i];
                inst.RemainingRounds--;
                inst.Definition.OnTick?.Invoke(list.Owner, inst, ctx);

                if (inst.RemainingRounds <= 0) {
                    list.Owner.GetComponent<StateComponent>()
                        .RemoveState(inst.StatusId, inst.Provider);
                    list.RemoveAt(i);
                }
            }
        }
    }
}
```

---

## 六、特殊状态(倒地 / 死亡 / 鬼魂)

### 6.1 倒地 vs 死亡 vs 鬼魂

| 状态 | 触发 | 行为 | 复活 |
|------|------|------|------|
| **倒地** | 玩家 HP=0 | 不能行动,本场可救起 | 复活技能 / 复活药品 |
| **死亡** | NPC / 召唤兽 HP=0 | 当回合不可再出战 | 一般不可复活 |
| **鬼魂** | 鬼魂系单位 HP=0 | 可复活(次数限制)| 战斗结束前复活不算死亡 |

### 6.2 特殊状态的实现要点

- 倒地 = `StateComponent.AddState(StatusId.Down, "self")`
- 死亡 = `StateComponent.AddState(StatusId.Dead, "self")`
- 鬼魂 = `StateComponent.AddState(StatusId.Ghost, "self")`(可被特定技能驱散)

### 6.3 特殊状态的 UI 表现

| 状态 | UI |
|------|-----|
| **倒地** | 单位灰显 + "倒地"字样 |
| **死亡** | 单位消失 + 留下尸体 |
| **鬼魂** | 单位半透明 + 飘忽 |

### 6.4 特殊状态与战斗胜负的关系

- 一方所有玩家**倒地** → 玩家方判负
- 一方所有 NPC **死亡** → 敌方方判负
- 鬼魂系的 NPC 死亡后,还能复活 → 仍然算存活

---

## 七、状态系统的常见模式

### 7.1 同类状态覆盖

**常见规则**(梦幻西游 / 大话西游):
- 同类状态多次施加 → 以**最后一次为准**(覆盖)
- 例:中毒 3 回合 + 中毒 5 回合 → 中毒 5 回合(覆盖,而不是累加)

**实现**:`StatusEffectComponent` 在 `ApplyStatus` 时检查同状态是否已存在,如果有就**覆盖**持续时间。

### 7.2 抗性机制

**常见模式**(高级神迹抗封):
- 某些状态有"抗性"(基础 80-95%)
- 命中时随机判定是否抵抗
- 抗性高的单位更难被施加

**实现**:在 `ApplyStatus` 时调用抗性检查,失败则不施加。

### 7.3 净化机制

**常见模式**(净化 / 解毒):
- 净化技能可以一次性移除所有减益
- 部分净化(解毒)只移除特定类型

**实现**:`RemoveAllStates(target, category)` 实现"净化类别 X"。

### 7.4 状态叠加 vs 状态共存

| 模式 | 描述 |
|------|------|
| **叠加** | 同一状态可叠加(中毒 Lv1 + 中毒 Lv2 = 中毒 Lv2 高伤害) |
| **共存** | 不同状态可共存(中毒 + 灼烧 = 各自 Tick) |
| **覆盖** | 同类状态覆盖(中毒 3 回合 + 中毒 5 回合 = 中毒 5 回合) |

### 7.5 状态图标 UI

| 状态 | UI 表现 |
|------|---------|
| 增益 | 头像右侧绿色图标 + 倒计时数字 |
| 减益 | 头像右侧红色图标 + 倒计时数字 |
| 控制 | 头像右侧灰色图标 + 倒计时数字 |
| 特殊 | 头像底部"倒地 / 死亡 / 鬼魂"字样 |

---

## 八、状态系统的常见边界

### 8.1 状态无敌

"无敌"状态 = 免疫所有伤害(但持续回合数有限)。

**实现**:`OnTick` 不做 + 在伤害计算时检查 `StateComponent.CheckState(StateId.Invincible)`。

### 8.2 状态叠加 vs 互斥

| 关系 | 描述 | 例子 |
|------|------|------|
| **叠加** | 可同时存在 | 隐身 + 中毒 |
| **互斥** | 同类只能存在一个 | 隐身 vs 现身 |
| **升级** | 同状态不同等级 | 中毒 Lv1 + 中毒 Lv2 → 中毒 Lv2 |

### 8.3 状态 vs 资源

| 状态 | 资源 |
|------|------|
| 在 / 不在(持续 N 回合)| 当前值 / 最大值(可消耗)|
| 引用计数处理 | `Modify / Cost / SetMax` |
| `StateComponent` | `ResourceComponent` |

**常见误用**:把"中毒掉血"做成资源(`HP -10 / 回合`) → 应该用 `StateComponent` + `OnTick`。

### 8.4 状态 vs 属性

| 状态 | 属性 |
|------|------|
| 在 / 不在(buff / debuff)| 数值(可计算)|
| 持续 N 回合 | 永久(直到改)|
| `StateComponent` | `NumericComponent` |

**常见误用**:把"中毒"做成属性(`HP -10%`) → 应该用 `StateComponent`。

---

## 九、本章沉淀的设计模式

沉淀的不是具体某款 RPG 的状态数值,**而是状态系统的设计骨架**:

1. **4 类状态分类** —— 增益 / 减益 / 控制 / 特殊(决策维度 + 解除方式最小完备)。
2. **状态定义 + 实例** —— `StatusDefinition`(静态)+ `StatusInstance`(运行时)。
3. **状态生命周期** —— ApplyStatus → TickAll → RemoveState。
4. **引用计数语义** —— 多 provider 叠加 + 各自取消,自然合并。
5. **持续回合数外置** —— `StateComponent` 不知道回合,由 `StatusEffectComponent` 维护。
6. **同类状态覆盖** —— 同状态多次施加,以最后一次为准。
7. **抗性 / 净化机制** —— 施加 / 移除的扩展点。
8. **特殊状态(倒地 / 死亡 / 鬼魂)** —— 不参与常规决策,但需要清晰区分。

具体数值(中毒 3 回合、隐身 5 回合、封印命中率 55%)是梦幻西游的数值,**不**沉淀到这里。

---

## 参考来源

沉淀自 2026-06 HoweFramework 框架分析研究,综合多款经典回合制 RPG 的状态系统设计。**不指向任何具体游戏的数值表**,只沉淀通用设计骨架。