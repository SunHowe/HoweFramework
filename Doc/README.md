# HoweFramework 框架分析文档

> 面向**未来 Agent 开发其他类型游戏**(回合制 RPG / SLG / FPS / 卡牌 / 模拟经营……)。
> 来源:2026-06 HoweFramework 框架分析沉淀,涵盖项目框架原语、决策模式、扩展点、常见坑。

---

## 这目录是干嘛的

`Doc/` 沉淀的是**框架层分析** —— 你做哪种游戏都能复用。不含任何具体游戏(回合制 RPG)的玩法数据表(那些数据表属于业务,应该跟着各自的业务代码走,不该污染框架层)。

未来 Agent 在做下列事情**之前**,应该先读这个目录:

| 我要做什么 | 先读哪一篇 |
|------------|-----------|
| 我是第一次接触这个框架,想知道能用什么 | [`framework-analysis.md`](framework-analysis.md) |
| 我有具体需求(HP / 状态 / 属性 / AI / 计时 / 流程),不知道该用哪个原语 | [`primitives.md`](primitives.md) |
| 我要在几个候选方案里选一个(X 还是 Y) | [`decision-guide.md`](decision-guide.md) |
| 我要新增一个组件 / Manager | [`extension-points.md`](extension-points.md) |
| 我写的代码遇到奇怪 bug,框架给的报错不像报错 | [`gotchas.md`](gotchas.md) |
| 我要决定类名 / 文件名怎么起 | [`naming-conventions.md`](naming-conventions.md) |

---

## 文档清单

| 文件 | 用途 | 大小 |
|------|------|------|
| [`framework-analysis.md`](framework-analysis.md) | 框架能力总览 —— 4 层架构、20 个模块的分类、"Gameplay 是业务层不是框架核心"的关键澄清 | ~3 KB |
| [`primitives.md`](primitives.md) | 原语目录 —— 6 内置组件 + 7 内置 Manager + 跨模块工具的速查表 | ~12 KB |
| [`decision-guide.md`](decision-guide.md) | 决策模式 —— 用 X 还是 Y、什么时候用框架 vs 什么时候自写 | ~10 KB |
| [`extension-points.md`](extension-points.md) | 扩展点 —— 何时新增组件 / Manager / 上下文,继承关系怎么搭 | ~6 KB |
| [`gotchas.md`](gotchas.md) | 常见坑 —— 13 条概念误读、版本冲突、未结清单 | ~8 KB |
| [`naming-conventions.md`](naming-conventions.md) | 命名规范 —— 来自 `Client/CLAUDE.md` 的强制命名规则 | ~2 KB |
| [`CHANGELOG.md`](CHANGELOG.md) | 沉淀记录 —— 这些分析是怎么一步步沉淀下来的 | ~1 KB |

### 按"游戏类型"分目录沉淀(业务层模式)

> 上面 7 篇是**框架层**(技术能力),下面按游戏类型沉淀的子目录是**业务层**(设计模式)。

| 子目录 | 用途 |
|--------|------|
| [`rpg/`](rpg/README.md) | **RPG 类型项目设计骨架**(子类型分类、战斗循环、属性、技能、状态、召唤兽、阵法、愤怒 / 特技 / 药品、规则边界)—— 不含具体游戏数值 |

---

## 三句话总结 HoweFramework

1. **它是一个 Unity3D 客户端框架**(基于 GameFramework / YooAsset / Luban / UniTask / FairyGUI 拼装的工程化骨架)。
2. **它有 4 层结构**:`HoweFramework`(框架核心)+ `HoweFramework.Editor`(编辑器工具)+ `GameMain`(业务层,含 Gameplay 玩法框架)+ Server / GeekServer(可选)。
3. **"Gameplay"模块归属有坑**:根 `README.md` 把 Gameplay 列在框架模块表里,**但实际目录在业务层 `Assets/GameMain/Scripts/Gameplay/`**。以 `Client/CLAUDE.md` 与 `Assets/HoweFramework/Doc/gameplay/README.md` 为准。

---

## 怎么用这个目录

### 场景 A:你是新来的 Agent,要在这个项目里加一个新功能(任何类型游戏)

1. 先读 [`framework-analysis.md`](framework-analysis.md) —— 5 分钟了解项目能给你什么。
2. 把你需求拆成"原语选择"问题:HP 用什么?状态用什么?AI 用什么?计时用什么?
3. 对照 [`primitives.md`](primitives.md) 的目录速查表找答案。
4. 如果同时有几个候选,翻 [`decision-guide.md`](decision-guide.md) 的决策树。
5. 决定"要新增组件 / Manager"前,先读 [`extension-points.md`](extension-points.md) 看现有原语能不能拼。
6. 写代码前扫一眼 [`gotchas.md`](gotchas.md) 的"易踩坑清单"。

### 场景 B:你写的代码有 bug,框架给的报错信息很奇怪

→ 直接看 [`gotchas.md`](gotchas.md)。

### 场景 C:你不知道类应该叫什么名字

→ [`naming-conventions.md`](naming-conventions.md)。

---

## 不在这个目录里的东西

- **具体游戏的玩法数据表**(梦幻西游阵法数值、门派列表、技能公式) —— 这些跟着业务代码走,不污染框架层。
- **完整 API 参考** —— 在 `Client/Assets/HoweFramework/Doc/` 里(模块级 + Gameplay 级两个 Doc 目录)。
- **Unity 基础 / C# 语法 / FairyGUI 基础用法** —— 不教,自己看官方文档。

---

## 维护约定

- 这目录沉淀的是**框架层共识**,业务层代码改动**不应**回写到这。
- 新增框架原语 / 重命名模块 / 改 GameContextBase 生命周期时,来这里同步更新。
- 业务层 Agent(写 RPG / SLG / FPS 的)读这目录就够了,不该再读 380KB 研究原文。