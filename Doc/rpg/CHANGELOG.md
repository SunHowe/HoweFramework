# RPG 沉淀记录(Changelog)

> `Doc/rpg/` 目录自身的变更记录。每改 Doc 都要在这记一笔。

---

## 2026-06-08 (2) · 架构修正:StateComponent ≠ buff,buff = Provider

**触发事件**:用户指出之前沉淀的 `Doc/rpg/05-status.md` 和框架层 `Doc/primitives.md` 的 1.5 StateComponent 章节存在概念错误 —— `StateComponent` **不**等于 buff,它是玩法底层的"集合"基础设施;buff 是 **Provider** 的一种(自己持有 duration / tick / dispellable / source / level)。

**修正内容**:

| 位置 | 改动 |
|------|------|
| `Doc/primitives.md` 1.5 | 重写 StateComponent 描述:从"buff 风格的引用计数布尔状态"改为"底层基础设施 / 集合机制 / 不持有时间效果驱散";明确"Provider = 状态来源 / 主人" |
| `Doc/gotchas.md` 坑 2 | 重写为"`StateComponent` 不是 buff —— 是底层'集合'基础设施",加坑 2.5 详述 Provider 模式 vs "一切皆状态"反模式 |
| `Doc/gotchas.md` 坑 7 | 重写理由:从"`StateComponent` 不知道回合"改为"持续时间是 Provider 的事,不是 `StateComponent` 的事" |
| `Doc/decision-guide.md` 决策 2 | 重构为"`StateComponent` 和'持续回合的 buff'是什么关系",明确 Provider 模式 |
| `Doc/rpg/05-status.md` | **删除**,替换为两个新文件 |
| `Doc/rpg/05-state-component.md` | **新增** —— 专门讲 StateComponent 本身的设计骨架(底层基础设施 / 集合机制 / Provider 模式) |
| `Doc/rpg/06-buff-system.md` | **新增** —— 专门讲 buff 作为典型 Provider 的完整实现(duration / tick / dispellable / source / level / 池化)|
| `Doc/rpg/06-pet-and-formation.md` → `07-pet-and-formation.md` | 重命名 + 内容加交叉引用 |
| `Doc/rpg/07-anger-and-items.md` → `08-anger-and-items.md` | 重命名 + 内容加交叉引用 |
| `Doc/rpg/08-rules-and-edge-cases.md` → `09-rules-and-edge-cases.md` | 重命名 + 内容加交叉引用 |
| `Doc/rpg/README.md` | 更新章节列表、决策原则、映射表 |
| `Doc/CHANGELOG.md` | 记录此次架构修正 |

**沉淀原则**(本次修正强化):

| 沉淀 | 不沉淀 |
|------|--------|
| ✅ StateComponent 是底层基础设施(不等于 buff) | ❌ 把 buff 效果塞进 StateComponent |
| ✅ buff = Provider 的一种(自己持有 duration / tick / dispellable) | ❌ 持续时间塞进 StateComponent |
| ✅ Provider 自己决定何时 AddState / RemoveState | ❌ StateComponent 自己持有时间 |
| ✅ 业务层"回合开始"事件遍历所有活跃 Provider | ❌ 遍历 StateComponent 找持续时间 |

---

## 2026-06-08 · 初始沉淀

**触发事件**:完成"HoweFramework 框架分析"沉淀(见 `../CHANGELOG.md`),沉淀出 RPG 类型项目的设计层文档。

**沉淀产物**(8 个文档 / ~80 KB):

| 文件 | 来源研究产物 | 主要内容 |
|------|--------------|---------|
| `README.md` | 新写 | RPG 文档索引、3 个决策原则、8 个章节导航 |
| `01-genre-and-mode.md` | `document.md` §4.4,§4.5,§5.5;`background.md` §2.10 | RPG 4 大子类型 + 半自动/全自动模式 + 决策密度设计 |
| `02-battle-loop.md` | `document.md` §4.5-§4.10,§5;`background.md` §2.3,§2.10 | 战斗循环 FSM + 半自动指令 + 出手顺序 + 胜负判定 + 行为树 AI |
| `03-attributes.md` | `document.md` §1.4;`background.md` §2.6 | 8 项基础属性 + 6 项高级属性 + 双路攻击 + 链式公式 + HP/MP 链式 |
| `04-skills.md` | `document.md` §1.5;`background.md` §2.5,§2.6,§2.9 | 6 类技能 + 数据结构 + 目标选择 + 公式字符串 + 释放流程 |
| ~~`05-status.md`~~ | ~~(已废弃,见 2026-06-08 (2) 修正)~~ | ~~4 类状态 + 引用计数 + 持续回合 + 倒地/死亡/鬼魂~~ |
| ~~`06-pet-and-formation.md`~~ | ~~(已重命名为 07-pet-and-formation.md)~~ | ~~召唤兽 4 大要素 + 参战副本 + 阵法 = 站位 + 相克~~ |
| ~~`07-anger-and-items.md`~~ | ~~(已重命名为 08-anger-and-items.md)~~ | ~~愤怒分段加怒 + 特技 + 药品系统 + 三者协同~~ |
| ~~`08-rules-and-edge-cases.md`~~ | ~~(已重命名为 09-rules-and-edge-cases.md)~~ | ~~通用规则 + HP=0 三态 + 掉线 + 录像回放 + MMORPG 网络~~ |

**沉淀原则**(对应 README 中的"不沉淀什么"):

| 沉淀 | 不沉淀 |
|------|--------|
| ✅ 设计模式(RPG 通用) | ❌ 具体游戏数值(梦幻西游阵法表 / 门派列表 / 技能公式) |
| ✅ 设计意图(为什么这样做) | ❌ 具体游戏版本(端游 / 手游 / 2026 资料片)|
| ✅ 决策维度(8 项基础属性 / 6 类技能 / 4 类状态) | ❌ 具体平衡数值(横扫 3 刀衰减系数 / 推气 4 回合)|
| ✅ 实现要点(框架原语映射) | ❌ 具体游戏平衡(化生寺治疗暴击 +18%) |
| ✅ 常见反模式 | ❌ 具体商业化设计(装备词条 / 稀有度)|

---

## 设计骨架沉淀统计

沉淀的不是"怎么做"的具体数值,而是"为什么这样设计"的设计骨架:

| 维度 | 沉淀的设计骨架 |
|------|----------------|
| **战斗循环** | FSM 阶段设计 + 半自动指令 + 出手顺序 + 胜负判定 |
| **属性体系** | 8+6 属性分类 + 双路攻击 + 链式公式 + HP/MP 链式 |
| **技能系统** | 6 类分类 + 数据结构 + 目标选择 + 公式字符串 |
| **StateComponent(底层基础设施)** | 集合机制 / (stateId, provider) 引用计数 / 不持有时间效果 |
| **buff(典型 Provider)** | 4 类分类(增益 / 减益 / 控制 / 特殊)+ 自己持有 duration / tick / dispellable + 倒地/死亡/鬼魂 三态 |
| **召唤兽** | 4 大要素 + 参战副本 + AI 决策 |
| **阵法** | 站位 + 加成 + 相克 = `FinalPercent` 修改 |
| **愤怒 / 特技 / 药品** | 分段加怒 + 6 类特技 + 限制次数 + 三者协同 |
| **规则边界** | HP=0 三态 + 掉线 + 录像回放 + MMORPG 网络 |

---

## 维护约定

**何时更新 `Doc/rpg/`**:
- 发现新的 RPG 通用设计模式 → 新增 / 修订章节。
- 框架原语有变动(影响 RPG 映射)→ 更新 `README.md` 的"跟框架的关系"映射表。
- RPG 设计模式的新变体(比如新发现的设计模式)→ 在对应章节追加。

**何时**不**更新**:
- 业务层(梦幻西游 / 某 RPG 产品)的具体数据 → 不进这目录。
- 单纯 bug fix → 不进这目录。
- 框架代码自身的小改动 → 不进这目录,只在 `Assets/HoweFramework/` 里。
- 某款具体游戏的新版本(端游 / 手游更新)→ 不进这目录。

**格式**:
- 每条变更记一行(日期 / 触发事件 / 沉淀产物 / 沉淀原则)。
- 涉及文件改动的要列"沉淀产物"表格。

---

## 沉淀历史索引

| 日期 | 沉淀事件 | 涉及文件 |
|------|---------|---------|
| 2026-06-08 | 初始沉淀(回合制 RPG 研究) | 全部 9 个文件(8 章节 + 1 索引 + 1 CHANGELOG = 10 个)|

(后续按日期继续追加。)