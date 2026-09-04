# ProcedureModule

## 职责

应用级流程：启动一组 `ProcedureBase`，同时只运行一个，可切换。

## 关键类型

- `ProcedureModule`：`Launch<T>` / `Launch`、`Stop`、`Procedure`、`ProcedureId`；内部 `ChangeProcedure`、`ChangeNextProcedure`
- `ProcedureBase`：`Id`（具体类型的 `TypeId`）、`OnEnter`/`OnLeave`/`OnUpdate`、`AddController`、`ChangeProcedure<T>`、`ChangeNextProcedure`（按 Launch 数组顺序）
- `IProcedureController`：进入时 `Initialize`，离开时 `Dispose`

## 用法

见 `GameEntry.Start`：构造流程数组后 `Launch<ProcedureSplash>(procedures)`。

业务流程在 `Client/Assets/GameMain/Scripts/Procedure/`。流程 id 是运行时 `TypeId`，不要再为流程写枚举占号。

```csharp
var procedures = new ProcedureBase[]
{
    new ProcedureSplash(),
    new ProcedureLoadDataTable(),
    new ProcedureLogin(),
};
ProcedureModule.Instance.Launch<ProcedureSplash>(procedures);
```

跳到指定流程用 `ChangeProcedure<ProcedureLogin>()`；顺序前进用 `ChangeNextProcedure()`。

## 扩展点

新流程：新 `ProcedureBase` 子类，加入 `GameEntry` 的 Launch 数组。需要进入/离开时的子逻辑用 `AddController`。不必手写 `Id`。

## 约束与坑

- `Launch` 只能一次，否则 `ProcedureAlreadyLaunch`。
- 未知 id：`ProcedureNotExist`；未运行时切换：`ProcedureNotRunning`。
- `TypeId` 是运行时分配，不要当协议号，也不要用 `Id + 1` 切流程。
- `ChangeNextProcedure` 按 **Launch 数组顺序** 前进，与 TypeId 数值无关。顺序由数组决定，不由类型名决定。
- 同一类型只能注册一次（字典按 TypeId 去重）。
- 模块销毁会 `Leave` 当前流程。

## 相关源码

- `Client/Assets/HoweFramework/Procedure/ProcedureModule.cs`
- `Client/Assets/HoweFramework/Procedure/ProcedureBase.cs`
- `Client/Assets/HoweFramework/Base/TypeId.cs`
- `Client/Assets/GameMain/Scripts/GameEntry.cs`
- `Client/Assets/GameMain/Scripts/Procedure/`
