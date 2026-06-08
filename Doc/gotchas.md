# 常见坑(Gotchas)

> 13 条概念误读 + 版本冲突 + 未结清单。
> 写代码前扫一眼,debug 时回头看。

---

## 坑 1:把 HoweFramework 的"EC"当成"DOTS-ECS"

**症状**:以为有 Burst 加速、Struct 组件、并行 System、Archetype / Chunk。

**正解**:
- HoweFramework 的 EC = 轻量级 OOP 风格的实体-组件。
- 实体(`IGameEntity`)只持 `EntityId` + `Context`,**不存数据**。
- **组件是引用类型(class)**,通过 `AddComponent<T>()` 注册,**不用 struct**(对象池复用 + struct 矛盾)。
- 组件之间通过 `Entity.GetComponent<OtherComponent>()` 互访 —— 这是耦合,要克制,**能用事件解耦就别用 GetComponent**。
- **不涉及** DOTS / Burst / Job System / Chunk / Archetype。

**为什么重要**:误读会导致把 struct 数据当组件(对象池失效)、尝试用 Burst 优化(框架不支持)、写并行 System(框架不支持)。

---

## 坑 2:`StateComponent` 不是 buff —— 是底层"集合"基础设施

> **架构澄清**(高优先级):`StateComponent` 是**玩法底层基础设施**,**不**等于 buff。`StateComponent` 只关心"我当下有什么 stateId";持续时间 / 效果 / 驱散 —— 这些是 **Provider 的属性**,不是 StateComponent 的。

**症状**:把 `StateComponent` 当成 buff 自己 —— 试图给它加 duration / tick / dispellable 字段,试图在它内部"持续 N 回合"。

**正解架构**:
```
StateComponent(底层基础设施)
   ↑ AddState(stateId, provider)
   ↑ RemoveState(stateId, provider)
Provider(状态来源 / 主人)—— 持有 duration / tick / dispellable
   ↑
   ├── Buff 实例(中毒 3 回合 / 隐身 5 回合 …)
   ├── Skill Effect(法术命中后的灼烧)
   ├── Death Flag(倒地 / 死亡 / 鬼魂)
   └── 其他业务自定义来源
```

**`StateComponent` 真正职责**(就 3 条):
1. **(stateId, provider) 复合键的集合成员关系** —— 当前哪些 stateId 处于活跃
2. **按 provider 引用计数** —— 多 provider 同时施加 → 全部 Remove 才真正退出
3. **状态切换通知** —— `Subscribe(stateId, Action<bool>, ...)` 给 UI / AI / 录像

**`StateComponent` 不持有的职责**(全归 Provider):
- ❌ 持续时间(回合数 / 秒数)—— Provider 自己持有
- ❌ Tick 逻辑(中毒扣血 / 灼烧伤害)—— Provider 自己实现
- ❌ 驱散规则(能否被净化)—— Provider 自己属性
- ❌ owner / source(谁施加的 / 施加在谁身上)—— Provider 自己持有
- ❌ 等级(普通 / 高级中毒)—— Provider 自己区分,`StateComponent` 不区分

**常见误用 4 种**:
1. ❌ 用 `Dictionary<int, int>` 存"buff 名 → 持续回合" —— 丢失"施加者是谁"信息,且把 buff 和 state 混为一谈。
2. ❌ 误以为 `RemoveState(stateId)` 不带 provider 参数就够了 —— 会误删其他 provider 加的状态。**应该始终带 provider**。
3. ❌ 把"中毒"用 `bool` 表示 —— 应该用 `int stateId`,Provider 区分等级(普通 / 高级中毒 = 两个 Provider + 同一 stateId)。
4. ❌ **把持续时间塞进 `StateComponent`** —— StateComponent 是集合机制,不持有时间。这**不是 bug**,是设计 —— 时间是 Provider 的属性。详见坑 7。

---

## 坑 2.5(架构澄清):Provider 模式 vs "一切皆状态"反模式

> 这一条是高优先级 —— 框架层设计意图。

**反模式("一切皆状态")**:
- 把"buff 效果 / 持续时间 / Tick 逻辑"全塞进 `StateComponent`
- 把"技能命中后的灼烧"也做成 state
- 把"装备光环"也做成 state
- 结果:`StateComponent` 膨胀成万能类,失去"集合机制"的纯粹性

**正解模式("Provider 是来源")**:
- `StateComponent` 只关心"我当下有什么 stateId"
- 每个 stateId 的来源(buff / skill effect / flag / 光环 / …)都是 **Provider**
- Provider 自己持有 duration / tick / dispellable / source / level
- Provider 决定何时调 `AddState / RemoveState`
- 多 Provider 同时施加同一 stateId → 全部 Remove 才真正退出(底层机制天然处理)

**典型场景对比**:

| 场景 | 反模式 | 正解 |
|------|--------|------|
| "中毒 3 回合" | `StateComponent.AddState(PoisonId, this)` 然后在 StateComponent 内置回合倒数 | `BuffPoison(target, source, duration=3)` 初始化时调 `AddState(PoisonId, this)`,自己持有 `RemainingRounds`,到期调 `RemoveState` |
| "法术命中灼烧" | 在 StateComponent 内挂 "灼烧" 字段 + Tick | `EffectBurn(target, source, damagePerTick, duration=3)` 自己持有 duration + damage,ToString 时调 `AddState(Burning, this)` |
| "装备光环" | 在 StateComponent 内挂 "光环" 字段 | `EquipmentAura(owner, auraId)` 在 owner 进入 / 离开时对周围实体调 `AddState(AuraId, this)` |
| "倒地标记" | 在 StateComponent 内挂 "已倒地" 字段 | `DeathFlag(target, deathType)` 自己持有死亡类型,直接调 `AddState(Dead, this)` |

---

## 坑 3:`NumericComponent` 百分比基数 10000 vs 小数 0.2

**症状**:写 `entity.Set(1001, NumericSubType.BasicPercent, 0.2)` —— 结果是 0。

**正解**:
- **百分比以 10000 为基数**,2000 = 20%。
- 公式:`Final = (Basic * (1 + BasicPercent/10000) + BasicConstAdd) * (1 + FinalPercent/10000) + FinalConstAdd`
- 反例:"20% 加成"应写 `BasicPercent = 2000`,不是 `0.2`。

---

## 坑 4:混淆 `Basic` 和 `FinalConstAdd`

**症状**:把"装备 +20 物伤"用 `Basic` 加 → 被后续百分比缩放,数值错乱。

**正解**:
- `Basic` = 基础值,**会被百分比缩放**。
- `BasicConstAdd` = 基础固定加值,被基础百分比缩放。
- `FinalPercent` = 最终百分比,**不缩放 Basic,只影响最终乘区**。
- `FinalConstAdd` = 最终固定加值,**不被任何百分比影响**。
- `Final` = 只读计算结果,不要手动 Set。

**实战**:
- 装备 "+10% 物伤" → `FinalPercent = 1000`(10%)
- 装备 "+20 固定物伤" → `FinalConstAdd = 20`

---

## 坑 5:`NumericSubType` 枚举顺序在根 README 和 Doc 不一致

**症状**:照根 `README.md` 写 `Final = 0`,结果跟框架 Doc 完全对不上,数值全部错位。

**正解**:
- 以 `Assets/HoweFramework/Doc/gameplay/numeric-system.md` 为准:
  ```
  Basic = 0
  BasicPercent = 1
  BasicConstAdd = 2
  FinalPercent = 3
  FinalConstAdd = 4
  Final = 5
  ```
- 根 `README.md` 写的是早期版本,数值有差异。
- **永远以 Doc 为准**,不要看根 README 抄。

---

## 坑 6:`GameContextBase` 生命周期忘了 OnDispose

**症状**:Context 里挂着实体 / Fsm / Timer / 订阅,销毁时全没释放,内存泄漏。

**正解**:
- Context 生命周期:`None → Initialize → Running ↔ Pause → Stopped`。
- 必 override `OnAwake / OnAfterAwake / OnDispose`。
- `OnDispose` 必须:
  - [ ] 调 `m_Fsm?.Dispose()`(FsmMachine)
  - [ ] 调所有 `Timer.RemoveTimer`
  - [ ] 注销所有 `EventModule.Unsubscribe`(局部事件)
  - [ ] 注销所有 `ResourceComponent.Unsubscribe`
  - [ ] 注销所有 `NumericComponent.Unsubscribe`
  - [ ] `DestroyEntity` 所有实体
- **不**在 `OnDispose` 销毁跨模块的全局实例(`UIModule` / `EventModule`)。

---

## 坑 7:"持续 N 回合"是 Provider 的事,不是 `StateComponent` 的事

**症状**:误以为 `StateComponent` 自带回合倒数,在它内部找持续时间字段,找不到 → 觉得是 bug,自己加字段。

**正解(架构澄清)**:
- `StateComponent` **是底层基础设施**,不是 buff 自己。**它的职责是"集合(set)成员关系追踪"**,**不**持有持续时间。
- 持续时间是 **Provider 自己持有**的,不是 StateComponent 的。
- **典型 Provider = Buff 实例**(典型 Provider 见坑 2.5)。
- Provider 决定何时调 `StateComponent.AddState / RemoveState` —— 持续时间到 0 时,Provider 自己调 `RemoveState`。
- 业务层"回合开始"事件遍历**所有活跃 Provider**,每个 Provider 自己减 / 检持续时间,**不**遍历 `StateComponent`。

**典型模式**(参考实现,不是绑定):

```csharp
public class BuffPoison {
    public int RemainingRounds;
    public IGameEntity Target;
    public object Caster;
    
    public void OnApply() {
        Target.GetComponent<StateComponent>().AddState(StatusId.Poison, this);
    }
    
    public void OnTick(BattleContext ctx) {
        // 每回合减持续 + 扣血
        RemainingRounds--;
        Target.GetComponent<ResourceComponent>().Modify(ResourceId.HP, -10);
    }
    
    public void OnRemove() {
        // 持续时间到 / 被驱散 / owner 死亡 → Provider 自己决定退出
        Target.GetComponent<StateComponent>().RemoveState(StatusId.Poison, this);
    }
}

// 业务层"回合开始"事件
foreach (var buff in ctx.AllActiveBuffs) {
    buff.OnTick(ctx);
    if (buff.RemainingRounds <= 0) buff.OnRemove();
}
```

**关键点**:`StateComponent` 完全不感知持续时间。持续时间是 **BuffPoison 这个 Provider 自己持有的**。

**反模式**(❌):
```csharp
// 试图在 StateComponent 内置持续时间字段 → 错误!
// 错 1:StateComponent.AddState(id, provider, duration) ← 这是设计错位
// 错 2:在 StateComponent 内置 _durations:Dictionary<int,int> ← 这是把状态机制当成 buff 实现
```

**正解(✅)**:
- `StateComponent` 保持纯粹的"集合机制"
- 持续时间在 Provider(`BuffPoison` 等)上
- 多 Provider 共享同一 stateId → StateComponent 引用计数天然处理

**详细模式见 [`Doc/rpg/05-state-component.md`](rpg/05-state-component.md) 和 [`Doc/rpg/06-buff-system.md`](rpg/06-buff-system.md)`。

---

## 坑 8:用 `Time.timeScale` 敏感的帧累计做倒计时

**症状**:玩家暂停游戏 → 计时也停 → 战斗卡死。

**正解**:
- **任何"现实时间"含义的倒计时 / 冷却 / 持续时间 → `GameTimerManager.AddTimer` 走绝对时间**。
- `Time.timeScale=0` 也不会卡死。
- 按 `count` 自动停止,无需手动管理。
- 30 秒倒计时:`GameTimerManager.Instance.AddTimer(1f, OnTick, null, 30)` 按 30 次回调。

---

## 坑 9:`GameTimerManager`(Gameplay 内) vs `TimerModule`(全局)

**症状**:用错 Module,计时器不随 Context 生命周期释放。

**正解**:
| 名称 | 范围 | 用途 |
|------|------|------|
| `GameTimerManager` | **Gameplay Context 内**(挂在 Context 上) | 跟随 Context 生命周期 |
| `TimerModule.Instance` | **全局** | 应用级 / 跨场景计时 |

**实战**:
- 战斗内 30s 倒计时 / 状态持续回合 → `GameTimerManager`(上下文结束自动清理)。
- 全屏公告 / 跨战斗计时 → `TimerModule`(全局)。

---

## 坑 10:把 `Procedure` 当 `FsmMachine` 用

**症状**:用 `Procedure` 管"战斗内阶段切换",每次切 Procedure 太重,丢失 Context。

**正解**:
- `FsmMachine` 用于**单个上下文内的细粒度状态切换**(战斗内阶段)。
- `Procedure` 用于**应用级流程切换**(启动→登录→主城→战斗→主城)。

**简记**:**粗粒度流程 → Procedure**;**细粒度状态 → Fsm**。

---

## 坑 11:跨实体持有裸 `IGameEntity`

**症状**:AI 行为树选了目标,5 回合后还在用同一个裸引用;目标被销毁 → `NullReference`。

**正解**:
- **持有一个会死的目标超过一个事件周期 → 必须用 `GameEntityRef`**。
- `entityRef.GameEntity` 自动变 `null`,不会崩。
- `GameComponentRef<T>.Component` 同理。

**例外**:持有一个**确定不会被销毁**的引用(全局静态服务 / Singleton),裸引用 OK。

---

## 坑 12:新增 `GameManager` 类型 id 跟内置冲突

**症状**:业务 Manager 类型 id 写成 `1`-`6` 或 `100` → 跟内置 `GameUpdate/Random/Scene/View/Timer/Expression/Entity` 冲突,Context 注册时崩溃或被覆盖。

**正解**:
- 内置 `GameManagerType` 枚举占用了 `1-6` 和 `100`:
  - 1 = `GameUpdateManager`
  - 2 = `GameRandomManager`
  - 3 = `GameSceneManager`
  - 4 = `GameViewManager`
  - 5 = `GameTimerManager`
  - 6 = `ExpressionManager`
  - 100 = `GameEntityManager`
- **业务自定义 Manager 类型 id 必须从 101+ 起**。

---

## 坑 13:新增 `GameComponent` 类型 id 写成 0

**症状**:写了 `GameComponentType.Xxx = 0` → 跟 `Invalid` 保留值冲突,组件注册失败。

**正解**:
- `GameComponentType.Invalid = 0` 是保留值。
- 业务自定义 id 从 1+ 起(但不要重复 1-6)。

---

## 版本冲突清单(Agent 需要知道的"哪个文档为准")

| 冲突点 | 来源 A | 来源 B | 以哪个为准 |
|--------|--------|--------|----------|
| `NumericSubType` 枚举顺序 | 根 README(早期版本) | `Doc/gameplay/numeric-system.md` | **以 Doc 为准** |
| "Gameplay" 模块归属 | 根 README(列在框架核心表) | `CLAUDE.md` + `Doc/gameplay/README.md` | **以 CLAUDE.md / Doc 为准** |
| 客户端示例工程结构 | 根 README("Client" 目录) | `CLAUDE.md`(更详细,标了具体子目录) | **以 CLAUDE.md 为准** |
| 模块清单 | 根 README(13 个) | `CLAUDE.md`(20 个) | **以 CLAUDE.md 为准** |

---

## 未结清单(已知未确认事项)

> 这些是"已识别但文档未统一"的项;新增代码时遇到要回头查。

| # | 事项 | 状态 |
|---|------|------|
| 1 | "Gameplay" 模块归属(根 README vs 业务层实际位置) | 已澄清:**业务层** |
| 2 | `NumericSubType` 枚举顺序(根 README vs Doc) | 已澄清:**以 Doc 为准** |
| 3 | 行为树 Condition 节点完整实现 | 已有框架源码,业务可继承 `BehaviorConditionNodeBase` |
| 4 | 服务端权威(GeekServer)与客户端的事件同步 | 未深入,业务层按需展开 |
| 5 | 网络协议层与 Gameplay 业务层的职责切分 | 未深入,业务层按需展开 |
| 6 | 性能优化(Burst / Job System / 池高级调优) | 框架不走 DOTS 路线,不进 |
| 7 | 多语言场景(`SceneModule` 与 `Localization` 协作) | 未深入 |
| 8 | SafeArea 与刘海屏适配的具体规则 | 框架有 `SafeAreaModule`,业务可读源码 |
| 9 | `IOCModule` 在 Gameplay Context 内的使用 | 未深入,业务按需 |
| 10 | `ReferencePool` 的池化策略与游戏业务对象的兼容性 | 未深入,业务可读源码 |
| 11 | 录像回放需要存哪些数据(seed + 玩家操作 + AI 状态) | 已基本明确,见 [`extension-points.md`](extension-points.md) §10 |
| 12 | 跨游戏的"通用战斗引擎"层应该封装到什么程度 | 未决(由业务 Agent 决策) |
| 13 | "Gameplay" 模块目录是否要重构(目前散在 `GameMain/Scripts/Gameplay/` 多个子目录) | 未决(由业务 Agent 决策) |

---

## 调试清单(Agent 跑代码时遇到问题翻这页)

| 现象 | 翻哪条 |
|------|--------|
| 组件 `GetComponent<T>()` 返回 null | 坑 6(忘了 `AddComponent`)、坑 11(用了裸引用已被销毁) |
| 数值算出来不对 | 坑 3(百分比基数错)、坑 4(Basic vs FinalConstAdd 错)、坑 5(枚举顺序错) |
| buff 不消失 / 错消失 | 坑 2(没带 provider、误用 Dictionary) |
| 倒计时暂停游戏时卡住 | 坑 8(用了帧累计、用了 Time.timeScale 敏感) |
| Context 内存泄漏 | 坑 6(没 OnDispose)、坑 9(用错 Timer) |
| AI 行为树不执行 | 坑 7(节点类型)、`userData` 转换错误、`Condition.OnEvaluate` 没返回 true |
| UI 不刷新 | `ResourceComponent` 订阅没 `notifyImmediately`、`OnDispose` 提前取消订阅 |
| 新组件 id 冲突 | 坑 12(Manager)、坑 13(Component) |
| 录像回放 AI 行为不一样 | `GameRandomManager.SetSeed` 没在战斗开始调 |
| 跨模块数据不同步 | 坑 11(用了裸引用)、`NumericComponent.Subscribe` vs `EventModule` 选错 |