# 扩展点(Extension Points)

> **什么时候该新增组件 / Manager / 上下文,以及怎么搭继承关系**。
> 给 Agent 的"动手前最后一关"清单。

---

## 一、新增组件 vs 复用既有

### 1.1 决策树

```
需求:实体需要一个"新维度的数据 + 行为"
    │
    ├─ 数值类(HP / MP / 速度 / 攻击)            → 用 NumericComponent
    ├─ 资源类(可消耗 + 上限 + Cost)             → 用 ResourceComponent
    ├─ 状态类(在 / 不在 + 引用计数)             → 用 StateComponent
    ├─ 位置类(Position / Euler / Scale)          → 用 TransformComponent
    ├─ 视觉类(绑定 / 同步)                       → 用 ViewComponent + ViewTransformSync
    └─ 其它(技能 / 召唤兽 / 阵法 / AI 上下文)    → 新增 GameComponentBase 子类
```

### 1.2 新增组件的"自检清单"

新增一个 `XxxComponent` 前,先回答:

- [ ] **这个组件的生命周期是"随实体销毁而销毁"吗?** 是 → 才适合做成组件。
- [ ] **这个组件需要被 `Entity.GetComponent<X>()` 找得到吗?** 是 → 才适合做成组件。
- [ ] **能不能用现有 6 个内置组件的组合表达?** 多数情况可以(尤其是 `Numeric + Resource + State` 组合)。
- [ ] **能不能用 `ScriptableObject` / DataTable 表替代?** 纯静态配置 → 用表,**不**做组件。
- [ ] **能不能用 `EventModule` + 业务层状态替代?** 业务事件 → 用事件,**不**做组件。

如果"需要做成组件"是答案,继续。

---

## 二、新增 `XxxComponent` 模板

> 必继承 `GameComponentBase`,实现 `OnAwake / OnDispose`。

### 2.1 代码骨架

```csharp
using HoweFramework.Gameplay;

namespace GameMain.Gameplay.Components
{
    /// <summary>
    /// 组件类型 id(必须与 GameComponentType 枚举值一致)。
    /// </summary>
    [GameComponent(GameComponentType.Xxx)]
    public sealed class XxxComponent : GameComponentBase
    {
        // 数据字段(引用类型,不要用 struct;会被池化)
        private SomeData m_Data;

        // 实体销毁前会调一次(回收资源 / 注销订阅)
        protected override void OnDispose()
        {
            // TODO: 注销订阅、Dispose 内部子对象
            m_Data = null;
        }

        // OnAwake 是"OnDispose 的反向":每次对象从池取出时调一次
        // 注意:OnAwake 不是构造函数,Init 数据用 OnAwake
        protected override void OnAwake()
        {
            // TODO: 初始化数据 / 注册订阅
        }

        // 业务方法
        public void DoSomething(...) { ... }
    }
}
```

### 2.2 关键约束

| 项 | 约束 |
|----|------|
| 类型 id | 用 `GameComponentType` 枚举,不要硬编码 int |
| 类访问性 | `public sealed`(避免破坏池化) |
| 继承 | 必须 `GameComponentBase` |
| 字段类型 | **不要**用 `struct`(对象池复用时会有问题) |
| 内部引用 | `OnDispose` 全部清空,但不释放"被引用对象"本身 |

### 2.3 命名规范

- 类名:`XxxComponent`
- 文件:`XxxComponent.cs`
- 类型 id:扩展 `GameComponentType` 枚举,不重复框架已用的 1-6

---

## 三、新增 `XxxManager` 模板

> 必继承 `GameManagerBase`,挂到 Context 上。

### 3.1 决策树

```
需求:一组容器级服务 / 全局数据 / 异步计时
    │
    ├─ 已有内置 Manager 能拼     → 用组合(GameUpdate + GameTimer + ...)
    └─ 都没有                    → 新增 GameManagerBase 子类
```

### 3.2 代码骨架

```csharp
using HoweFramework.Gameplay;

namespace GameMain.Gameplay.Managers
{
    /// <summary>
    /// Manager 类型 id(必须与 GameManagerType 枚举值一致)。
    /// 自定义 id 必须从 101+ 起,内置用了 1-6 和 100。
    /// </summary>
    [GameManager(GameManagerType.Xxx)]
    public sealed class XxxManager : GameManagerBase
    {
        // 容器级服务(每个 Context 一份)
        private readonly Dictionary<int, XxxData> m_AllData = new();

        public XxxData Get(int id) => m_AllData.TryGetValue(id, out var data) ? data : null;

        protected override void OnAwake() { /* 初始化容器级服务 */ }
        protected override void OnDispose() { m_AllData.Clear(); }
    }
}
```

### 3.3 关键约束

| 项 | 约束 |
|----|------|
| 类型 id | 必须从 101+ 起,不能跟内置 1-6 / 100 冲突 |
| 类访问性 | `public sealed` |
| 继承 | 必须 `GameManagerBase` |
| 字段类型 | `Dictionary` / `List` 等容器清空即可 |

---

## 四、新增 `XxxContext` 模板

> 业务每种"容器化场景"都可以新建 Context。**一场战斗 = 一个 Context**。

### 4.1 何时新建 Context?

- **一场战斗 / 一局游戏**(独立状态,结束统一清理)。
- **一个副本 / 一关 / 一个地图**(独立状态)。
- **多语言场景**(独立资源)。
- **任何"一组实体 + 一组 Manager 一起活一起死"的场景**。

**不要**给每个实体一个 Context —— Context 是"一批实体的容器",不是实体的属性。

### 4.2 代码骨架

```csharp
using HoweFramework.Gameplay;

namespace GameMain.Gameplay.Contexts
{
    public sealed class BattleContext : GameContextBase
    {
        // 一场战斗的所有"容器级"数据
        public List<IGameEntity> AllUnits { get; } = new();
        public SkillTable SkillTable { get; private set; }
        public StatusTable StatusTable { get; private set; }

        protected override void OnAwake()
        {
            // 注册 Manager
            CreateManager<GameEntityManager>();
            CreateManager<GameUpdateManager>();
            CreateManager<GameTimerManager>();
            // ... 自己注册的自定义 Manager
        }

        protected override void OnAfterAwake()
        {
            // 此时所有 Manager 已就绪
        }

        protected override void OnDispose()
        {
            // 清理容器级数据
            AllUnits.Clear();
        }
    }
}
```

### 4.3 生命周期

```
None → Initialize → Running ↔ Pause → Stopped
```

| 状态 | 含义 |
|------|------|
| `None` | 未创建 |
| `Initialize` | `OnAwake` 调用中 |
| `Running` | 正常运行 |
| `Pause` | 暂停(可恢复) |
| `Stopped` | 终止(不可恢复,只能 Dispose) |

### 4.4 实战要点

- **一场一战**:每场战斗一个 `BattleContext`,结束统一 `OnDispose`。
- 不要把"全游戏所有实体"挂在一个全局 Context 上,内存不释放。
- `OnDispose` 要把 `FsmMachine` / `BehaviorTree` / 订阅的事件 / 计时器全部清掉。

---

## 五、自定义 `Procedure` 模板

> 应用级流程切换(启动→登录→主城→战斗→回主城)。

### 5.1 代码骨架

```csharp
using HoweFramework.Procedure;

namespace GameMain.Procedure
{
    public sealed class BattleProcedure : ProcedureBase
    {
        protected override void OnEnter()
        {
            // 启动战斗 Context
            var ctx = BattleContext.Create();
            ctx.Awake();
        }

        protected override void OnUpdate() { /* 每帧逻辑(可选) */ }

        protected override void OnLeave()
        {
            // 战斗结束,清理 Context
            // (具体怎么清理取决于 BattleContext 持有者是谁)
        }
    }
}
```

### 5.2 启动流程

```csharp
ProcedureModule.Instance.Launch(
    startProcedureId: "Battle",
    procedures: new ProcedureBase[] {
        new SplashProcedure(),
        new LoadDataTableProcedure(),
        new LoginProcedure(),
        new MainCityProcedure(),
        new BattleProcedure(),
    });
```

---

## 六、自定义 `BehaviorAction` / `BehaviorCondition` 模板

> AI 行为树的自定义节点。

### 6.1 Action 节点

```csharp
using HoweFramework.BehaviorTree;

public sealed class SelectTargetAction : BehaviorActionNodeBase
{
    protected override BehaviorResult DoExecute(object userData)
    {
        var ctx = (BattleContext)userData;
        // 选血最少的目标
        var target = ctx.AllUnits.Where(e => IsEnemy(ctx.Self, e))
                                  .OrderBy(e => e.GetComponent<ResourceComponent>()
                                                  .Get(BattleResourceId.HP))
                                  .First();
        ctx.Target = target;
        return BehaviorResult.Success;
    }
}
```

### 6.2 Condition 节点

```csharp
using HoweFramework.BehaviorTree;

public sealed class HpLowCondition : BehaviorConditionNodeBase
{
    protected override bool OnEvaluate(object userData)
    {
        var ctx = (BattleContext)userData;
        var hp = ctx.Self.GetComponent<ResourceComponent>().Get(BattleResourceId.HP);
        var maxHp = ctx.Sellf.GetComponent<ResourceComponent>().GetMax(BattleResourceId.HP);
        return hp < maxHp * 0.3f;  // HP < 30%
    }
}
```

### 6.3 关键概念

- `userData` 可以传任意 class(`BattleContext` 这样的)。
- `BehaviorRoot.Execute(userData)` 是入口。
- `BehaviorRoot` 本身实现 `IBehaviorContext`,可通过 `SetValue/GetValue<T>(key, value)` 在节点间共享黑板数据。

---

## 七、扩展"持续 N 回合"的状态(经典扩展模式)

> 这个模式太常用了,单独列。

### 7.1 需求

`StateComponent` 不知道回合。要做"中毒 3 回合",需要外置"剩余回合数"管理。

### 7.2 实现

```csharp
public sealed class StatusEffectComponent : GameComponentBase
{
    private readonly Dictionary<int, StatusInstanceList> m_Active = new();

    public void ApplyStatus(int statusId, IGameEntity target, object provider, int duration, BattleContext ctx)
    {
        var stateComp = target.GetComponent<StateComponent>();
        var def = ctx.StatusTable.Get(statusId);
        stateComp.AddState(statusId, provider);

        if (!m_Active.TryGetValue(target.EntityId, out var list))
        {
            list = new StatusInstanceList { Owner = target };
            m_Active[target.EntityId] = list;
        }
        list.Add(new StatusInstance {
            StatusId = statusId,
            Provider = provider,
            RemainingRounds = duration,
            Definition = def,
        });
    }

    public void TickAll(BattleContext ctx)
    {
        foreach (var (entityId, list) in m_Active)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var inst = list[i];
                inst.RemainingRounds--;
                inst.Definition.OnTick?.Invoke(list.Owner, inst, ctx);  // 中毒扣血

                if (inst.RemainingRounds <= 0)
                {
                    list.Owner.GetComponent<StateComponent>()
                        .RemoveState(inst.StatusId, inst.Provider);
                    list.RemoveAt(i);
                }
            }
        }
    }
}
```

### 7.3 关键点

- `StateComponent` 只负责"在不在"。
- 持续回合数由 `StatusEffectComponent` 外置。
- 到 0 时调 `RemoveState`。
- `OnTick` 在 `Definition` 里定义(中毒扣血 / 隐身失效 / 封印结束……)。

---

## 八、扩展"FsmMachine 战斗循环"模板

> 把战斗阶段切成一组状态。

### 8.1 状态枚举

```csharp
public enum BattleState
{
    Idle = 0,
    RoundStart = 1,
    Sorting = 2,         // 按速度排序
    WaitingCommand = 3,  // 玩家 30s 下指令
    Executing = 4,       // 按顺序执行指令
    RoundEnd = 5,
    CheckingResult = 6,
    Over = 7,
}
```

### 8.2 核心骨架

```csharp
public sealed class BattleManager : GameManagerBase
{
    private FsmMachine m_Fsm;
    public int CurrentRound { get; private set; }
    public List<IGameEntity> AllUnits { get; } = new();

    protected override void OnAwake()
    {
        m_Fsm = FsmMachine.Create();
        m_Fsm.AddState((int)BattleState.RoundStart);
        m_Fsm.AddState((int)BattleState.Executing);
        m_Fsm.AddState((int)BattleState.RoundEnd);
        m_Fsm.AddState((int)BattleState.Over);

        m_Fsm.RegisterStateEnter((int)BattleState.RoundStart, OnRoundStartEnter);
        m_Fsm.RegisterStateEnter((int)BattleState.Executing, OnExecutingEnter);
        m_Fsm.RegisterStateEnter((int)BattleState.RoundEnd, OnRoundEndEnter);
        m_Fsm.RegisterStateEnter((int)BattleState.Over, OnOverEnter);
    }

    private void OnRoundStartEnter() { /* 加回合数、排序、播开场动画 */ }
    private void OnExecutingEnter() { /* 按顺序执行 */ }
    private void OnRoundEndEnter() { /* 胜负判定 / 状态衰减 */ }
    private void OnOverEnter() { /* 清理资源 / 关闭 UI / Dispose Context */ }

    protected override void OnDispose()
    {
        m_Fsm?.Dispose();
        m_Fsm = null;
    }
}
```

### 8.3 关键约束

- `m_Fsm?.Dispose()` 必调 —— 不归还到引用池就泄漏。
- 不要把"应用流程"塞进 Fsm(用 Procedure)。
- 不要把"单个实体的状态(中毒/眩晕)"塞进 Fsm(用 StateComponent)。

---

## 九、扩展"FairyGUI 战斗界面"模板

> 业务 UI 全部走 `FairyGUIFormLogicBase`。

### 9.1 FormLogic 骨架

```csharp
using HoweFramework.UI;

namespace GameMain.UI.Battle
{
    public sealed class BattleFormLogic : FairyGUIFormLogicBase
    {
        private GImage m_PlayerAvatar;
        private GImage m_MonsterAvatar;
        private GProgressBar m_PlayerHpBar;
        private GProgressBar m_MonsterHpBar;
        private BattleContext m_BattleContext;

        protected override void OnInit()
        {
            // FGUI 资源加载(假设 formId 已在 FGUIProject 配好)
            m_PlayerAvatar = GetChild("player") as GImage;
            m_MonsterAvatar = GetChild("monster") as GImage;
            m_PlayerHpBar = GetChild("playerHp") as GProgressBar;
            m_MonsterHpBar = GetChild("monsterHp") as GProgressBar;
        }

        public void BindBattle(BattleContext ctx)
        {
            m_BattleContext = ctx;
            ctx.Player.GetComponent<ResourceComponent>()
                .Subscribe(BattleResourceId.HP, OnPlayerHpChanged, notifyImmediately: true);
            ctx.Monster.GetComponent<ResourceComponent>()
                .Subscribe(BattleResourceId.HP, OnMonsterHpChanged, notifyImmediately: true);
        }

        private void OnPlayerHpChanged(long hp)
        {
            var max = m_BattleContext.Player.GetComponent<ResourceComponent>().GetMax(BattleResourceId.HP);
            m_PlayerHpBar.value = (float)hp / max * 100;
        }

        protected override void OnDispose()
        {
            m_BattleContext?.Player.GetComponent<ResourceComponent>()
                .Unsubscribe(BattleResourceId.HP, OnPlayerHpChanged);
        }
    }
}
```

### 9.2 关键约束

- `OnDispose` 必须**取消订阅** —— `ResourceComponent` / `NumericComponent` 的 `Subscribe` 不会自动释放。
- `BindBattle` 把 `BattleContext` 注入,FormLogic 不自己创建 Context。

---

## 十、扩展"录像 / 回放"模式

> 用 `GameRandomManager.SetSeed` 实现。

### 10.1 实现要点

- 战斗开始时 `GameRandomManager.Instance.SetSeed(seed)`,**保存 seed + 玩家操作序列**。
- 回放时 `SetSeed(原seed)`,然后按序列重放玩家操作。
- AI 用同样的随机数序列,行为完全可重放。
- **不要**自己 `new Random()` 期望可回放。

### 10.2 关键代码

```csharp
// 保存
var record = new BattleRecord {
    Seed = GameRandomManager.Instance.CurrentSeed,
    PlayerCommands = m_Commands,
    // ...
};

// 回放
GameRandomManager.Instance.SetSeed(record.Seed);
foreach (var cmd in record.PlayerCommands)
    cmd.Execute();
```

---

## 十一、何时**不**要扩展

下列情况**不要**新增组件 / Manager / 上下文:

| 情况 | 应该怎么做 |
|------|----------|
| 纯静态数据(技能表 / 装备表) | 用 `DataTableModule` + ScriptableObject |
| 业务事件广播 | 用 `EventModule.Fire / Subscribe` |
| 全局服务(Singleton / 静态服务) | 用 `IOCModule` 注册 / 直接 static |
| 临时计算结果 | 用 C# struct + 方法,不需要持久化 |
| 跨回合状态但是"小且不重要" | 用 `Blackboard`(FsmMachine 自带)或 `Dictionary<EntityId, T>` |
| UI 状态(按钮高亮 / 输入框内容) | 用 FGUI FormLogic 自己的字段,不污染业务层 |

---

## 十二、扩展"框架"本身的约束

> 业务 Agent **不应**改框架核心 `Assets/HoweFramework/`。
> 如果真要改:
- 改前先在 `Client/CLAUDE.md` 看清楚。
- 改完在 `Doc/CHANGELOG.md` 记一笔。
- 在 `Doc/primitives.md` 同步更新对应章节。

框架 patch 的典型场景:
- 框架 bug fix
- 框架 API 扩展(加方法 / 加字段)
- 新加一个内置 Manager 类型(慎重,要更新 `GameManagerType` 枚举)

非典型场景:
- 加新游戏类型支持 → 不应该改框架,业务层加。
- 加新数据格式 → 不应该改框架,业务层加 ScriptableObject。