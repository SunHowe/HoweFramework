# 命名

以仓库现有类型为准，不要发明第二套词。

## 模式

| 种类 | 模式 | 例子 |
|------|------|------|
| 框架模块 | `*Module` | `UIModule`、`ResModule` |
| 界面逻辑 | `*FormLogic` / `*FormLogicBase` | `FullScreenFormLogicBase` |
| 玩法组件 | `*Component` + `[GameComponent]` | `NumericComponent` |
| 玩法管理器 | `*Manager` + 接口 `[GameManager]` | `GameEntityManager` / `IGameEntityManager` |
| 事件 | `*EventArgs` : `GameEventArgs` | `GameStartEventArgs` |
| 流程 | `Procedure*` | `ProcedureLogin`、`ProcedureId` |
| 系统 | `*System` / `I*System` | `ILoginSystem` |
| 请求 | `*Request` : `RequestBase` | `OpenFormRequest` |
| 工具 | `*Utility` / `*Helper` | `TextUtility`、`GameEntityHelper` |
| 错误码 | `FrameworkErrorCode` / `ErrorCode` | 常量 int |

C# 文件名与主类型名一致。业务代码在 `Client/Assets/GameMain/Scripts/`。

## 命名空间

- 框架：`HoweFramework`
- 业务默认：`GameMain`；UI：`GameMain.UI`
- Gameplay 当前也是 `GameMain`，没有单独的 `HoweFramework.Gameplay`

## 枚举占用

- `GameComponentType`：通用组件注释区间 **1–1000**，已用 1–6。业务从 **1001** 起。
- `GameManagerType`：已用 1–6 与 **100**。业务避开这些值。
- `ProcedureId`：从 1 起且 `ChangeNextProcedure` 依赖连续 +1。

## 不要

- 不要用 `Mgr`、`IEntity`、`BaseComponent` 等与框架词冲突的短名。
- 不要把框架类型改名来迁就业务。

## 相关源码

- `Client/Assets/GameMain/Scripts/Gameplay/GameComponentType.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/GameManagerType.cs`
