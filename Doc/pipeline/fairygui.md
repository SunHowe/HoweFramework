# FairyGUI

## 职责

界面源工程只在 `FGUIProject/`（`FGUIProject.fairy`，`type="Unity"`）。Headless 创建 / 编辑 / 发布走 `Tools/fgui`（[OpenFairyGUI](https://github.com/OpenFairyGUI/OpenFairyGUI) npm 包，锁定 0.3.x）。运行时由 `UIModule` + `FairyGUIFormLogicBase` 打开。C# 绑定仍由 Unity 编辑器生成，不用 OpenFairyGUI 自带 codegen。

## 目录

| 路径 | 用途 |
|------|------|
| `FGUIProject/` | 唯一 `.fairy` 源工程。包和组件都加在这里，不要另建工程 |
| `FGUIProject/settings/Publish.json` | 编辑器发布设置；输出已指向 `../Client/Assets/GameMain/UI`，扩展名 `bytes` |
| `Tools/fgui/` | 项目 CLI、组合 MCP、`publish.sh` |
| `Tools/fgui/mcp.json` | Cursor MCP 配置样例（`.cursor` 被 gitignore，把内容拷进本机 Cursor MCP） |
| `Client/Assets/GameMain/UI/` | 发布产物 `*_fui.bytes` 与 `MainPackageMapping.asset` |
| `Client/Assets/GameMain/Scripts/UI/` | 生成绑定、`UIFormId`、业务 `*FormLogic` |
| `Client/Scriban/GameMain/` | Unity 绑定 / 逻辑桩模板 |
| `Client/Assets/HoweFramework.Editor/FairyGUI/` | 导入监听、代码生成（默认不改） |
| `Client/Assets/HoweFramework/UI/FairyGUI/` | 运行时接入（默认不改） |

## 用法

一次性：`cd Tools/fgui && npm install`（Node.js >= 20）。

### CLI

命令固定打 `FGUIProject/`，发布固定写 `Client/Assets/GameMain/UI`：

```bash
node Tools/fgui/bin/fgui.mjs inspect
node Tools/fgui/bin/fgui.mjs validate
node Tools/fgui/bin/fgui.mjs publish
# 或
Tools/fgui/publish.sh
```

加 `--json` 可拿机器可读输出。`validate`：`valid` 退出 0，`invalid` 退出 1，其它非完整结果退出 2。

### Agent：创建 / 编辑 / 发布

1. 配置 Cursor MCP：把 [`Tools/fgui/mcp.json`](../../Tools/fgui/mcp.json) 拷进本机 MCP 设置。仓库根为工作区时 args 为 `Tools/fgui/bin/fgui.mjs`；若工作区是 `Client/`，改成 `../Tools/fgui/bin/fgui.mjs`。
2. 先读 prompt `howe_fgui_pipeline`。
3. `openfairygui_backend_open_session`，`projectPath` 用 `FGUIProject/FGUIProject.fairy`（允许根只有这一棵工程目录）。
4. `get_project_outline` / `get_capabilities`，再 `apply_transaction`（带 `expectedRevision`）建包、加组件、改控件。
5. 界面组件名必须 `*Form` 且 exported；可复用控件 `*Component`。不要新建第二个 `.fairy`。
6. `save_session` 写回 XML。若返回 `uam_fidelity_unsupported`，停止，不要强行覆盖。
7. `howe_fgui_validate_project` → `howe_fgui_publish_project`（或 CLI `publish`）。
8. 打开 Unity（已打开时 `FairyGUIAssetsImporter` 会监听到 `_fui.bytes`）。菜单 `Game Framework/FairyGUI/Generate Code` 可手动重跑。生成 `UIFormId`、`*.Designer.cs`；逻辑桩 `GameMain/Scripts/UI/{Package}/{Name}.cs` 只在文件不存在时生成。
9. 只改 `GameMain` 逻辑桩（`OnOpen` 等），用 `UIModule.OpenUIForm(UIFormId.Xxx)`。不要直接操作 `UIPackage`。

UAM 覆盖常见控件、布局、attach/detach、controller、transition 和已建模 gear，不是官方编辑器的全功能面板。官方 FairyGUI 编辑器仍可改同一 `FGUIProject`。

### 运行时

包路径格式：`Assets/GameMain/UI/{0}_fui.bytes`（`FairyGUISettings.UIPackagePathFormat`）。YooAsset 已收集 `Assets/GameMain/UI`。启动后 `LoadFairyGUIPackagesAsync(UIConst.UIPackageMappingAssetPath)`。

## 扩展点

- 新界面：在现有工程加包/组件 → 发布 → Unity 生成绑定 → 填 `GameMain` 逻辑。
- 逻辑基类：`FullScreenFormLogicBase`、`PopupFormLogicBase`、`MainFormLogicBase`、`FixedFormLogicBase`。
- 不要用 OpenFairyGUI codegen 替换 Scriban 绑定，也不要把 `restore` 当常规创作路径。

## 约束与坑

- 只允许打开 `FGUIProject/`。组合 MCP 把 `OPENFAIRYGUI_ALLOWED_PROJECT_ROOTS` 钉在该目录。
- 官方编辑器写入 OpenFairyGUI 尚未建模的属性后，`save_session` 会拒绝写回（`uam_fidelity_unsupported`），不会静默覆盖。
- 发布覆盖 `Client/Assets/GameMain/UI` 下的包 bytes；映射资产 `MainPackageMapping.asset` 仍由 Unity 生成。
- 空工程可以 inspect / validate / publish，但不会写出 `*_fui.bytes`，直到至少有一个包。
- OpenFairyGUI session 锁文件（`*.openfairygui.backend.lock`）已 gitignore，不要提交。
- 未明确要求时不要改 `HoweFramework` / `HoweFramework.Editor`。

## 相关源码

- `FGUIProject/FGUIProject.fairy`
- `FGUIProject/settings/Publish.json`
- `Tools/fgui/bin/fgui.mjs`
- `Tools/fgui/src/mcp-server.mjs`
- `Client/Assets/HoweFramework.Editor/FairyGUI/FairyGUIAssetsImporter.cs`
- `Client/Assets/HoweFramework.Editor/FairyGUI/FairyGUICodeGenerator.cs`
- `Client/Assets/HoweFramework/UI/FairyGUI/FairyGUISettings.cs`
- `Client/Assets/GameMain/Scripts/UI/`
