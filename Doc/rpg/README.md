# 通用 RPG 项目设计指导

> 沉淀自 2026-06 HoweFramework 框架分析研究。
> **面向未来 Agent 开发任何 RPG 项目** —— 不含具体游戏(梦幻西游 / 某 RPG 产品)的数值、门派、技能表 —— 只沉淀**通用 RPG 设计模式 + 设计意图**。

---

## 这目录是干嘛的

`Doc/rpg/` 沉淀的是 **RPG 类型项目的设计层共识** —— 你做回合制 RPG / SRPG / MMORPG 都能用,核心设计模式不变。具体游戏的数值(梦幻西游阵法数值表、门派列表、技能公式)不在这里 —— 那些跟着各自的业务代码走,不该污染设计层。

未来 Agent 在做下列事情**之前**,应该先读这个目录:

| 我要做什么 | 先读哪一篇 |
|------------|-----------|
| 我第一次做 RPG,不知道从哪儿下手 | [`01-genre-and-mode.md`](01-genre-and-mode.md) |
| 我要设计战斗循环 / 回合机制 | [`02-battle-loop.md`](02-battle-loop.md) |
| 我要设计属性体系(HP / MP / 攻击 / 防御 / 速度 …)| [`03-attributes.md`](03-attributes.md) |
| 我要设计技能系统(物理 / 法术 / 治疗 …) | [`04-skills.md`](04-skills.md) |
| 我要设计 buff / 状态 / 控制 | [`05-status.md`](05-status.md) |
| 我要做召唤兽 / 宠物系统 | [`06-pet-and-formation.md`](06-pet-and-formation.md) |
| 我要做阵法 / 站位系统 | [`06-pet-and-formation.md`](06-pet-and-formation.md) |
| 我要做愤怒 / 特技 / 药品系统 | [`07-anger-and-items.md`](07-anger-and-items.md) |
| 我要处理倒地 / 死亡 / 掉线 / 超时 等边界 | [`08-rules-and-edge-cases.md`](08-rules-and-edge-cases.md) |

---

## 文档清单

| 文件 | 用途 |
|------|------|
| [`01-genre-and-mode.md`](01-genre-and-mode.md) | RPG 子类型(回合制 / SRPG / ARPG / MMORPG) + 半自动 vs 全自动模式 |
| [`02-battle-loop.md`](02-battle-loop.md) | 战斗循环 + 回合机制 + 出手顺序规则 |
| [`03-attributes.md`](03-attributes.md) | 属性体系:基础 8 项 + 高级 6 项 + 治疗强度独立的设计意图 |
| [`04-skills.md`](04-skills.md) | 技能系统:6 类技能(物理 / 法术 / 封印 / 治疗 / 防御 / 复活)+ 目标选择 + 伤害公式 |
| [`05-status.md`](05-status.md) | 状态系统:4 类(增益 / 减益 / 控制 / 特殊)+ 持续回合 + 引用计数语义 |
| [`06-pet-and-formation.md`](06-pet-and-formation.md) | 召唤兽(资质 / 成长 / 技能 / 内丹)+ 阵法(站位 + 相克)|
| [`07-anger-and-items.md`](07-anger-and-items.md) | 愤怒 / 特技 / 药品系统的设计意图 |
| [`08-rules-and-edge-cases.md`](08-rules-and-edge-cases.md) | 通用规则 + 边界情况(倒地 / 死亡 / 鬼魂 / 掉线 / 超时 / 回合上限)|

---

## 不沉淀什么(明确隔离)

下列内容**不**沉淀到这目录(跟着业务代码走,不该污染设计层):

- ❌ 任何具体游戏的**数值表**(梦幻西游阵法数值、门派列表、技能公式具体数值)
- ❌ 任何具体游戏的**门派特色数值**(化生寺治疗暴击 +18%、龙宫法术命中率 100% 等)
- ❌ 任何具体游戏的**平衡性数据**(横扫三刀衰减系数、推气过宫 4 回合持续等)
- ❌ 任何具体游戏的**版本**(端游 vs 手游 vs 2026 资料片"弥勒山")
- ❌ 任何具体游戏的**商业化设计**(装备词条、稀有度、强化)

沉淀的只有**设计模式** —— 你做新的 RPG 项目也能套的"为什么这样设计"。

---

## 怎么用这个目录

### 场景 A:你是新来的 Agent,要开发一款 RPG

1. 先读 [`01-genre-and-mode.md`](01-genre-and-mode.md) —— 5 分钟明确你的项目是回合制 / 半自动 / 全自动。
2. 翻 [`02-battle-loop.md`](02-battle-loop.md) —— 把战斗循环的 FSM 设计落到框架。
3. 按需读其他章节(每章独立可读)。
4. 决策树见 [`Doc/decision-guide.md`](../decision-guide.md)(框架层 + RPG 层双重决策)。

### 场景 B:你做的是 RPG,但类型跟梦幻西游不一样

- 做 ARPG / MMORPG:仍然能参考,但要看每章开头的"本模式适用"段。
- 做卡牌 RPG / 二次元 RPG / 战棋 SRPG:核心模式通用,但细节(速度排序、半自动指令窗口、站位阵法)需要做变体。

### 场景 C:你做的是 SRPG(策略 RPG / 战棋)

- 通用战斗循环适用,但"30 秒指令窗口"会变成"无限时间,玩家主动结束"。
- 站位/阵法系统会更重要(每个角色独立站位)。
- 召唤兽可能变成"召唤"技能(精灵召唤)而非"参战宠物"。

---

## 设计原则(贯穿所有 RPG 模式)

### 原则 1:**链式反应优于手工同步**

HP 上限绑基础属性 → 装备变 → HP 上限自动重算 → 满血时自动填满。**单一事实来源**(single source of truth)。

### 原则 2:**引用计数优于手工状态**

buff 多源叠加 → 用引用计数语义;**最后一个 provider Remove 才真正消失**。比手写 `Dictionary<int, int>` 少 80% 边界 case。

### 原则 3:**公式字符串优于硬编码 if-else**

策划改 `.json` 改公式,不改代码 → `ExpressionManager` 派上用场。

### 原则 4:**双路攻击优于单路合并**

"物理伤害" 和 "法术伤害" 是两路独立属性,不要合并成"攻击力"。详见 [`03-attributes.md`](03-attributes.md) §"为什么需要双路攻击"。

### 原则 5:**状态(buff / debuff / 控制 / 特殊)优于"分类型组件"**

所有"在 / 不在"语义统一用 `StateComponent` + `stateId`,不要为每种 buff 写一个 bool 字段。

### 原则 6:**半自动指令 + 现实时间计时,优于全自动 + 帧累计**

让玩家有"被时钟追着跑"的紧张感,这是回合制 RPG 的核心体验。

### 原则 7:**倒地 ≠ 死亡 ≠ 鬼魂**

- 玩家 HP=0 → **倒地**(本场可救起,战斗结束未救起才真死)
- NPC HP=0 → **死亡**(当回合不可再出战)
- 鬼魂系 → 可复活次数限制

### 原则 8:**绝对时间计时优于帧累计**

`Time.timeScale=0` 玩家暂停游戏,计时也不能停。**走绝对秒数**。

---

## 跟框架的关系

- **这目录沉淀的是 RPG 设计模式**(业务层模式)
- **上层 [`Doc/`](../README.md) 沉淀的是框架原语**(技术层能力)
- 两层配合使用:用框架原语实现 RPG 模式,不要自己造轮子

**映射速查**:

| RPG 概念 | 框架原语 | 详见 |
|---------|----------|------|
| HP / MP / 法力 | `ResourceComponent` + `BindNumericMax` | [`03-attributes.md`](03-attributes.md)|
| 等级 / 攻击 / 防御 / 速度 | `NumericComponent`(百分比 + 固定加值)| [`03-attributes.md`](03-attributes.md)|
| 中毒 / 封印 / 隐身 | `StateComponent`(引用计数)| [`05-status.md`](05-status.md)|
| 战斗循环 / 回合阶段 | `FsmMachine` | [`02-battle-loop.md`](02-battle-loop.md)|
| 应用级流程(主城 ↔ 战斗)| `ProcedureModule` | [`02-battle-loop.md`](02-battle-loop.md) §"应用流程"|
| 怪物 AI | `BehaviorTree` | [`02-battle-loop.md`](02-battle-loop.md) §"AI 设计"|
| 30s 倒计时 / 持续 N 回合 | `GameTimerManager`(绝对时间)| [`02-battle-loop.md`](02-battle-loop.md) §"半自动指令"|
| 行动条 UI | `GameUpdateManager` | [`02-battle-loop.md`](02-battle-loop.md) §"行动条"|
| 伤害 / 经验 / 升级公式 | `ExpressionManager` | [`04-skills.md`](04-skills.md) §"伤害公式"|
| 跨实体持有目标 | `GameEntityRef` | [`08-rules-and-edge-cases.md`](08-rules-and-edge-cases.md) §"安全引用"|

---

## 维护约定

**何时更新 `Doc/rpg/`**:
- 发现新的 RPG 通用设计模式 → 新增 / 修订章节。
- 框架原语有变动(影响 RPG 映射) → 更新"跟框架的关系"映射表。
- 边界情况(倒地 / 死亡 / 掉线)有新的处理范式 → 更新 `08`。

**何时**不**更新**:
- 业务层(梦幻西游 / 某 RPG 产品)的数据 → 不进这目录。
- 单纯 bug fix → 不进这目录。
- 框架代码自身的小改动 → 不进这目录,只在 `Assets/HoweFramework/` 里。

---

## 参考来源

沉淀自:
- 2026-06 HoweFramework 框架分析研究(`20260608-111523-howegame-rpg/`)
- 多款经典回合制 RPG / SRPG / MMORPG 的设计模式整合
- 不指向任何具体游戏的数值,只沉淀"为什么这样设计"

具体引用见各章节末尾的"参考来源"。