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

## 坑 2:`StateComponent` 引用计数 vs `Dictionary<int, bool>`

**症状**:用 `Dictionary<int, int>` 存"buff 名 → 持续回合",丢"是谁施加的"信息。

**正解**:
- `StateComponent` 用"状态 id + provider 对象"作为 key,多个 provider 可同时施加同一状态。
- **只有当一个 stateId 的所有 provider 都被 Remove 后,状态才真正消失**。
- 天然处理"两个 buff 都加中毒,最后一个被驱散才算驱散"。

**常见误用 3 种**:
1. 用 `Dictionary<int, int>` 存"buff 名 → 持续回合" —— 丢失"施加者是谁"。
2. 误以为 `RemoveState(stateId)` 不带 provider 参数就够了 —— 会误删其他 provider 加的状态。**应该始终带 provider**。
3. 把"中毒"用 `bool` 表示 —— 应该用 `int stateId`(普通中毒 / 高级中毒 可合并)。

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

## 坑 7:"持续 N 回合"塞进 `StateComponent` 内部

**症状**:误以为 `StateComponent` 自带回合倒数,实际上不会。

**正解**:
- `StateComponent` 不知道回合,它的职责是"在 / 不在"。
- 持续回合需要**业务层外置**(经典 `StatusEffectComponent` 模式,见 [`extension-points.md`](extension-points.md) §7)。
- 在"回合开始"事件里手动检查 / 减计 / 移除。

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