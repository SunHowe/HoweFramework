# 02 · 战斗循环与回合机制

> 本章沉淀回合制 RPG 的战斗循环设计 —— 不含具体游戏数值,只沉淀"为什么这样设计"。

---

## 一、战斗循环的 FSM(有限状态机)

### 1.1 通用战斗阶段

```
战斗开始
   ↓
[初始化] — 创建实体 / 加载数据 / 布阵
   ↓
[回合开始] — 回合数 +1 / 状态衰减 / 持续效果触发
   ↓
[排序] — 按速度属性排序所有存活单位
   ↓
[收集指令] — 半自动模式:玩家 N 秒下指令;全自动:跳过
   ↓
[执行指令] — 按顺序处理每个单位的指令
   ↓
[回合结束] — 胜负判定 / 倒地判定 / 资源清理
   ↓
[继续 / 结束]
   ├─ 没结束 → 回合开始
   └─ 结束   → 胜负结算 / 经验 / 掉落 / 退出
```

### 1.2 用框架 `FsmMachine` 实现

| FSM 状态 | 何时进入 | 进入时做什么 |
|---------|----------|--------------|
| `Initializing` | 战斗开始 | 创建实体、加载数据、布阵 |
| `RoundStart` | 初始化完成 / 上回合结束 | 回合数 +1、状态 Tick、播放"回合 N"提示 |
| `Sorting` | 回合开始完成 | 按速度排序存活单位 |
| `WaitingCommand` | 排序完成(半自动)| 开 N 秒指令窗口、收指令 |
| `Executing` | 指令收完(或超时)| 按顺序执行每个单位指令、播动画 |
| `RoundEnd` | 执行完毕 | 胜负判定、倒地判定 |
| `BattleOver` | 胜负已定 | 结算、清理、退出 |

### 1.3 关键约束

- **不要把"应用级流程"放进 Fsm** —— 主城 ↔ 战斗的切换走 `ProcedureModule`。
- **不要把"单个实体的状态"放进 Fsm** —— 中毒 / 封印 = `StateComponent`(底层"集合机制)+ **Provider**(典型 = buff,自己持有 duration)。详见 [`05-state-component.md`](05-state-component.md) 和 [`06-buff-system.md`](06-buff-system.md)。
- **Fsm 不持久化** —— `Dispose` 必须归还到引用池。

---

## 二、半自动指令窗口

### 2.1 关键设计决策

| 决策点 | 常见选择 | 设计意图 |
|--------|---------|----------|
| 指令窗口时长 | 15-60 秒 | 短 = 紧迫;长 = 宽松 |
| 超时处理 | 默认物理攻击 | 有意义但不最优 |
| 指令可改 | 锁定(下完不可改)| 防"看了动画又改" |
| 指令面板 | 多按钮(技能 / 物品 / 召唤 / 保护 / 防御 / 逃跑 / 自动)| 决策空间大 |
| 半队 vs 全队 | 全队一起下(同时)| 简化流程 |

### 2.2 计时必须用绝对时间

```csharp
// ✅ 正确:绝对时间,玩家暂停游戏也能计时
GameTimerManager.Instance.AddTimer(1f, OnSecondTick, null, 30);

// ❌ 错误:帧累计,Time.timeScale=0 就卡死
void Update() {
    m_Remaining -= Time.deltaTime;  // ← 这里 Time.timeScale=0 时不计时
}
```

### 2.3 超时自动判定的"默认操作"模式

| 默认操作 | 适用场景 | 体验 |
|---------|----------|------|
| **物理攻击**(无目标时随机) | 梦幻西游、大话西游、问道 | 有意义(还能打)|
| 上次指令 | 适合重玩价值低的休闲游戏 | 减少学习成本 |
| 防御 | 适合"输也能接受"的休闲游戏 | 降低风险 |
| 自动全队 | 纯剧情 RPG | 解放玩家 |

**设计原则**:**默认操作要"有意义但不最优"** —— 让玩家觉得"反正输了也比这好"。

---

## 三、出手顺序规则

### 3.1 标准规则(大多数回合制 RPG)

```
出手顺序 = 按速度属性降序排列
同速时:
    优先规则 A(常见): 玩家方先于敌方
    优先规则 B(常见): 同阵营按站位(1→5 号位)
    优先规则 C(罕见): 随机
```

### 3.2 排序实现

```csharp
// 排序时新建一个 List,不修改原 AllUnits(AllUnits 用于胜负判定)
var sorted = AllUnits
    .Where(u => u.GetComponent<StateComponent>().CheckState(StateId.Dying) == false)
    .Where(u => u.GetComponent<StateComponent>().CheckState(StateId.Dead) == false)
    .OrderByDescending(u => u.GetComponent<NumericComponent>()
        .Get(NumericId.Speed, NumericSubType.Final))
    .ThenBy(u =>阵营优先级(玩家优先还是敌方优先))
    .ThenBy(u => u.GetComponent<TransformComponent>().Position.X)  // 站位次之
    .ToList();
```

### 3.3 速度被改时要重排

- buff 加速度 → 在该 buff 持续期间持续排序。
- 阵法加成 → 进入战斗时一次排序,持续生效。
- 倒地 / 死亡 → 从排序中剔除。

### 3.4 ATB / 半即时模式(变体)

ATB(Active Time Battle) / 半即时模式:
- 每个单位有独立的"行动槽",累积到阈值就出手。
- 没有"回合"概念,改成"行动槽满了就行动"。
- 玩家可以选择"等待"(等槽快满了再选技能)。
- 玩家反应速度直接影响战斗节奏。

---

## 四、战斗胜负判定

### 4.1 胜负判定

| 类型 | 判负条件 |
|------|----------|
| **一方所有单位 HP=0** | 立即判负 |
| 一方全部逃跑 | 立即判负 |
| 达到回合上限 | 平局(常见 30-150 回合,具体数值跟项目节奏相关)|
| 特定条件 | 触发剧情条件(比如 boss 战 HP=10% 自动触发剧情)|

### 4.2 胜负判定的位置

胜负判定应该放在**执行阶段末尾**(每个单位行动后 / 整回合执行完后),**不**应该放在执行过程中(否则会打断动画)。

### 4.3 判负的"判定链"

```
执行指令 → 播放动画 → 判定(玩家方全倒?敌方方全倒?)→ 是 → 战斗结束
                                                          → 否 → 继续
```

---

## 五、持续效果(Tick)的时机

### 5.1 三种常见时机

| 时机 | 触发内容 | 设计意图 |
|------|---------|----------|
| **回合开始** | 持续 N 回合状态(中毒扣血 / 封印倒数)| 标准回合制 |
| **回合结束** | 自身持续效果 | 部分 RPG |
| **每 N 秒** | buff 周期 tick(中毒、灼烧、回复) | 实时性高 |

### 5.2 实现位置

> 详见 [`05-state-component.md`](05-state-component.md) 和 [`06-buff-system.md`](06-buff-system.md) 的完整 Provider 模式。

- 持续回合数 → 在 **Provider**(典型 = `Buff`)上维护,不在 StateComponent 上。
- Tick 触发 → 业务层"回合开始"事件遍历**所有活跃 Provider**,每个 Provider 自己减计时 + 自己做效果。
- 每秒 tick → Provider 自己用 `GameTimerManager.AddTimer`(短倒计时)|StateComponent 不持有时间。

---

## 六、行动条 UI

### 6.1 行动条的两种设计

| 类型 | 描述 | 典型 |
|------|------|------|
| **行动槽式** | 每个单位一个槽,累积到阈值显示"准备就绪" | FF4-9 / 轨迹系列 |
| **行动条式** | 整条带,标记下一个行动者 | 梦幻西游、问道 |

### 6.2 行动条的实现要点

- 用 `GameUpdateManager.RegisterUpdate(target, callback)` 注册帧更新。
- 不放在 Fsm 里(不应该让 FSM 等行动条动画)。
- 行动条是 **UI**,不是业务逻辑 —— UI 订阅业务事件。

### 6.3 行动条的"决策含义"

行动条不只是装饰,它告诉玩家:
- 下一个是谁行动(战术预判)
- 我能不能在这个回合出招(行动槽快满了)
- 我能不能在对手出招前反击(取决于行动顺序)

---

## 七、应用级流程

### 7.1 主城 ↔ 战斗的切换

```
启动
  ↓
[Procedure] Splash(开场动画)
  ↓
[Procedure] LoadDataTable(加载配置表)
  ↓
[Procedure] Login(登录)
  ↓
[Procedure] MainCity(主城)
  ↓ (遇敌)
[Procedure] Battle(进入战斗)
  ↓ (战斗结束)
[Procedure] MainCity(回到主城)
```

### 7.2 关键约束

- **粗粒度流程 → Procedure**;**细粒度状态 → Fsm**。
- Battle Procedure 内创建 `BattleContext`;战斗结束 `Dispose` Context。
- **一场一战** —— 每场战斗新建 Context,不复用。

### 7.3 切换的"清理"要点

- 销毁所有战斗实体(不销毁召唤兽本体,只销毁召唤兽的"参战副本")
- 关闭战斗 UI(`UIModule.CloseForm`)
- Dispose BattleContext(包括 FsmMachine、所有 Timer、所有订阅)
- 回到主城后,主城 UI 应该是"无战斗残留"状态

---

## 八、怪物 AI(行为树)

### 8.1 AI 的三步决策

```
1. 选目标(谁打谁)
2. 选技能(打谁用什么技能)
3. 释放(执行技能)
```

### 8.2 行为树 vs if-else

| 决策树深度 | 决策分支数 | 推荐方案 |
|-----------|-----------|---------|
| 1-2 | 1-3 | `if-else` |
| 3+ | 4+ | `BehaviorTree`(JSON 配置)|

### 8.3 行为树典型结构(回合制 RPG)

```
Root (Selector: 尝试子节点直到一个 Success)
  ├─ Sequence: 残血逃跑(HP<30% → 加血 → 防御)
  │      ├─ Condition: HP < 30%
  │      ├─ Action: 加血技能
  │      └─ Action: 防御
  ├─ Sequence: 群体技能冷却中
  │      ├─ Condition: 群体技能冷却中
  │      ├─ Action: 单体技能
  │      └─ Action: 普攻
  └─ Sequence: 默认
         ├─ Action: 选血最少目标
         └─ Action: 选单体技能
```

### 8.4 `userData` 模式

```csharp
// BattleContext 是 userData,所有 Action / Condition 节点都通过 userData 访问
public class BattleContext {
    public IGameEntity Self;
    public IGameEntity Target;
    public BattleManager Battle;
    // ... 其他战斗内共享数据
}

// Action 节点
public class SelectLowestHpTargetAction : BehaviorActionNodeBase {
    protected override BehaviorResult DoExecute(object userData) {
        var ctx = (BattleContext)userData;
        // 选血最少的目标
        ctx.Target = ctx.Battle.AllUnits
            .Where(e => IsEnemy(ctx.Self, e))
            .Where(e => !e.GetComponent<StateComponent>().CheckState(StateId.Dead))
            .OrderBy(e => e.GetComponent<ResourceComponent>().Get(ResourceId.HP))
            .First();
        return BehaviorResult.Success;
    }
}
```

---

## 九、回合制战斗的关键设计原则

### 原则 1:**链式反应优于手工同步**

HP 上限绑基础属性 → 装备变 → HP 上限自动重算 → 满血时自动填满。**单一事实来源**(single source of truth)。

### 原则 2:**绝对时间计时,优于帧累计**

`Time.timeScale=0` 玩家暂停游戏,计时也不能停。

### 原则 3:**粗粒度流程 → Procedure**;**细粒度状态 → Fsm**。

### 原则 4:**AI 的 userData 是 BattleContext**

行为树 Action 节点通过 `userData` 访问战斗上下文,**不**自己"找"目标。

### 原则 5:**胜负判定在执行阶段末尾**

不要打断动画。

### 原则 6:**一场一战**

每场战斗一个 Context,结束统一清理。

### 原则 7:**默认操作有意义但不最优**

让玩家觉得"反正输了也比这好"。

---

## 十、本章沉淀的设计模式

沉淀的不是具体某款 RPG 的战斗循环数值,**而是战斗循环的设计骨架**:

1. **战斗阶段 FSM** —— 初始化 → 回合开始 → 排序 → 收指令 → 执行 → 回合结束 → 结束。
2. **半自动指令窗口** —— 计时器 + 锁定 + 默认操作。
3. **出手顺序规则** —— 按速度 + 同速 tiebreak。
4. **胜负判定位置** —— 执行末尾,不打断动画。
5. **持续效果触发时机** —— 回合开始 / 回合结束 / 每 N 秒。
6. **行动条 UI** —— UI 订阅业务事件,不污染业务逻辑。
7. **应用级流程分离** —— Procedure(粗粒度)vs Fsm(细粒度)。
8. **AI 行为树** —— 三步(选目标 → 选技能 → 释放)+ userData 模式。

具体数值(30s 指令窗口 / 150 回合上限 / 5 人队 + 1 宠 vs 14 怪)是梦幻西游的数值,**不**沉淀到这里。

---

## 参考来源

沉淀自 2026-06 HoweFramework 框架分析研究,综合多款经典回合制 RPG / SRPG / MMORPG 的战斗循环设计。**不指向任何具体游戏的数值表**,只沉淀通用设计骨架。