# AGENTS.md

## 基本原则
1. 尽量使用简体中文回复，禁止废话，言简意赅
2. 修改复杂业务逻辑时，必须用简体中文添加必要注释
3. 尊重用户已有改动，不擅自回滚无关文件

## 项目验证
- Unity 编译只能使用 `$CLI compile unity`
- `compile dotnet` 只能作为额外检查，不能作为 Unity 编译的替代或 fallback

<!-- AIBRIDGE:START {"assistant":"aibridge","templateId":"unity-integration","version":7,"target":"root-rule"} -->
## AIBridge Bootstrap

**CLI Alias**: `$CLI = ./.aibridge/cli/AIBridgeCLI.exe`

**常用命令**:
```bash
$CLI compile unity
$CLI get_logs --logType Error
$CLI editor log --message "Hello" --logType Warning
```

**Host Exec**:
- 当 AIBridge CLI 可用时，调用 `rg`、`git`、`dotnet`、`python`、`node`、`sg`、`grep` 等外部 host 工具优先用 `$CLI exec run --stdin`，快速查找/显示任务也适用；多任务使用 `$CLI exec batch --stdin`。直接 host shell 仅用于极简单的一次性命令、用户明确要求或 AIBridge CLI 不可用时。

**路由原则**:
- 快速任务：纯问答、代码解释、简单查找/显示，且不需要修改代码或 Unity 资源、不输出审查/验证/根因结论时，直接回答或执行，不加载 `aibridge-development-workflow`。
- 工作流任务：当任务需要修改代码或 Unity 资源、修改持久化 AGENTS/Skill/workflow 规则、调试根因、采集 Runtime/日志证据，或输出风险审查/验证结论时，必须优先加载 `aibridge-development-workflow`。
- 进入工作流后，由 `aibridge-development-workflow` 探测 harness 能力、选择任务分支，并决定是否继续加载其它 Skill。

**Skill 加载**:
- 工作流任务先加载 `/.codex/skills/aibridge-development-workflow/SKILL.md` 中的 `aibridge-development-workflow`。
- AIBridge Skills 安装在 `/.codex/skills/<skill-name>/SKILL.md`；当本根规则或工作流要求时，从该目录加载同级 Skill。

**项目版本**:
- 当前项目 Unity 版本：6000.3.6f1
- 当前项目 C# 语言版本要求：兼容 C# 9.0，禁止使用更高版本语法。

**当前能力状态**:
- Harness 能力快照：`.aibridge/harness/capabilities.json`。RootRule 只提供 compact 摘要；工作流任务需要确认能力时先用 `$CLI harness status` compact 输出，仅在缺失、过期或任务需要未确认能力时读取完整 snapshot 或运行完整探测。已选助手：codex, cursor。Skill 根目录：.codex/skills, .cursor/skills。Code Index：enabled。外部 agent/sub-agent 能力：Unity 无法判断，按 unknown 处理。
- Code Index：已启用。C# 代码查找或源码导航中，只要查询可表达为符号、定义、引用、实现、派生类型、调用者或诊断查询，应优先加载 `aibridge-code-index`。Unity 已导入资源或脚本资源的名称/类型查找中，当 AIBridge 和 Editor 可用时使用 `asset search/find --format paths`。字面量内容、模糊文本、非 C# 仓库文件、任意路径正则或 Code Index/AIBridge 不可用时使用 `rg`。
<!-- AIBRIDGE:END -->
