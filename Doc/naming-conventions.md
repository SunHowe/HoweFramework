# 命名规范(Naming Conventions)

> 来源:`Client/CLAUDE.md` 的"命名规范"节。**强制执行**,违反会让 Code Review 卡住。

---

## 一、核心命名规范

| 类型 | 规范 | 示例 |
|------|------|------|
| **模块类** | `XXXModule` | `UIModule`, `ResModule`, `EventModule` |
| **界面逻辑类** | `XXXFormLogic` | `MainMenuFormLogic`, `BattleFormLogic` |
| **游戏组件** | `XXXComponent` | `TransformComponent`, `NumericComponent` |
| **游戏管理器** | `XXXManager` | `GameEntityManager`, `GameTimerManager` |
| **游戏实体** | `GameEntity` | - |
| **组件类型枚举** | `GameComponentType` | `Transform = 1`, `View = 2` |
| **管理器类型枚举** | `GameManagerType` | `Update = 1`, `Entity = 100` |
| **事件参数类** | `XXXEventArgs`(继承 `GameEventArgs`)| `BattleStartEventArgs` |
| **状态枚举** | `XXXState`(业务自定义) | `BattleState.RoundStart` |

---

## 二、文件与目录规范

| 类别 | 规范 | 示例 |
|------|------|------|
| C# 文件名 | 与类名一致 | `TransformComponent.cs` |
| 业务代码目录 | `Assets/GameMain/Scripts/` | `Assets/GameMain/Scripts/Battle/` |
| Gameplay 业务目录 | `Assets/GameMain/Scripts/Gameplay/` | `Assets/GameMain/Scripts/Gameplay/Common/` |
| UI 业务目录 | `Assets/GameMain/Scripts/UI/` | `Assets/GameMain/Scripts/UI/Battle/` |
| 框架核心 | `Assets/HoweFramework/`(业务 Agent **不要动**)| - |
| 编辑器扩展 | `Assets/HoweFramework.Editor/`(业务 Agent **不要动**)| - |

---

## 三、命名空间规范

```csharp
// 框架核心
namespace HoweFramework.Gameplay { ... }      // Gameplay 原语
namespace HoweFramework.UI { ... }           // UI
namespace HoweFramework.Procedure { ... }    // 流程
namespace HoweFramework.Event { ... }        // 事件
namespace HoweFramework.BehaviorTree { ... } // 行为树

// 业务层
namespace GameMain { ... }
namespace GameMain.Gameplay { ... }          // 业务 Gameplay
namespace GameMain.UI { ... }                // 业务 UI
namespace GameMain.Procedure { ... }         // 业务流程
namespace GameMain.Components { ... }        // 业务自定义组件
namespace GameMain.Managers { ... }          // 业务自定义 Manager
namespace GameMain.Contexts { ... }          // 业务自定义 Context
```

---

## 四、类型 id 分配规范

### 4.1 `GameComponentType`(从 1 起,**避开 0**)

| 占用方 | 占用值 |
|--------|--------|
| **0** | **`Invalid`**(保留值,业务不能用) |
| 1-6 | 6 个内置组件(框架占用) |
| 7+ | 业务自定义(从 7+ 起即可) |

**业务 Agent 新增组件示例**:
```csharp
// Assets/GameMain/Scripts/Gameplay/GameComponentType.cs(扩展枚举)
public enum GameComponentType {
    // 内置(不要改)
    Invalid = 0,
    Transform = 1,
    View = 2,
    ViewTransformSync = 3,
    Numeric = 4,
    State = 5,
    Resource = 6,
    // 业务自定义(从 7 起)
    SkillExecutor = 7,
    StatusEffect = 8,
    Pet = 9,
    Formation = 10,
    // ...
}
```

### 4.2 `GameManagerType`(从 1 起,避开内置占用)

| 占用方 | 占用值 |
|--------|--------|
| 1-6 | 内置 6 个 Manager(框架占用) |
| 100 | `GameEntityManager`(框架占用) |
| **101+** | **业务自定义 Manager 必须从这里起** |

**业务 Agent 新增 Manager 示例**:
```csharp
// Assets/GameMain/Scripts/Gameplay/Framework/Entity/GameManagerType.cs
public enum GameManagerType {
    // 内置(不要改)
    Update = 1,
    Random = 2,
    Scene = 3,
    View = 4,
    Timer = 5,
    Expression = 6,
    Entity = 100,
    // 业务自定义(从 101 起)
    Battle = 101,
    Cleanup = 102,
    Skill = 103,
    // ...
}
```

---

## 五、`Subscribed` 方法与事件参数命名

| 类别 | 规范 | 示例 |
|------|------|------|
| `Subscribe` 方法 | `OnXxxChanged`(Action<bool> / Action<long> 等)| `OnHpChanged`, `OnSpeedChanged` |
| `GameEventArgs` 子类 | `XXXEventArgs` | `BattleStartEventArgs`, `SkillCastEventArgs` |
| 事件 ID(常量) | 业务模块前缀 + 动作 | `BattleEventId.RoundStart = 1001` |

---

## 六、`ID` / 常量命名规范

| 类别 | 规范 | 示例 |
|------|------|------|
| Numeric ID | 业务模块前缀 + 数字 | `BattleNumericId.HP = 1001`, `BattleNumericId.Speed = 1008` |
| Resource ID | 业务模块前缀 + 数字 | `BattleResourceId.HP = 1`, `BattleResourceId.MP = 2` |
| State ID | 业务模块前缀 + 数字 | `BattleStateId.Poison = 1`, `BattleStateId.Seal = 2` |
| 技能 ID | 业务模块前缀 + 数字 | `BattleSkillId.HengSao = 1001`, `BattleSkillId.ShiXin = 1002` |
| 状态枚举值 | 业务模块前缀 + 名 | `BattleState.RoundStart = 1` |

**实战**(常量类):
```csharp
public static class BattleNumericId {
    public const int HP = 1001;
    public const int MP = 1002;
    public const int Speed = 1008;
}

public static class BattleResourceId {
    public const int HP = 1;
    public const int MP = 2;
    public const int Anger = 3;
}

public static class BattleStateId {
    public const int Poison = 1;
    public const int Seal = 2;
}
```

---

## 七、不该用的命名

| 错误示范 | 错误原因 |
|---------|---------|
| `BattleFsm` | 应该是 `BattleFsmMachine` 或 `BattleStateMachine` |
| `BattleEntityMgr` | 缩写,应该是 `BattleManager` |
| `IEntity` | 跟框架 `IGameEntity` 冲突,业务用前缀 |
| `BaseComponent` | 跟框架 `GameComponentBase` 冲突 |
| `CommonHelper` | 太泛,要有具体业务语义 |
| `Manager1`, `ComponentA` | 临时命名,提交前必须改 |

---

## 八、给 Agent 的"命名决策树"

```
我要给一个类起名
    │
    ├─ 模块(全局服务 / 单例)     → XxxModule
    ├─ UI 界面逻辑                → XxxFormLogic
    ├─ 实体组件(挂在实体上)       → XxxComponent
    ├─ 容器级管理器(挂在 Context) → XxxManager
    ├─ 流程状态(应用级切换)       → XxxProcedure
    ├─ 行为树节点                 → XxxAction / XxxCondition
    ├─ 事件参数类                 → XxxEventArgs
    ├─ 状态枚举                   → XxxState(值)/ XxxStateId(常量类)
    ├─ 数据表 / 配置              → XxxTable / XxxConfig / XxxData
    └─ 工具类 / 静态方法          → XxxUtility / XxxHelper
```

---

## 九、跟现有框架不一致时怎么办?

如果实在找不到合适命名,看框架源码里类似概念怎么命名:
- 战斗 → 看 `Assets/GameMain/Scripts/Gameplay/Doc/` 下现有示例。
- UI → 看 `Assets/HoweFramework/Doc/modules/ui.md`。
- 行为树 → 看 `Assets/HoweFramework/Doc/modules/behavior-tree.md`。
- 流程 → 看 `Assets/HoweFramework/Doc/modules/procedure.md`。

**不要造新词**,跟现有框架对齐。