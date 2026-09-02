# ProcedureModule

## 职责

应用级流程：启动一组 `ProcedureBase`，同时只运行一个，可切换。

## 关键类型

- `ProcedureModule`：`Launch`、`Stop`、`Procedure`、`ProcedureId`；内部 `ChangeProcedure`
- `ProcedureBase`：`Id`、`OnEnter`/`OnLeave`/`OnUpdate`、`AddController`、`ChangeProcedure`、`ChangeNextProcedure`（`Id + 1`）
- `IProcedureController`：进入时 `Initialize`，离开时 `Dispose`

## 用法

见 `GameEntry.Start`：构造流程数组后 `Launch((int)ProcedureId.Splash, procedures)`。

业务流程在 `Client/Assets/GameMain/Scripts/Procedure/`，id 见 `ProcedureId`。

## 扩展点

新流程：新 `ProcedureBase` + `ProcedureId` + 加入 `GameEntry` 列表。需要进入/离开时的子逻辑用 `AddController`。

## 约束与坑

- `Launch` 只能一次，否则 `ProcedureAlreadyLaunch`。
- 未知 id：`ProcedureNotExist`；未运行时切换：`ProcedureNotRunning`。
- `ChangeNextProcedure` 假定枚举值连续。
- 模块销毁会 `Leave` 当前流程。

## 相关源码

- `Client/Assets/HoweFramework/Procedure/ProcedureModule.cs`
- `Client/Assets/HoweFramework/Procedure/ProcedureBase.cs`
- `Client/Assets/GameMain/Scripts/GameEntry.cs`
- `Client/Assets/GameMain/Scripts/Procedure/ProcedureId.cs`
