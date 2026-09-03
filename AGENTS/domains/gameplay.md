# Gameplay 域

玩法 EC 源码在 **`Client/Assets/GameMain/Scripts/Gameplay/`**，不是 `HoweFramework`。

知识：[`Doc/gameplay/`](../../Doc/gameplay/README.md)。扩展边界：[`Doc/conventions/extension.md`](../../Doc/conventions/extension.md)。

## 能改什么

| 位置 | 用途 |
|------|------|
| `Gameplay/Framework/` | Context、Entity、内置 Manager、事件、表达式 |
| `Gameplay/Common/` | 通用组件（Transform / View / Numeric / State / Resource / Ability） |
| `Gameplay/Mono/` | 场景物体 → 实体的转换器 |
| `GameMain/Scripts/` 下业务目录 | 具体玩法组件与 Manager |

未要求重构框架时，优先加业务组件 / Manager，而不是改 `Framework/` 内核。

## 约定

- 组件：`*Component`，继承 `GameComponentBase`（以源码基类名为准）。`ComponentType` 用 `TypeId`，不要再加 Attribute 或组件枚举。
- 管理器：`*Manager`，实现现有 Manager 基类与 `I*Manager : IGameManager`。`ManagerType` 用接口的 `TypeId`，不要再加 Attribute 或 Manager 枚举。
- 实体与组件通过 Context / EntityManager 创建，不要绕过管理器直接 new 长期存活对象却不走回收。
- 数值、状态、视图的职责拆分见 `Doc/gameplay/` 各篇，不要用一个组件包办。

## 不要

- 不要把 Gameplay 类型搬进 `HoweFramework` 程序集。
- 不要引入第二套 EC 或依赖 Unity `MonoBehaviour` 当实体组件（场景侧只用 Converter 桥接）。
- 不要在 `Gameplay/Doc/` 写手册正文。
