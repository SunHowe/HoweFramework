# 框架能力总览

> **给 Agent 的第一份必读文档** —— 5 分钟讲清 HoweFramework 能给业务做什么。

---

## 一、4 层架构

```
┌──────────────────────────────────────────────────────────────┐
│ Server / GeekServer         (可选 — 服务端/同步)                │
├──────────────────────────────────────────────────────────────┤
│ GameMain/Scripts/           (业务层 — 你主要在这里写代码)       │
│   ├── Gameplay/             (玩法框架,6 组件 + 7 Manager)      │
│   ├── UI/                   (界面业务)                          │
│   ├── Procedure/            (流程业务)                          │
│   ├── Network/              (协议业务)                          │
│   └── ...                                                      │
├──────────────────────────────────────────────────────────────┤
│ HoweFramework               (框架核心 — 20 个 Module)           │
│ HoweFramework.Editor        (编辑器工具 — 模板/代码生成)        │
├──────────────────────────────────────────────────────────────┤
│ Unity                       (引擎层)                            │
└──────────────────────────────────────────────────────────────┘
```

**关键点**:
- 游戏代码可以引用框架,**框架不应引用游戏代码**(依赖单向)。
- 业务代码基本只动 `GameMain/Scripts/` 这块。
- 框架核心基本不动,除非打 patch。

---

## 二、20 个框架 Module 分类(快速回忆)

> 完整清单见 `Assets/HoweFramework/README.md`,这里只分类。

### 2.1 容器 / 调度类(决定何时实例化、生命周期)

| 模块 | 干什么的 |
|------|---------|
| `IOCModule` | 依赖注入,`[Inject]` 属性标记字段 |
| `BaseModule` | `IJsonHelper`、`ITextTemplateHelper` 基础工具注册 |
| `SystemModule` | 系统统一管理 |
| `ProcedureModule` | **应用级流程**切换(启动→登录→主城→战斗→回主城) |

### 2.2 资源 / 数据 / 配置

| 模块 | 干什么的 |
|------|---------|
| `ResModule` | YooAsset 包装,**只提供局部 `ResLoader`**,用完必须 `Dispose()` |
| `DataTableModule` | Luban 配置表,4 种加载模式(启动异步/启动同步/懒加载/懒加载+预加载) |
| `LocalizationModule` | 多语言 |
| `SettingModule` | 键值对 + Json 序列化 |
| `GameObjectPoolModule` | Unity GameObject 池 |

### 2.3 时间 / 帧 / 异步

| 模块 | 干什么的 |
|------|---------|
| `TimerModule` | 全局计时器,Dispose 时回收 |
| `RemoteRequestModule` | 远程请求调度(基于 UniTask 的异步包装) |
| `NetworkModule` | TCP 网络,基于 UniTask |
| `WebRequestModule` | HTTP 请求 |
| `CameraModule` | 相机管理 |

### 2.4 表现 / 用户界面

| 模块 | 干什么的 |
|------|---------|
| `UIModule` | FairyGUI 包装,提供 `OpenForm(formId)` / `FairyGUIFormLogicBase` |
| `SoundModule` | 音频 |
| `SceneModule` | 场景管理(Additive 模式) |
| `SafeAreaModule` | 刘海屏 / 安全区适配 |

### 2.5 玩法相关(Gameplay 业务层)

| 模块/原语 | 干什么的 |
|------|---------|
| `BehaviorModule` | 行为树(怪物 AI 用) |
| `EventModule` | 全局事件 / 局部事件 |
| `GameEntityManager` | 实体增删(配合 `IGameEntity`) |
| `GameContextBase` | 一场战斗 / 一局游戏的上下文 |
| 6 个内置组件 | `Transform` / `View` / `ViewTransformSync` / `Numeric` / `State` / `Resource` |

---

## 三、"Gameplay" 模块归属 —— 重要的版本冲突

> 这一段是 **Agent 最容易踩的坑**。

### 3.1 三处描述不一致

| 来源 | 说法 |
|------|------|
| 根 `README.md` | "Gameplay" 列在框架核心模块表里 |
| `Client/CLAUDE.md` | 把 Gameplay 归到业务层 `Assets/GameMain/Scripts/Gameplay/` |
| `Assets/HoweFramework/Doc/gameplay/README.md` | 同上,Gameplay 是业务层 |
| 实际目录 | `Assets/GameMain/Scripts/Gameplay/`(在 `GameMain` 下,**不在框架核心下**)|

### 3.2 正确解读

**以 `CLAUDE.md` 与 `Doc/gameplay/README.md` 为准** —— "Gameplay" 是业务层,不是框架核心。

读根 `README.md` 时要注意:它列的模块清单**不完全可信**;真正的"框架模块"以 `Assets/HoweFramework/Doc/modules/` 下的 20 个 `*.md` 为准。

### 3.3 实战建议

- 业务 Agent 改业务代码 → 在 `Assets/GameMain/Scripts/` 下新增,**别动 `Assets/HoweFramework/`**。
- 框架 Agent 打 patch → 先在 `Client/CLAUDE.md` 看清楚再动 `Assets/HoweFramework/`。

---

## 四、框架原语速查表(高频使用的 9 个)

> 详细目录见 [`primitives.md`](primitives.md)。这里只列最常用的 9 个,做"5 秒决策"。

| 我要做的事 | 用这个原语 | API 关键字 |
|------------|-----------|-----------|
| HP / MP / 弹药 / 法力 等可消耗资源 | `ResourceComponent` | `Set / Modify / Cost / RecoverToMax / BindNumericMax` |
| 8 项基础属性 / 高级属性 / 装备加值 | `NumericComponent` | `Set / Get(id, subType) / Subscribe / NumericHelper.EncodeNumericKey` |
| 中毒 / 封印 / 隐身 / 增益减益 buff | `StateComponent` | `AddState / RemoveState(stateId, provider) / Subscribe / CheckState` |
| 战斗内阶段 / 回合状态切换 / 状态机 | `FsmMachine` | `Create / AddState / ChangeState / RegisterStateEnter/Exit / Blackboard` |
| 应用级流程切换(主城↔战斗) | `ProcedureModule` | `Launch(startId, procedures[]) / ChangeProcedure(id)` |
| 怪物 AI / 复杂决策树 | `BehaviorModule` + `BehaviorTree` | `CreateBehaviorLoader / LoadBehaviorTree / 自定义 Action/Condition` |
| 30 秒倒计时 / 持续 N 秒回调 | `GameTimerManager`(注意是 Gameplay 内,不是全局 `TimerModule`)| `AddTimer(interval, callback, ..., count)` |
| 行动条 UI / 帧更新 | `GameUpdateManager` | `RegisterUpdate(target, callback)` |
| 跨实体持有目标 / 跨回合保存引用 | `GameEntityRef` / `GameComponentRef<T>` | `entityRef.GameEntity`(自动 null-check) |

**记不住就查**:每个原语都有 `Doc/gameplay/*.md`(组件)或 `Doc/modules/*.md`(Manager)详述。

---

## 五、GameContextBase 的角色

`GameContextBase` 是"一场战斗 / 一局游戏"的**容器**:

- 一场战斗 = 一个 `BattleContext` 继承 `GameContextBase`。
- 一个 `Context` 持**一组实体** + **一组 Manager** + 一组可注入资源。
- Context 生命周期:`None → Initialize → Running ↔ Pause → Stopped`。
- 必 override:`OnAwake / OnAfterAwake / OnDispose`。

**实战要点**:
- 一场一战 —— 每场战斗一个 Context,结束统一 `OnDispose`。
- 不要把"全游戏所有实体"挂在一个全局 Context 上,内存不释放。
- 详细生命周期 → [`gotchas.md`](gotchas.md) 第 6 节。

---

## 六、自带 7 个内置 Manager

| 类型 id | Manager | 干什么 |
|---------|---------|--------|
| 1 | `GameUpdateManager` | 帧 / 物理帧 / 延迟帧注册;提供 `GameTime / GameFrame / GameFrameRate` |
| 2 | `GameRandomManager` | `GetRandom(min, max) / SetSeed(int)`(录像回放关键) |
| 3 | `GameSceneManager` | 战斗场景切换 |
| 4 | `GameViewManager` | `IViewObject` 资源加载/卸载 |
| 5 | `GameTimerManager` | `AddFrameTimer / AddTimer(interval, callback)` |
| 6 | `ExpressionManager` | `RegisterTokenParser / Evaluate(expression, userData)`(伤害公式用) |
| 100 | `GameEntityManager` | `CreateEntity / GetEntity / DestroyEntity / SpawnComponentId` |

**注意 `GameManagerType` 枚举**:内置值占用了 `1-6` 和 `100`。**业务自定义 Manager 类型 id 必须从 `101+` 起,否则跟内置冲突**。

---

## 七、跨模块工具

| 工具 | 在哪 | 干什么 |
|------|------|--------|
| `FsmMachine` | `Assets/HoweFramework/Fsm/` | 单上下文内的细粒度状态切换 |
| `BehaviorRoot` | `Assets/HoweFramework/BehaviorTree/Decor/` | 行为树入口,实现 `IBehaviorContext`,可在节点间共享黑板数据 |
| `NumericHelper` | `Assets/GameMain/Scripts/Gameplay/Common/Numeric/` | `(numericId, subType)` 复合键编解码 |
| `EventModule.Instance.Subscribe` | `Assets/HoweFramework/Event/` | 全局 / 局部事件订阅 |

---

## 八、给 Agent 的"5 分钟上手"建议

1. **第一件事**:打开 `Client/Assets/HoweFramework/Doc/gameplay/README.md` 读一遍(Gameplay 是业务层,你要写代码就在这里)。
2. **第二件事**:打开 `Client/Assets/HoweFramework/Doc/modules/README.md` 把 20 个模块名过一遍。
3. **第三件事**:挑一个模块深读(看你下一个需求最相关的)。比如要做战斗系统 → `Doc/modules/fsm.md` + `Doc/modules/behavior-tree.md` + `Doc/gameplay/state-component.md`。
4. **第四件事**:翻 [`primitives.md`](primitives.md) 看决策树,再翻 [`decision-guide.md`](decision-guide.md) 的"自写 vs 复用"。
5. **写代码前**:翻 [`gotchas.md`](gotchas.md)。

---

## 九、文档维护约定

**何时更新这份总览**:
- `Assets/HoweFramework/README.md` 模块表有变动 → 更新第二节"20 个模块"。
- `Assets/HoweFramework/Doc/gameplay/README.md` 的内置组件清单有变动 → 更新第四节。
- `GameContextBase` 生命周期有变 → 更新第五节。
- `GameManagerType` 内置值有变 → 更新第六节。

**何时不更新**:
- 业务层(梦幻西游、RPG、SLG 等)的数据 / 表 / 逻辑 → 不进这目录。
- 单纯 bug fix → 不进这目录,只在 `Client/CLAUDE.md` 或代码注释里。