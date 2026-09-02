# AGENTS.md — HoweFramework

本文件被 Claude Code / Codex CLI / Cursor / MiniMax Code 在仓库根目录自动加载。

**两套目录：**

| 目录 | 职责 |
|------|------|
| [`AGENTS/`](AGENTS/README.md) | 怎么干活：路由、任务结束沉淀、索引维护、各域约定 |
| [`Doc/`](Doc/README.md) | 项目是什么：架构、模块、Gameplay、规范、管线 |

知识**只写在** `Doc/`。`Client/Assets/**/Doc` 与框架内长 README 只是指针，不要在那里再写第二份知识。

## 开工

1. 读 [`AGENTS/README.md`](AGENTS/README.md)，再打开任务对应的域文档。
2. 读 [`Doc/README.md`](Doc/README.md)，再打开任务对应的知识文档。
3. 以源码和 `Doc/` 为准，不要沿用已删除的旧分析稿或 `Assets` 里的过时手册。

Unity 改代码时在 `Client/` 上下文工作。不要改 `HoweFramework` / `HoweFramework.Editor`，除非任务明确要求动框架。

## 任务完成（强制）

对照 [`AGENTS/workflow/distill.md`](AGENTS/workflow/distill.md)。命中任一条就必须写 `Doc/`：

- 公共 API / 模块契约变了
- 新增或修正了命名、分层、扩展方式
- 做了架构取舍（选 A 不选 B，以及原因）
- 踩到非显而易见的约束或坑
- 形成了可重复流程（如何加 UI / 协议 / 组件 / 表）

落盘后按 [`AGENTS/workflow/index-maintenance.md`](AGENTS/workflow/index-maintenance.md) **自下而上**更新 README 索引，直到 `Doc/README.md`（若改的是 agent 流程，则直到 `AGENTS/README.md`，必要时改本文件）。

**不要写进 Doc：** 会话记录、一次性且无普遍教训的修 bug、未落地设计、源码注释复印件。

## 仓库地图

| 路径 | 角色 |
|------|------|
| `Client/` | Unity 工程。启动：[`GameEntry`](Client/Assets/GameMain/Scripts/GameEntry.cs) → [`GameApp`](Client/Assets/HoweFramework/GameApp.cs) |
| `Client/Assets/HoweFramework/` | 运行时框架模块 |
| `Client/Assets/HoweFramework.Editor/` | 编辑器扩展 |
| `Client/Assets/GameMain/` | 业务层（含 Gameplay EC 框架） |
| `DataTable/` | Luban 表源 |
| `Tools/` | Luban 工具链 |
| `FGUIProject/` | FairyGUI 工程 |
| `Server/` | 自研服务端（按仓库实际内容为准） |
| `GeekServer/` | 参考服务端 |
| `AGENTS/` | Agent 流程 |
| `Doc/` | 项目知识 |

`cd Client` 时见 [`Client/AGENTS.md`](Client/AGENTS.md)，它指向本文件。

## 不要做的事

- 不要把知识写进 `Client/Assets/**/Doc` 或把框架 README 重新写成手册。
- 不要凭已删除的 `Doc/rpg/`、旧 analysis 或失效的 `CLAUDE.md` 写代码。
- 不要在仓库根 `.cursor/` 放强制规则（该目录被 `.gitignore` 忽略）；强制约定写在本文件和 `AGENTS/`。
- 不要跳过「任务完成」清单。
