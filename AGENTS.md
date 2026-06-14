# AGENTS.md — Project Root

> 本文件被 Claude Code / Codex CLI / Cursor / MiniMax Code 在项目根目录自动加载。
> 涉及 Unity 客户端的工作,请直接 `cd Client/` 并按 `Client/AGENTS.md` 的完整指引执行。

## 项目结构

| 目录 | 角色 | 工作入口 |
|---|---|---|
| `Client/` | **Unity 工程**(Assets/、ProjectSettings/、Packages/、cn.lys.aibridge) | `Client/AGENTS.md` —— 必读,含 AIBridge bootstrap、CLI alias、路由原则、Unity 编译命令 |
| `Server/` | 服务端工程 | 见 `Server/AGENTS.md`(目前未提供) |
| `GeekServer/` | 服务端参考实现 | 无 AI 入口 |
| `Tools/` | 工具脚本 | 无 AI 入口 |
| `FGUIProject/` | FairyGUI 工程 | 无 AI 入口 |
| `DataTable/` | Luban 数据表源 | 由 `Client/Assets/GameMain/DataTable/` 加载,改表后用 `$CLI compile unity` 触发 |
| `Doc/` | 文档 | 无 AI 入口 |

## Unity 相关任务 → 必读 `Client/AGENTS.md`

无论你是 Claude Code、Codex CLI、Cursor 还是 MiniMax Code,接到 Unity 任务后的**第一步**是:

```bash
cd Client
cat AGENTS.md      # 读完整 AIBridge bootstrap 块
$CLI harness status   # 看能力快照(codex/cursor)
```

AIBridge 已经在 `Client/AGENTS.md` 里固化了:
- **`$CLI` 别名** —— 指向 `Client/.aibridge/cli/AIBridgeCLI`,所有 host 命令、Unity 编译、日志读取都走它
- **路由原则** —— 简单查找直接答,工作流任务先加载 `aibridge-development-workflow`
- **Skill 加载路径** —— `Client/.codex/skills/<name>/SKILL.md`(codex)、`Client/.cursor/skills/<name>/SKILL.md`(cursor)
- **Unity 编译** —— **必须**用 `$CLI compile unity`,`compile dotnet` 只是额外检查
- **Unity 版本** —— 6000.3.6f1,C# 9.0

根 AGENTS.md 不重复上述细节,直接指过去。

## 通用工作流

1. 定位任务域(Unity 业务 / 框架 / 服务端 / 工具)
2. Unity 任务: `cd Client && cat AGENTS.md` 拿最新指引
3. 工作流任务: 加载 `aibridge-development-workflow`,由它决定后续 skill
4. 改完跑对应验证 —— Unity 用 `$CLI compile unity`,服务端暂无标准测试入口

## 不要做的事

- 不要在根目录上下文里直接改 `Client/Assets/...` 下的代码 —— 漏掉框架层约束和 AIBridge 路由
- 不要绕过 `aibridge-development-workflow` 凭"经验"做 Unity 工作流任务
- 不要在根目录建第二份 `Client/AGENTS.md` 的副本(用 symlink 也行,但没意义,直接 `cd Client/` 更清楚)
- 不要在 `Client/` 以外的项目子目录(GeekServer/FGUIProject 等)自作主张建 skill —— 当前没配置,先复用 `Client/` 入口
