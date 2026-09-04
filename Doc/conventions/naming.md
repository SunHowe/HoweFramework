# 命名

以仓库现有类型为准，不要发明第二套词。

## 模式

| 种类 | 模式 | 例子 |
|------|------|------|
| 框架模块 | `*Module` | `UIModule`、`ResModule` |
| 界面逻辑 | `*FormLogic` / `*FormLogicBase` | `FullScreenFormLogicBase` |
| 玩法组件 | `*Component` : `GameComponentBase` | `NumericComponent` |
| 玩法管理器 | `*Manager` + `I*Manager` : `IGameManager` | `GameEntityManager` / `IGameEntityManager` |
| 事件 | `*EventArgs` : `GameEventArgs` | `GameStartEventArgs` |
| 流程 | `Procedure*` | `ProcedureLogin` |
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

- 玩法组件 / Manager / 流程不再用枚举占号，运行时 id 用 `TypeId` / `TypeId<T>.Id`。
- `ChangeNextProcedure` 按 Launch 数组顺序前进，不要用 TypeId 数值加减。

## 不要

- 不要用 `Mgr`、`IEntity`、`BaseComponent` 等与框架词冲突的短名。
- 不要把框架类型改名来迁就业务。

## 相关源码

- `Client/Assets/HoweFramework/Base/TypeId.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Framework/Helper/GameEntityHelper.cs`
- `Client/Assets/GameMain/Scripts/Gameplay/Framework/Helper/GameManagerHelper.cs`
