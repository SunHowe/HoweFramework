# 客户端域

Unity 工程在 `Client/`。薄入口：[`../../Client/AGENTS.md`](../../Client/AGENTS.md)。

知识：[`Doc/architecture/`](../../Doc/architecture/README.md)、[`Doc/framework/`](../../Doc/framework/README.md)、[`Doc/conventions/`](../../Doc/conventions/README.md)。

## 分层

| 程序集 / 目录 | 角色 | 未明确要求时 |
|---------------|------|----------------|
| `Client/Assets/HoweFramework/` | 运行时框架（`HoweFramework` 程序集） | **不要改** |
| `Client/Assets/HoweFramework.Editor/` | 编辑器扩展 | **不要改** |
| `Client/Assets/GameMain/` | 业务：流程、UI、System、Gameplay | 默认改这里 |

启动链：`GameEntry`（`GameMain/Scripts/GameEntry.cs`）构造 `GameApp`，按 `GameApp` 构造函数注册模块，再 `ProcedureModule.Launch`。细节见 [`Doc/architecture/startup.md`](../../Doc/architecture/startup.md)。

## 改业务时

- 新界面：业务放 `GameMain/Scripts/UI/`，逻辑类命名 `*FormLogic`，走 `UIModule`，不要直接操作 FairyGUI 全局单例绕过模块。
- 新流程：`ProcedureBase` 子类，挂到 `GameEntry` 的 procedures 列表，并分配 `ProcedureId`。
- 新系统：`SystemBase` + `SystemModule`，见 [`Doc/framework/modules/system.md`](../../Doc/framework/modules/system.md)。
- 需要框架能力时先查 `Doc/framework/modules/`，用现有 Module / 扩展方法；不要在业务里再实现一套事件、资源加载或对象池。

## 编译与验证

改 C# 后应以项目现有方式验证编译（若本机有 AIBridge CLI 则走其 Unity 编译；否则用 Unity 生成的工程编译）。不要把 `compile dotnet` 当成 Unity 编译的替代。

## 文档

契约或用法变了，写入 `Doc/framework/` 或 `Doc/architecture/`，不要写进 `Assets/HoweFramework/Doc/`。
