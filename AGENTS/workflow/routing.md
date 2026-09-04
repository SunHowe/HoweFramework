# 任务路由

接到任务后先定域，再打开对应源码与 [`Doc/`](../../Doc/README.md)。不要凭经验跨层乱改。

## 怎么定域

| 任务像什么 | 域 | 源码 | 必读 |
|------------|----|------|------|
| 框架模块、ModuleBase、IOC、事件、资源、UI 运行时 | 客户端框架 | `Client/Assets/HoweFramework/` | [`domains/client.md`](../domains/client.md)、[`Doc/framework/`](../../Doc/framework/README.md) |
| 编辑器工具、FairyGUI 导入、YooAsset 收集器、行为树图 | 编辑器 | `Client/Assets/HoweFramework.Editor/` | [`Doc/framework/editor.md`](../../Doc/framework/editor.md) |
| 流程、登录、业务 UI、业务 System | 客户端业务 | `Client/Assets/GameMain/` | [`domains/client.md`](../domains/client.md) |
| 实体、组件、Manager、数值/状态/视图 | Gameplay | `Client/Assets/GameMain/Scripts/Gameplay/` | [`domains/gameplay.md`](../domains/gameplay.md)、[`Doc/gameplay/`](../../Doc/gameplay/README.md) |
| 配表、Luban、生成代码 | 管线 | `DataTable/`、`Tools/` | [`domains/pipeline.md`](../domains/pipeline.md)、[`Doc/pipeline/luban.md`](../../Doc/pipeline/luban.md) |
| FairyGUI 包、界面资源 | 管线 | `FGUIProject/`、`Tools/fgui/` | [`domains/pipeline.md`](../domains/pipeline.md)、[`Doc/pipeline/fairygui.md`](../../Doc/pipeline/fairygui.md) |
| 资源打包、加载器 | 管线 | YooAsset + `ResModule` | [`Doc/pipeline/yooasset.md`](../../Doc/pipeline/yooasset.md) |
| 网关、玩法服、协议 | 服务端 | `Server/`、参考 `GeekServer/` | [`domains/server.md`](../domains/server.md)、[`Doc/server/`](../../Doc/server/README.md) |
| Agent 流程、索引、沉淀规则 | 本目录 | `AGENTS/` | 改完更新 AGENTS 索引 |
| 架构说明、模块手册、规范 | 知识 | `Doc/` | [`index-maintenance.md`](index-maintenance.md) |

简单查找（「这个类型在哪」）可以直接搜源码并回答，不必改 Doc。一旦开始改代码、改约定或做出取舍，收尾走 [`distill.md`](distill.md)。

## 边界

- **Gameplay 源码在 `GameMain`，不在 `HoweFramework`。** 不要把玩法 EC 写进框架程序集。
- 未明确要求时，不要改 `HoweFramework` / `HoweFramework.Editor`。
- 配表改 `DataTable/` 源表，不要手改生成物充数（生成物以管线文档为准）。
- 服务端以仓库里**实际存在**的工程为准，不要假设 `GeekServer/` 可直接当生产服。
