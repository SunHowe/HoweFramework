# Context 与 Manager

## 职责

`IGameContext` 持有玩法级事件、对象池、资源加载器、状态，以及 Manager 表。`GameManagerBase` 从所属 Context 取得服务。

## 关键类型

- `IGameContext`：`EventDispatcher`、`GameObjectPool`、`ResLoader`、`GameStatus`、`Awake`/`StartGame`/`PauseGame`/`ResumeGame`/`StopGame`、`GetManager`
- `GameContextBase`：`AddManager` / `AddManager<T>`；未赋值时 `Awake` 会创建托管 dispatcher/loader/pool
- `IGameManager` / `GameManagerBase`：`ManagerType` 为对外接口的 `TypeId`（实现类会归约到继承 `IGameManager` 的最根基接口）
- `GameManagerHelper.GetManager<T>(context)`
- `GameStatus`：None、Initialize、Running、Pause、Stopped

内置 Manager 接口：`IGameUpdateManager`、`IGameRandomManager`、`IGameSceneManager`、`IGameViewManager`、`IGameTimerManager`、`IExpressionManager`、`IGameEntityManager`、`IGameAbilitySystemManager`。

对应实现：`GameUpdateManager`、`GameRandomManager`、`GameSceneManager`、`GameViewManager`、`GameTimerManager`、`ExpressionManager`、`GameEntityManager`、`GameAbilitySystemManager`。

技能系统见 [`ability-system.md`](ability-system.md)。需要 Tick 的玩法应先 `AddManager<GameUpdateManager>()`。

`StartGame` 派发 `GameStartEventArgs` 与 `GameStatusChangeEventArgs`；`StopGame` 派发 `GameStopEventArgs`。

## 用法

子类在 `OnAwake` 里 `AddManager<GameEntityManager>()` 等。外部在 `GameStatus == None` 时才能替换 EventDispatcher/ResLoader/Pool，否则 `ErrorCode.InvalidOperationException`。

`Dispose`：若已 Running 则 `StopGame`；逆序 Dispose Manager；再释放托管的 dispatcher/pool/loader。

## 扩展点

新 Manager：新增 `IXxxManager : IGameManager`，类继承 `GameManagerBase` 并实现该接口，Context 里 `AddManager`。对外用 `GetManager<IXxxManager>()`。同一接口 TypeId 不能加两次。

## 约束与坑

- `ManagerType` 不以实现类 TypeId 为准，否则 `GetManager<IXxxManager>()` 对不上。
- 一个实现类不要同时实现两个互不继承的 `IGameManager` 接口。
- `GetManager` 找不到返回 null。
- TypeId 是运行时分配，不要当协议号持久化。
- 托管 ResLoader 必须在托管对象池之前（`UseManagedGameObjectPool` 会确保 loader）。

## 相关源码

- `Client/Assets/GameMain/Scripts/Gameplay/Framework/GameContextBase.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Framework/GameManagerBase.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Framework/Helper/GameManagerHelper.cs`
- `Client/Assets/HoweFramework/Base/TypeId.cs`
