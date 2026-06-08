# 决策模式(Decision Guide)

> **遇到候选方案选哪个时翻这页**。
> 11 个最常碰到的"二选一 / 多选一"决策树 + 答案。

---

## 决策 1:HP 用 `ResourceComponent` 还是自写 `Dictionary<string, int>`?

**答**:永远用 `ResourceComponent`。除非你在写**纯 Demo 30 行**(那确实不需要)。

**理由**:
- `ResourceComponent` 自带上限、消耗安全(`Cost` 检查)、订阅、自动填满。
- 自写 Dictionary 要自己处理:超限截断、不足保护、订阅机制、自动填满 —— 每一个都是坑。
- 自写版本没有链式反应(装备变 → HP 上限变 → 自动填满)。

**例外**:纯教学 / Demo,业务规模 < 100 行。

---

## 决策 2:buff 用 `StateComponent` 还是自写 `Dictionary<int, int>`(持续回合)?

**答**:`StateComponent` + **自己外置持续回合表**(`StatusEffectComponent`)。

**理由**:
- `StateComponent` 本身**不知道回合**,它的职责是"在不在"。持续回合需要业务层维护。
- 多个 provider 同时施加同一状态(两个 buff 都加中毒),`StateComponent` 天然引用计数处理 —— 自己手写 Dictionary 会丢"施加者是谁"信息,没法分别移除。
- 不同等级中毒(普通 / 高级)用同一 `stateId` + 不同 provider,自然合并。

**典型代码**(伪代码,具体见 `extension-points.md`):
```
public class StatusEffectComponent {
    private Dictionary<int, StatusInstanceList> m_Active;
    public void TickAll(BattleContext ctx) {
        // 每回合减少持续回合 → 到 0 调 StateComponent.RemoveState
    }
}
```

---

## 决策 3:战斗循环用 `FsmMachine` 还是 `enum + if-else`?

**答**:超过 3 个状态用 `FsmMachine`。

**理由**:
- 状态切换有回调(`OnRoundStartEnter` 等),未来加 30s 计时只需在 `WaitingCommand` 状态里挂 `GameTimerManager.AddTimer`。
- 状态可视化(`fsm.CurrentState`)。
- 可调试(打断点 / 日志)。

**如果就 2-3 个状态**且没有回调需求,`if-else` 可以接受。

---

## 决策 4:应用流程用 `FsmMachine` 还是 `ProcedureModule`?

**答**:**应用流程用 `Procedure`;单个战斗内阶段用 Fsm**。

**理由**:
- `ProcedureModule` 是应用级(启动→登录→主城→战斗→主城),**重启战斗时所有 Procedure 状态保留**,会污染。
- `FsmMachine` 是单个上下文内的细粒度状态(战斗内阶段),**切一次 Procedure 太重**,频繁切换会丢上下文。

**简记**:**粗粒度流程 → Procedure**;**细粒度状态 → Fsm**。

---

## 决策 5:跨实体引用用裸 `IGameEntity` 还是 `GameEntityRef`?

**答**:**持有一个会死的目标超过一个事件周期 → 必须用 `GameEntityRef`**。

**理由**:
- AI 行为树选了一个目标,5 个回合后还在用同一个引用。
- 目标在第 3 回合被销毁 → 裸 `IGameEntity` → `NullReference`。
- `GameEntityRef.GameEntity` 自动变 `null`,不会崩。

**例外**:持有一个**确定不会被销毁**的引用(全局静态服务 / Singleton),裸引用 OK。

---

## 决策 6:计时用 `GameTimerManager` 还是 `MonoBehaviour.Update` 帧累计?

**答**:**任何"现实时间"含义的倒计时 / 冷却 / 持续时间 → `GameTimerManager.AddTimer`**。

**理由**:
- `AddTimer` 走绝对时间,`Time.timeScale=0` 不会卡死。
- 按 `count` 自动停止,无需手动管理。
- 30 秒倒计时:用 `GameTimerManager.AddTimer(1f, OnTick, null, 30)` 按 30 次回调。

**如果用 `Update` 帧累计**:玩家暂停游戏 → 计时也停。**这是 bug**。

---

## 决策 7:数值变化通知用 `EventModule` 还是 `NumericComponent.Subscribe`?

**答**:**特定 `numericId + subType` 数值变化 → `NumericComponent.Subscribe`;全局业务事件(战斗开始 / 回合切换 / 胜负)→ `EventModule`**。

**理由**:
- `NumericComponent.Subscribe(id, subType, handler)` 监听**特定数值 id + subType** 的变化(如"速度"基础值变了),粒度细、性能好、可读。
- `EventModule.Subscribe(eventId, handler)` 监听**全局事件**,粒度粗(适合业务层事件)。

**典型错误**:用 `EventModule` 广播"HP 变了" —— 所有实体收到通知,自己再过滤。性能差、可读性差。**应该用 `NumericComponent.Subscribe` 监听具体 HP id**。

---

## 决策 8:技能公式用 `if-else` 还是 `ExpressionManager`?

**答**:**有伤害公式 / 经验公式 / 升级公式 → 用 `ExpressionManager` + 可配置字符串**。

**理由**:
- 公式字符串可配置(策划改 `.json` / `.asset` 不用改代码)。
- 可注册任意 token(`$atk` / `$def` / `$lv` / `$critRate`),未来加五行相克只需多注册一个 token。
- `Evaluate(expression, userData) → long` 直接返回结果。

**示例**:
```
max(1, $atk - $def) * (1 + $critRate) * (0.95 + $randomNext() * 0.1)
```

**例外**:**极简 Demo / 一两个固定公式 → `if-else` 也行**。

---

## 决策 9:状态变化订阅用 `StateComponent.Subscribe` 还是 `EventModule.Fire`?

**答**:`StateComponent.Subscribe`(局部 + 引用计数语义)优先。

**理由**:
- `StateComponent.Subscribe(stateId, Action<bool>, notifyImmediately)` —— 监听"这个实体进入 / 离开了某个状态"。
- `EventModule.Fire(eventId, ...)` —— 全局广播,粒度粗。

**典型用法**:监听某玩家进入"中毒"状态 → 触发 UI 中毒图标出现。

---

## 决策 10:UI 指令输入用 FairyGUI 直接绑定还是走 `FairyGUIFormLogicBase`?

**答**:**永远走 `FairyGUIFormLogicBase`**。

**理由**:
- 逻辑 ↔ 资源分离(`.fui` 资产 + FormLogic C# 类)。
- FormLogic 自带生命周期(`OnInit / OnOpening / OnClose` 等)。
- `OpenForm(formId)` 由 `UIModule` 管,统一管理栈。

**例外**:极简 Demo。

---

## 决策 11:新游戏类型(非 RPG)需要"HP / MP"概念吗?

**答**:**任何"会消耗 + 有上限"的资源都映射到 `ResourceComponent`**。

| 游戏类型 | 资源(用 ResourceComponent) |
|---------|-----------------------------|
| FPS | 弹药、手雷数、医疗包 |
| MOBA | 法力、生命、能量 |
| 卡牌 | 手牌数、法力水晶、墓地 |
| 模拟经营 | 金币、行动点、体力、库存 |
| 益智 | 步数、道具数、剩余时间(若以"可消耗"角度) |

`ResourceComponent` + `BindNumericMax` 是通用解,不是只为 RPG 设计的。

---

## 决策 12:AI 用 `if-else` 还是 `BehaviorTree`?

**答**:**超过 3 个分支 / 多状态机嵌套 / 需要策划配置 → `BehaviorTree`**。

**理由**:
- `BehaviorTree` 节点可视化(JSON 配置),策划可改。
- `userData` 传上下文,Action 节点可访问"自己是谁 / 目标是谁"。
- `BehaviorRoot` 实现 `IBehaviorContext`,黑板数据可在节点间共享。

**例外**:AI 决策就是 `if (HP<30%) HealSelf` 一条 → 用 `if-else`。

---

## 决策 13:跨模块通信用 `EventModule` 还是直接 `GetComponent`?

**答**:**优先 `EventModule` + 订阅;`GetComponent` 是耦合,要克制**。

**理由**:
- `GetComponent` 在 EC 容器里"组件之间互访"是设计允许的(就是耦合)。
- **能用事件解耦就别用 GetComponent**。

**反例**(不要写):
```
// 反模式:每个组件自己 GetComponent 找别的组件
public class SkillExecutor : GameComponentBase {
    void Cast(...) {
        var ui = Entity.GetComponent<UIComponent>();  // 耦合!
        ui.ShowAnimation();
    }
}
```

**正例**(写):
```
// 正例:用事件,UI 自己订阅
EventModule.Instance.Fire(BattleEventId.SkillCast, new SkillCastEventArgs { ... });
```

---

## 决策总结(可打印)

```
┌──────────────────────────────────────────────────────────┐
│  决策速查表(11 个高频决策)                                  │
│  ────────────────────────────────────────────────────────  │
│  HP / 弹药 / 金币         →  ResourceComponent             │
│  buff / 状态               →  StateComponent (+ 自己外置回合)│
│  战斗循环 / 角色状态        →  FsmMachine (>3 个状态)       │
│  应用流程 / 主城 ↔ 战斗    →  ProcedureModule              │
│  跨实体持有目标            →  GameEntityRef                 │
│  倒计时 / 冷却 / 持续时间   →  GameTimerManager (绝对时间)   │
│  数值变化通知              →  NumericComponent.Subscribe    │
│  全局业务事件              →  EventModule                   │
│  技能 / 升级 / 抽卡公式    →  ExpressionManager             │
│  AI / NPC 决策             →  BehaviorTree (>3 个分支)     │
│  UI 指令输入              →  FairyGUIFormLogicBase          │
│  跨模块通信               →  优先 EventModule (而非 Get)    │
└──────────────────────────────────────────────────────────┘
```

---

## 不在本决策树里的"高级决策"

下面这些**不是"二选一"**,而是"系统设计"级别,见 `extension-points.md`:

- 什么时候新增组件 / Manager / 上下文?
- 怎么把"战斗 / 一局游戏"装进 `GameContextBase`?
- 组件之间怎么通信(`Subscribe` vs `GetComponent` vs `Event`)?