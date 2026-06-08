# 沉淀记录(Changelog)

> `Doc/` 目录自身的变更记录。每改 Doc 都要在这记一笔。

---

## 2026-06-08 (2) · 架构修正:StateComponent ≠ buff,buff = Provider

**触发事件**:用户指出沉淀的 `Doc/primitives.md` 1.5 章节、`Doc/gotchas.md` 坑 2 / 7、`Doc/decision-guide.md` 决策 2、`Doc/rpg/05-status.md` 存在架构层面的概念错误 —— `StateComponent` **不**等于 buff,它是玩法底层的"集合"基础设施;buff 是 **Provider** 的一种,自己持有 duration / tick / dispellable / source / level。

**修正清单**:

| 位置 | 改动 |
|------|------|
| `Doc/primitives.md` 1.5 | 重写 StateComponent 描述:从"buff 风格的引用计数布尔状态"改为"底层基础设施 / 集合机制 / 不持有时间效果驱散";新增"Provider 是什么"小节 |
| `Doc/gotchas.md` 坑 2 | 重写为"`StateComponent` 不是 buff —— 是底层'集合'基础设施";新增坑 2.5(Provider 模式 vs "一切皆状态"反模式)|
| `Doc/gotchas.md` 坑 7 | 重写理由:从"`StateComponent` 不知道回合"改为"持续时间是 Provider 的事,不是 `StateComponent` 的事" |
| `Doc/decision-guide.md` 决策 2 | 重构为"`StateComponent` 和'持续回合的 buff'是什么关系",明确 Provider 模式 |
| `Doc/rpg/05-status.md` | **删除**,替换为两个新文件 |
| `Doc/rpg/05-state-component.md` | **新增** —— 专门讲 StateComponent 本身的设计骨架 |
| `Doc/rpg/06-buff-system.md` | **新增** —— 专门讲 buff 作为典型 Provider 的完整实现 |
| `Doc/rpg/06/07/08` → `07/08/09` | 重编号 + 内容加交叉引用 |
| `Doc/rpg/README.md` | 更新章节列表、决策原则、映射表 |
| `Doc/rpg/CHANGELOG.md` | 记录此次架构修正 |
| `Doc/README.md` | RPG 索引小节维持不变(已经指向 rpg/) |

**沉淀原则强化**(本次修正):

| 沉淀 | 不沉淀 |
|------|--------|
| ✅ StateComponent 是底层基础设施(不等于 buff) | ❌ 把 buff 效果塞进 StateComponent |
| ✅ buff = Provider 的一种(自己持有 duration / tick / dispellable) | ❌ 持续时间塞进 StateComponent |
| ✅ Provider 自己决定何时 AddState / RemoveState | ❌ StateComponent 自己持有时间 |
| ✅ 业务层"回合开始"事件遍历所有活跃 Provider | ❌ 遍历 StateComponent 找持续时间 |

---

## 2026-06-08 · 初始沉淀

**触发事件**:完成"基于 HoweFramework 的回合制 RPG 战斗系统渐进式教程"研究(详见 `20260608-111523-howegame-rpg/turn_001/`),交付 `final.md`(1413 行 / ~13000 字 / 64KB)。沉淀出框架分析层文档。

**沉淀产物**:

| 文件 | 来源研究产物 | 主要内容 |
|------|--------------|---------|
| `README.md` | 新写 | 文档索引 + 三句话总结 + 怎么用 |
| `framework-analysis.md` | `background.md` §1.1-1.3,1.6;`document.md` §1 | 4 层架构、20 模块分类、"Gameplay"模块归属澄清 |
| `primitives.md` | `background.md` §1.4-1.5;`document.md` §1.1-1.3 | 6 内置组件 + 7 内置 Manager + 跨模块工具目录 |
| `decision-guide.md` | `document.md` §4,§11.2;`analysis.md` §1.2-1.7 | 13 个最常碰到的"二选一"决策树 |
| `extension-points.md` | `document.md` §1.4-1.5,§6,§7;`analysis.md` §1.11 | 何时新增组件 / Manager / Context / Procedure / BT 节点 |
| `gotchas.md` | `background.md` §1.4.5,§3;`document.md` §5 | 13 条常见坑 + 版本冲突清单 + 未决清单 |
| `naming-conventions.md` | `CLAUDE.md` 命名规范节 | 强制命名规则 |
| `CHANGELOG.md` | 本文件 | 沉淀记录 |

**沉淀原则**:
- **不**包含具体游戏(回合制 RPG)的数据表(阵法数值 / 门派列表 / 技能公式)—— 这些跟着业务代码走,不污染框架层。
- **不**包含完整 API 参考 —— 那些在 `Client/Assets/HoweFramework/Doc/` 里。
- **聚焦**决策模式、扩展点、常见坑 —— 让 Agent 在新游戏里也能用。

**统计**:

| 指标 | 数值 |
|------|------|
| 研究产物总大小 | ~380 KB(`background.md` 47 KB + `document.md` 103 KB + `analysis.md` 63 KB + `research_plan.md` 83 KB + `judgment.md` 20 KB + 其他) |
| 沉淀后 Doc 目录总大小 | ~50 KB |
| 沉淀比例 | ~13%(其余 87% 是具体游戏的业务分析,不进 Doc) |

---

## 维护约定

**何时更新 `Doc/`**:
- 框架原语(6 组件 / 7 Manager)有变动 → 更新 `primitives.md`。
- 命名规范(`CLAUDE.md`)有变动 → 更新 `naming-conventions.md`。
- 新增业务类型支持(非 RPG)→ 在 `primitives.md` 加对照表。
- 发现新的常见坑 → 在 `gotchas.md` 加条目。
- `Doc/` 自身结构有变化 → 更新 `README.md`。

**何时**不**更新**:
- 业务层(RPG / SLG / FPS 等)的具体数据 / 表 / 逻辑 → 不进这目录。
- 单纯 bug fix → 不进这目录,只在代码注释里。
- 框架代码自身的小改动 → 不进这目录,只在 `Assets/HoweFramework/` 里。

**格式**:
- 每条变更记一行(日期 / 触发事件 / 沉淀产物 / 沉淀原则)。
- 涉及文件改动的要列"沉淀产物"表格。

---

## 沉淀历史索引

| 日期 | 沉淀事件 | 涉及文件 |
|------|---------|---------|
| 2026-06-08 | 初始沉淀(回合制 RPG 研究) | 全部 8 个文件 |

(后续按日期继续追加。)