# 管线域

配表、UI 工程、资源打包与运行时加载是三条分开的管线。知识：[`Doc/pipeline/`](../../Doc/pipeline/README.md)。

## Luban / 配表

| 改什么 | 去哪 |
|--------|------|
| 表结构、字段 | `DataTable/` 的 Defines |
| 表数据 | `DataTable/` 的 Datas |
| 生成脚本与模板 | `Tools/`、`DataTable/gen_client*.sh` |
| 运行时加载 | `HoweFramework/DataTable/`（框架，默认不改） |

改表后走项目已有生成脚本，再在客户端验证加载。不要手改生成代码冒充出表。详见 [`Doc/pipeline/luban.md`](../../Doc/pipeline/luban.md)。

## FairyGUI

界面源工程只在 `FGUIProject/`。创建 / 编辑 / 发布走 `Tools/fgui`（OpenFairyGUI CLI 与组合 MCP）或官方 FairyGUI 编辑器；不要另建 `.fairy`。C# 绑定仍由 Unity `HoweFramework.Editor/FairyGUI/` 生成。业务逻辑只进 `GameMain/Scripts/UI/`。详见 [`Doc/pipeline/fairygui.md`](../../Doc/pipeline/fairygui.md)。

## YooAsset

运行时入口是 `ResModule.UseYooAsset()`（`GameApp` 已接）。收集器与编辑器扩展在 `HoweFramework.Editor/YooAsset/`。加载必须经 `IResLoader` / 模块 API，用完释放加载器。详见 [`Doc/pipeline/yooasset.md`](../../Doc/pipeline/yooasset.md)。

## 沉淀

管线步骤（命令、目录、生成物位置）变了，更新 `Doc/pipeline/` 对应篇，不要只写在聊天里。
