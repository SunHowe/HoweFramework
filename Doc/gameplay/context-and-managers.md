# Context 与 Manager

## 职责

`IGameContext` 持有玩法级事件、对象池、资源加载器、状态，以及 Manager 表。`GameManagerBase` 从所属 Context 取得服务。

## 关键类型

- `IGameContext`：`EventDispatcher`、`GameObjectPool`、`ResLoader`、`GameStatus`、`Awake`/`StartGame`/`PauseGame`/`ResumeGame`/`StopGame`、`GetManager`
- `GameContextBase`：`AddManager` / `AddManager<T>`；未赋值时 `Awake` 会创建托管 dispatcher/loader/pool
- `IGameManager` / `GameManagerBase`：`ManagerType` 来自接口上的 `GameManagerAttribute`
- `GameManagerHelper.GetManager<T>(context)`
- `GameStatus`：None、Initialize、Running、Pause、Stopped

内置 `GameManagerType`：Update=1、Random=2、Scene=3、View=4、Timer=5、Expression=6、Entity=100。

对应实现：`GameUpdateManager`、`GameRandomManager`、`GameSceneManager`、`GameViewManager`、`GameTimerManager`、`ExpressionManager`、`GameEntityManager`。

`StartGame` 派发 `GameStartEventArgs` 与 `GameStatusChangeEventArgs`；`StopGame` 派发 `GameStopEventArgs`。

## 用法

子类在 `OnAwake` 里 `AddManager<GameEntityManager>()` 等。外部在 `GameStatus == None` 时才能替换 EventDispatcher/ResLoader/Pool，否则 `ErrorCode.InvalidOperationException`。

`Dispose`：若已 Running 则 `StopGame`；逆序 Dispose Manager；再释放托管的 dispatcher/pool/loader。

## 扩展点

新 Manager：接口标 `[GameManager(GameManagerType.Xxx)]`（先在枚举占用空闲值，避开 1–6 与 100），类继承 `GameManagerBase`，Context 里 `AddManager`。同一 `ManagerType` 不能加两次。

## 约束与坑

- Attribute 打在**接口**上（`AttributeTargets.Interface`），Helper 用 `GetCustomAttribute(..., inherit: true)`。
- `GetManager` 找不到返回 null。
- 托管 ResLoader 必须在托管对象池之前（`UseManagedGameObjectPool` 会确保 loader）。

## 相关源码

- `Client/Assets/GameMain/Scripts/Gameplay/Framework/GameContextBase.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Framework/GameManagerBase.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Framework/GameManagerAttribute.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/GameManagerType.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Framework/Helper/GameManagerHelper.cs`
