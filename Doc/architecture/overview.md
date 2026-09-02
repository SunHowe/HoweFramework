# 仓库分层

HoweFramework 是 Unity 客户端骨架：框架模块提供全局服务，业务在 `GameMain`，玩法 EC 也在 `GameMain`（不是框架程序集）。

## 顶层目录

| 路径 | 角色 |
|------|------|
| `Client/` | Unity 工程（`Assets/`、`Packages/`、`ProjectSettings/`） |
| `DataTable/` | Luban 客户端表配置与生成脚本 |
| `Tools/` | Luban 工具链与客户端模板 |
| `FGUIProject/` | FairyGUI 编辑器工程 |
| `Server/` | 自研服务端（以磁盘实际内容为准） |
| `GeekServer/` | 参考服务端 |
| `AGENTS/` | Agent 怎么干活 |
| `Doc/` | 项目知识 |

## 客户端三层

```
Client/Assets/
├── HoweFramework/           # 运行时框架，程序集 HoweFramework
├── HoweFramework.Editor/    # 编辑器扩展
└── GameMain/                # 业务：流程、UI、System、Gameplay、生成表代码
```

| 层 | 程序集 / 命名空间 | 默认谁改 |
|----|-------------------|----------|
| 框架 | `HoweFramework` | 未明确要求则不要改 |
| 编辑器 | `HoweFramework.Editor` | 未明确要求则不要改 |
| 业务 | `GameMain`、`GameMain.UI` 等 | 默认改这里 |

Gameplay 命名空间是 `GameMain`，源码在 `Client/Assets/GameMain/Scripts/Gameplay/`。不要把玩法类型放进 `HoweFramework`。

## 依赖方向

业务可以调用框架模块（`*Module.Instance` 或 IOC 注入）。框架不引用 `GameMain`。

外部组件（以根 README 与 `Packages/manifest.json` 为准）：FairyGUI、YooAsset、Luban、UniTask 等。资源加载走 `ResModule` + YooAsset，配表走 Luban 生成物 + `DataTableModule`，UI 走 `UIModule` + FairyGUI。

## 相关源码

- `Client/Assets/HoweFramework/GameApp.cs`
- `Client/Assets/GameMain/Scripts/GameEntry.cs`
- `Client/Assets/HoweFramework/HoweFramework.asmdef`
