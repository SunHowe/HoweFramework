# 原语目录(Primitives Catalog)

> **6 内置组件 + 7 内置 Manager + 跨模块工具**的速查手册。
> 每条都告诉你:**它做什么、关键 API、典型用法、坑**。

---

## 第一部分:6 个内置组件

> 全部定义在 `Assets/HoweFramework/Doc/gameplay/README.md` 的 `GameComponentType` 枚举里。
> 业务代码一般在 `Assets/GameMain/Scripts/Gameplay/Common/` 下扩展。

| 类型 id | 组件 | 来源 |
|---------|------|------|
| 1 | `TransformComponent` | `Doc/gameplay/entity-component.md` |
| 2 | `ViewComponent` | `Doc/gameplay/view-system.md` |
| 3 | `ViewTransformSyncComponent` | 同上 |
| 4 | `NumericComponent` | `Doc/gameplay/numeric-system.md` |
| 5 | `StateComponent` | `Doc/gameplay/state-component.md` |
| 6 | `ResourceComponent` | `Doc/gameplay/resource-component.md` |

> ⚠️ **类型 id 从 1 起,业务自定义组件 id 也可从 1 起**(只要不重复);但 **id = 0 是保留值**(`Invalid`)。见 [`extension-points.md`](extension-points.md)。

---

### 1.1 `TransformComponent` —— 位置 / 旋转 / 缩放

**它是什么**:实体在世界上的"位置 + 朝向 + 缩放"。

**关键 API**:
- `entity.GetComponent<TransformComponent>()`
- `transform.Position` / `transform.EulerAngles` / `transform.Scale`(都是属性)
- `entity.GetPosition()` / `entity.GetTargetDirection(targetEntity)`(`GameEntityHelper` 扩展)

**典型挂点**:
- 角色站位
- 子弹飞行轨迹
- 摄像机焦点

**坑**:TransformComponent **不是** `UnityEngine.Transform`;它只是数值属性,不直接挂到 GameObject。需要视觉同步请加 `ViewTransformSyncComponent`。

---

### 1.2 `ViewComponent` —— 绑定视觉对象到实体

**它是什么**:实体 → 视觉对象(`IViewObject`)的桥。视觉对象可以是 Unity GameObject、FGUI 元件、纯逻辑占位。

**关键 API**:
- `entity.GetComponent<ViewComponent>().ViewObject` —— `IViewObject` 实例
- `IViewObject` 接口属性:`ResKey / GameObject / Transform / ParentTransform / Position / EulerAngles / Scale / IsVisible / IsLoaded / OnLoaded / OnUnloaded`

**典型挂点**:
- 角色模型
- NPC 立绘
- 子弹特效

**坑**:`ViewComponent` 本身只持引用,实际加载 / 卸载由 `GameViewManager` 管。

---

### 1.3 `ViewTransformSyncComponent` —— Transform → View 自动同步

**它是什么**:`TransformComponent` 数值变化时,**自动**推到 `IViewObject`。`OnAwake` 注册同步逻辑,`OnDispose` 注销。

**关键 API**:
- 加到实体后,改 `TransformComponent.Position` 会自动同步到 View。

**典型挂点**:
- 任何"实体移动需要看到东西跟着动"的场景

**坑**:实体销毁时 View 不会自动销毁(由 View 自己的资源生命周期管理)。`ViewComponent` + `ViewTransformSyncComponent` 是常见的"开箱即用"组合。

---

### 1.4 `NumericComponent` —— 多 subType 数值(百分比 + 固定加值)

**它是什么**:能表达"基础值 + 百分比加成 + 固定加值"链式数值的属性组件。**最常用也最容易写错**的组件。

**关键 API**:
- `Set(id, subType, value)` / `Get(id, subType)` / `Modify(id, subType, delta)`
- `Subscribe(id, subType, handler, notifyImmediately)` —— 监听某个 id+subType 数值变化
- `BindNumericMax(...)` —— 由 `ResourceComponent` 调用,把 HP 上限绑到基础属性
- `Unsubscribe(...)` / `TakeSnapshot()` / `RestoreSnapshot(snap)`(存档 / 录像用)
- `NumericHelper.EncodeNumericKey(id, subType) → int` / `DecodeNumericKey(int) → (id, subType)`

**subType 枚举**(以 `Doc/gameplay/numeric-system.md` 为准,**不是**根 README):

```
Basic = 0           // 基础值
BasicPercent = 1    // 基础百分比(10000 为基数,2000 = 20%)
BasicConstAdd = 2   // 基础固定加值
FinalPercent = 3    // 最终百分比
FinalConstAdd = 4   // 最终固定加值
Final = 5           // 只读计算结果
```

**最终值公式**:

```
Final = (Basic * (1 + BasicPercent/10000) + BasicConstAdd)
        * (1 + FinalPercent/10000) + FinalConstAdd
```

**典型挂点**(覆盖大部分游戏的属性系统):
- RPG:HP 上限 / 物理伤害 / 法术伤害 / 速度
- SLG:武将攻击力 / 防御力 / 移动力
- FPS:武器伤害 / 射速 / 后坐力
- 卡牌:攻击力 / 生命值 / 费用

**坑**(必看 [`gotchas.md`](gotchas.md)):
- 百分比必须用 10000 作基数(2000 = 20%),不是小数。
- 装备 "+10% 物伤" → `FinalPercent`;装备 "+20 固定物伤" → `FinalConstAdd`。
- `Basic` vs `FinalConstAdd`:前者会被百分比缩放,后者不会。
- 根 README 与 Doc 的枚举顺序不一致,**以 Doc 为准**。

---

### 1.5 `StateComponent` —— 玩法底层"集合"基础设施

> ⚠️ **重要架构澄清**:`StateComponent` **不等于 buff**。它是底层基础设施,只关心"我当下有什么 stateId";持续时间 / 效果 / 驱散 —— 都是 **Provider 的属性**,不是 StateComponent 的。

**它是什么**:实体上"当前哪些 stateId 处于活跃"的**集合(set)成员关系追踪机制**。支持按 `(stateId, provider)` 复合键做引用计数。

```
StateComponent
   ↑
   │ AddState(stateId, provider)   ← Provider 是 "为什么这个 state 处于活跃"
   │ RemoveState(stateId, provider) ← Provider 是 "为什么这个 state 退出活跃"
   │
Provider (状态来源 / 主人)
   ↑ 一对一 / 一对多
   │
   ├── Buff 实例(中毒 3 回合 / 隐身 5 回合 / …)
   ├── Skill Effect(法术命中后的灼烧效果)
   ├── Death Mark(倒地 / 死亡 / 鬼魂标记)
   ├── Equipment Passive(装备触发的"灼烧光环")
   └── 其他业务自定义状态来源
```

**关键 API**(只是"集合成员关系" + "切换通知" —— 不持有时间、不持有效果):
- `CheckState(stateId) → bool` —— 当前是否活跃
- `AddState(stateId, provider)` / `RemoveState(stateId, provider)` —— **始终带 provider**(引用计数 + 区分施加者)
- `Subscribe(stateId, Action<bool>, notifyImmediately)` —— 监听切换
- `Unsubscribe(...)` / `GetAllStates()`

**引用计数的精确语义**:
- (stateId, providerA) Add → 引用计数 1,stateId 活跃
- (stateId, providerB) Add → 引用计数 2,**stateId 仍然活跃**(同一 stateId 可多 provider 共存)
- (stateId, providerA) Remove → 引用计数 1,**stateId 仍然活跃**(还有 providerB 在)
- (stateId, providerB) Remove → 引用计数 0,stateId **真正退出活跃**

**Provider 是什么**(关键概念):
- Provider = "为什么这个 state 处于活跃"的来源 / 主人
- **典型 Provider = Buff 实例**(有 duration / tick / dispellable)
- 其他常见 Provider:Skill Effect(法术命中后的灼烧)、Death Flag(倒地标记)、Equipment Passive(装备光环)、Aura Effect(范围光环)、Status From Damage(被击中后的反弹状态)
- Provider **自己持有**持续时间、效果、是否可驱散、tick 逻辑、owner / source 等"业务属性"
- StateComponent **只知道**"(stateId, provider) 这个组合目前是否处于活跃"
- 当一个 Provider 决定退出(持续时间到 / 被驱散 / owner 死亡) → Provider 自己调 `StateComponent.RemoveState(stateId, this)`

**典型挂点**(Provider 的典型业务场景,**不是** StateComponent 的"挂点"):

| Provider 类别 | 典型场景 | 是不是 state |
|--------------|---------|--------------|
| Buff | 中毒、封印、隐身、金刚护体 | ✅ state |
| Skill Effect | 灼烧、感电、冰冻(法术附带) | ✅ state |
| Death Flag | 倒地、死亡、鬼魂标记 | ✅ state |
| Equipment Passive | 装备光环、套装效果 | ✅ state |
| Aura Effect | 范围光环(队伍 / 区域)| ✅ state |
| World Effect | 地形 / 天气 / 时段 | ✅ state |
| 裸数据 flag | 已攻击过 / 已对话过 | ❌ 不是 state(用普通 bool 字段)|

**坑**(全部围绕"集合机制 vs Provider 职责"):
- ❌ 把"持续 N 回合"塞进 `StateComponent` —— StateComponent 不知道回合,这**不是 bug**,是设计。持续回合是 **Provider 自己持有**的。
- ❌ 把 buff 等同于 state —— buff 是 Provider 的**一种**,不是 state 本身。
- ❌ 把"buff 效果 / Tick / 驱散"塞进 `StateComponent` —— 这些都是 Provider 的职责,不是集合机制的职责。
- ✅ 在"回合开始"事件里遍历**所有活跃 Provider**,每个 Provider 自己减 / 检持续时间,**自己决定何时调 `RemoveState`**。
- ✅ 不同等级的中毒(普通 / 高级)用**同一 `stateId` + 不同 Provider** 表达,自然合并(底层机制天然处理)。
- ✅ 永远带 provider 调 `AddState / RemoveState`,否则会误删。

**详细模式见 [`Doc/rpg/05-state-component.md`](rpg/05-state-component.md) 和 [`Doc/rpg/06-buff-system.md`](rpg/06-buff-system.md)**(典型 Provider 实现)。

---

### 1.6 `ResourceComponent` —— 可消耗资源(HP / MP / 弹药 / 法力)

**它是什么**:"有上限 + 可消耗"资源的管理组件。内置绑定上限到 `NumericComponent` 的机制。

**关键 API**:
- `this[id]`(索引器)/ `Set(id, value)` / `Modify(id, delta)` / `Cost(id, value) → bool`(不足返回 false)
- `RecoverToMax(id)` —— 回满
- `GetMax(id) / SetMax(id, max) / ModifyMax(id, delta)`
- `BindNumericMax(id, maxNumericId, numericComponent)` —— 绑定上限到 `NumericComponent` 的某属性,**链式反应的核心**
- `Subscribe(id, handler)` / `Unsubscribe(...)` —— 监听资源变化

**绑定机制**(`BindNumericMax`):
- 调用 `BindNumericMax(hpId, basicNumericId, numericComp)` 后,`ResourceInfo.BindNumeric` 订阅 `NumericComponent.(basicNumericId, NumericSubType.Final)` 变化。
- 回调里把 `MaxValue = value`。
- **链式**:装备改了基础属性 → HP 上限自动重算 → 满血时自动填满。

**典型挂点**(凡是有"上限 + 可消耗"概念的游戏):
- RPG:HP / MP / 愤怒
- FPS:弹药 / 手雷数
- 卡牌:手牌数 / 法力水晶
- 模拟经营:金币 / 体力 / 行动点

**坑**:
- `Cost` 比直接 `Modify` 安全(先检查)。
- `Set` 超限自动截断到 MaxValue;`Modify` 不足自动截断到 0。
- 多资源时,`id` 自己定义(HP=1, MP=2, Anger=3)。
- 订阅者要记得 `Unsubscribe`,否则内存泄漏。

---

## 第二部分:7 个内置 Manager(都挂在 `GameContextBase` 上)

> `GameManagerType` 枚举值见 `Assets/GameMain/Scripts/Gameplay/Framework/Entity/GameManagerType.cs`。

| 类型 id | Manager | 来源 |
|---------|---------|------|
| 1 | `GameUpdateManager` | `Doc/gameplay/managers.md` |
| 2 | `GameRandomManager` | 同上 |
| 3 | `GameSceneManager` | 同上 |
| 4 | `GameViewManager` | `Doc/gameplay/view-system.md` |
| 5 | `GameTimerManager` | `Doc/gameplay/managers.md` |
| 6 | `ExpressionManager` | 同上 |
| 100 | `GameEntityManager` | `Doc/gameplay/managers.md` |

> ⚠️ **业务自定义 Manager 类型 id 必须从 101+ 起**(内置用了 1-6 和 100)。

---

### 2.1 `GameUpdateManager`(类型 1)

**它是什么**:注册帧 / 物理帧 / 延迟帧 / 延迟物理帧回调。提供 `GameTime / GameFrame / GameFrameRate`。

**关键 API**:
- `RegisterUpdate(target, callback)` / `RegisterFixedUpdate(target, callback)` / `RegisterLateUpdate(...)` / `RegisterLateFixedUpdate(...)`
- `UnregisterByTarget(target)` —— 按 target 注销

**典型挂点**:
- UI 行动条推进
- 摄像机跟随
- 持续动画 / 持续效果(Tick)

**坑**:`Update` 是 Unity 原生循环;这里的 `RegisterUpdate` 是基于 `GameUpdateManager`,**不等价** `MonoBehaviour.Update`,但等价于"在 Update 里被调"。

---

### 2.2 `GameRandomManager`(类型 2)

**它是什么**:提供随机数;**可设种子 → 可回放**。

**关键 API**:
- `GetRandom(min, max)` / `GetRandom(max)`
- `SetSeed(int)` —— 录像回放的关键

**典型挂点**:
- 伤害波动(95%-105%)
- 暴击判定
- AI 概率选择
- 掉落判定
- 抽奖

**坑**:录像回放只需要存 seed + 玩家操作序列;AI 行为完全可重放。**不要**自己 `new Random()` 然后期望可回放。

---

### 2.3 `GameSceneManager`(类型 3)

**它是什么**:战斗场景 / 副本场景的加载卸载。

**典型挂点**:战斗场景切换、副本进入、地图加载。

(详见 `Doc/gameplay/managers.md`,实际 API 比较少,主要是流程集成。)

---

### 2.4 `GameViewManager`(类型 4)

**它是什么**:`IViewObject` 资源加载 / 卸载的容器。

**关键 API**(配合 `ViewComponent`):
- `IViewObject` 接口属性:`ResKey / GameObject / Transform / ParentTransform / Position / EulerAngles / Scale / IsVisible / IsLoaded / OnLoaded / OnUnloaded`

**典型挂点**:角色模型、NPC 立绘、技能特效、子弹。

---

### 2.5 `GameTimerManager`(类型 5) —— **别和全局 `TimerModule` 搞混**

**它是什么**:**Gameplay 上下文内**的计时器,跟随 Context 生命周期,跟全局 `TimerModule` 是两个东西。

**关键 API**:
- `AddFrameTimer(...)` / `AddTimer(interval, callback, ..., count)` —— **走绝对时间**，`Time.timeScale=0` 不会卡死也给
- `RemoveTimer(id)`

**典型挂点**:
- 30 秒指令窗口
- 状态持续回合倒数
- 技能冷却
- 倒计时动画

**坑**:**绝对时间,不是帧累计**;`Time.timeScale=0` 也能计时。

---

### 2.6 `ExpressionManager`(类型 6)

**它是什么**:把"伤害公式字符串"当表达式求值。**公式可配置**(策划改 `.json` / `.asset` 不用改代码)。

**关键 API**:
- `RegisterTokenParser("$tokenName", parser)` —— 注册变量 token 解析器
- `Evaluate(expression, userData) → long`

**典型挂点**:
- 技能伤害公式:`max(1, $atk - $def) * (1 + $critRate)`
- 治疗公式:`$healPower * 0.6 + $lv * 4`
- 经验公式 / 升级公式 / 掉落公式

**坑**:`userData` 可以传任意 class;Action/Condition 节点也用 `userData` 传上下文。

---

### 2.7 `GameEntityManager`(类型 100)

**它是什么**:实体的增删改查。

**关键 API**:
- `CreateEntity() → IGameEntity`
- `GetEntity(entityId) → IGameEntity`
- `DestroyEntity(entityId)`
- `SpawnComponentId() → int` —— 组件 id 生成

**典型挂点**:**任何** 涉及实体的创建 / 销毁(玩家、怪物、子弹、召唤兽、道具)。

---

## 第三部分:跨模块工具(不是组件也不是 Manager,但是高频使用)

---

### 3.1 `FsmMachine` —— 细粒度状态机

**位置**:`Assets/HoweFramework/Fsm/`(`FsmMachine.cs` / `FsmStateBase.cs` / `IFsmMachine.cs`)

**两种使用风格**:
- 风格 1:直接注册回调(`RegisterStateEnter / RegisterStateExit`)—— 短平快,适合小状态机。
- 风格 2:OOP 子类化 `FsmStateBase` 重写 `OnEnter / OnExit` —— 适合复杂状态机(每个状态有大量逻辑)。

**关键 API**:
- `FsmMachine.Create()` —— 工厂方法创建(从引用池取)
- `AddState(id)` / `ChangeState(id)`
- `RegisterStateEnter(id, Action)` / `RegisterStateExit(id, Action)`
- `OnStateEnter` / `OnStateExit`(事件)
- `CurrentState` / `Blackboard`
- `Dispose()` —— 归还到引用池(必调)

**典型挂点**:
- 战斗内阶段切换(回合开始 → 等待指令 → 执行中 → 回合结束)
- 角色状态(待机 → 移动 → 攻击 → 受击 → 死亡)
- 任务步骤
- 教程步骤

**坑**:
- FsmMachine **不持久化**,实体销毁时记得 `Dispose`。
- 不要用 FsmMachine 管应用级流程(用 `ProcedureModule`)。

---

### 3.2 `ProcedureModule` —— 应用级流程

**位置**:`Assets/HoweFramework/Procedure/`

**关键 API**:
- `ProcedureModule.Instance.Launch(startId, procedures[])` —— 启动流程
- 单个 Procedure override `OnEnter / OnLeave / OnUpdate`
- `ChangeProcedure(id)` / `ChangeNextProcedure()` —— 切换

**典型挂点**:启动 → 登录 → 主城 → 战斗 → 主城 → 退出。

**坑**:
- 不要用 `Procedure` 管"战斗内阶段"(太重),用 FsmMachine。
- 默认流程 `ProcedureSplash → ProcedureLoadDataTable → ProcedureLoadLocalization → ProcedureInitSystem → ProcedureLogin`(参考)。

---

### 3.3 `BehaviorTree` —— 行为树

**位置**:`Assets/HoweFramework/BehaviorTree/`

**节点类型**:
- `Composite` —— Sequence / Selector / Parallel
- `Decor` —— Repeat / AlwaysFailure / AlwaysSuccess
- `Action` —— Failure / Success / Log(以及自定义)
- `Condition` —— 以及自定义

**关键 API**:
- `BehaviorModule.CreateBehaviorLoader(resLoader)` —— 加载器工厂
- `await LoadBehaviorTree(assetKey, token)` —— 异步加载行为树(JSON 配置)
- `BehaviorResult` 枚举:`Success / Failure / Running`
- 自定义 `BehaviorActionNodeBase` 重写 `DoExecute(userData)` 返回 `BehaviorResult`
- 自定义 `BehaviorConditionNodeBase` 重写 `OnEvaluate(userData)` 返回 `bool`

**关键概念:`userData` 与 `IBehaviorContext`**:
- `BehaviorRoot.Execute(userData)` 接收任意对象。
- 文档示例:`BattleContext` 含 `IGameEntity Self / Target`,可被 Action 节点读取。
- `BehaviorRoot` 本身实现 `IBehaviorContext`,可通过 `SetValue/GetValue<T>(key, value)` 在树节点间共享"黑板"数据。

**典型挂点**:
- 怪物 AI(选目标 + 选技能 + 概率释放)
- NPC 行为树
- 召唤兽 AI

---

### 3.4 `NumericHelper` —— 复合键编解码

**位置**:`Assets/GameMain/Scripts/Gameplay/Common/Numeric/NumericHelper.cs`

**关键 API**:
- `EncodeNumericKey(id, subType) → int`
- `DecodeNumericId(key) → int` / `DecodeSubType(key) → NumericSubType`

**典型用法**:`Dictionary<int, NumericData>` 作属性表时,用 `EncodeNumericKey` 把 `(numericId, subType)` 压成一个 `int` 当 key,避免字典 key 类型复杂。

**坑**:**不要**手写 `numericId * 100 + (int)subType` 这种编码,以后改 subType 数量就崩。用 `NumericHelper` 的标准编码。

---

### 3.5 `EventModule` —— 全局 / 局部事件

**位置**:`Assets/HoweFramework/Event/`

**关键 API**:
- `EventModule.Instance.Subscribe(eventId, handler)` —— 全局事件订阅
- 自定义 `GameEventArgs` 通过 `IReference` 池化

**典型挂点**:
- 全局事件(战斗开始 / 回合切换 / 胜负)
- 跨模块通信

**坑**:
- 局部属性变化用 `NumericComponent.Subscribe` / `ResourceComponent.Subscribe`,**不要用 EventModule 广播数值变化**(粒度太粗)。
- 局部事件调度器不用后要 `Dispose()`。

---

### 3.6 `GameEntityRef` / `GameComponentRef<T>` —— 跨实体安全引用

**位置**:`Assets/HoweFramework/Doc/gameplay/entity-component.md` § "Safe Reference"

**关键 API**:
- `entityRef.GameEntity`(属性,自动 null-check)
- `componentRef.Component`(属性)

**典型用法**:
- AI 行为树在第 3 回合选了一个目标,第 8 回合回来还在用同一个引用。
- 如果目标在第 5 回合被销毁,`entityRef.GameEntity` 自动变 `null`,**不会 NullReference**。

**典型挂点**:**任何**"持有一个会死的目标超过一个事件周期"的代码,都必须用安全引用。

---

## 第四部分:对照表(需求 → 原语)

> 跨游戏类型的"需求 → 原语"映射,做新游戏时直接查这个。

| 游戏需求 | 推荐原语 | 备注 |
|---------|----------|------|
| RPG / 卡牌:HP / MP / 法力 | `ResourceComponent` | 上限绑 `NumericComponent` |
| RPG / 卡牌:等级 / 经验 | `NumericComponent` | 用 `Basic` + `FinalConstAdd` |
| RPG / 卡牌 / MOBA:中毒 / 封印 / 眩晕 | `StateComponent` + **Provider**(典型 = `Buff`)| 引用计数天然处理多 Provider 叠加;持续时间在 Provider 上,不在 StateComponent 上 |
| MOBA / FPS:弹药 / 手雷数 | `ResourceComponent` | 多资源 id |
| 模拟经营 / 卡牌:金币 / 行动点 | `ResourceComponent` | 同上 |
| RPG / SLG:速度 / 攻击 / 防御 | `NumericComponent` | 百分比加成用 `FinalPercent`,固定加值用 `FinalConstAdd` |
| RPG / SLG:战斗循环 / 回合阶段 | `FsmMachine` | 一场一战,挂 Context 上 |
| 所有类型:应用级流程 | `ProcedureModule` | 启动 → 登录 → 主场景 → 战斗 → 回主场景 |
| 所有 AI 决策 | `BehaviorTree` | `userData` 传上下文 |
| 倒计时 / 持续时间 / 冷却 | `GameTimerManager` | **绝对时间**,不是帧累计 |
| 帧更新 / 行动条 UI | `GameUpdateManager` | 容器级,不是实体级 |
| 伤害波动 / 暴击 / 抽卡 | `GameRandomManager` | `SetSeed` 可录像 |
| 技能公式 | `ExpressionManager` | 公式字符串可配置 |
| UI 指令面板 | `FairyGUIFormLogicBase`(UIModule) | `OpenForm(formId)` |

---

## 速查卡片(可打印)

```
┌─────────────────────────────────────────────────────────┐
│  RPG / 卡牌 / 模拟经营 / MOBA / FPS                       │
│  ──────────────────────────────────────────────────────  │
│  HP / MP / 弹药 / 法力        →  ResourceComponent        │
│  等级 / 经验 / 攻击 / 防御    →  NumericComponent         │
│  中毒 / 封印 / 眩晕 / 隐身    →  StateComponent           │
│                                 (Provider = Buff / Skill Effect / …)│
│  战斗内阶段 / 角色状态         →  FsmMachine              │
│  应用级流程切换                →  ProcedureModule         │
│  怪物 AI / NPC 决策           →  BehaviorTree             │
│  倒计时 / 持续 / 冷却          →  GameTimerManager        │
│  帧更新 / 行动条               →  GameUpdateManager       │
│  伤害 / 经验 / 抽卡随机         →  GameRandomManager       │
│  技能 / 升级 / 治疗公式        →  ExpressionManager       │
│  UI 指令面板 / 主城 / 登录      →  FairyGUIFormLogicBase   │
│  跨实体持有目标                →  GameEntityRef            │
│  一场战斗 / 一局游戏           →  GameContextBase          │
└─────────────────────────────────────────────────────────┘
```