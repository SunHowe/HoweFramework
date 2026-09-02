# Luban 配表

## 职责

用 Luban 从 `DataTable/` 生成客户端二进制与 C#。

## 目录

| 路径 | 用途 |
|------|------|
| `DataTable/luban.conf` | schema：`Defines`、`Datas/__tables__.xlsx` 等；client target 的 manager 为 `GameMainDataTableSource`，`topModule` 为 `GameMain` |
| `DataTable/gen_client.sh` / `.bat` | 客户端表 |
| `DataTable/gen_client_localization.sh` / `.bat` | 本地化 |
| `Tools/Luban/` | `Luban.dll` |
| `Tools/LubanClientTemplate/` | 自定义模板（脚本 `--customTemplateDir`） |
| `Tools/LubanProject/` | Luban 源工程 |
| `Client/Assets/GameMain/DataTable/` | 输出数据 |
| `Client/Assets/GameMain/Scripts/DataTable/AutoGen/` | 输出代码 |

`gen_client.sh`：`dotnet Tools/Luban/Luban.dll -t client -c cs-bin -d bin`。

## 用法

改 Defines/Datas 后在 `DataTable/` 跑 `gen_client.sh`，不要手改 AutoGen。运行时由 `DataTableModule` + `IDataTableSource` 加载，见 [`../framework/modules/data-table.md`](../framework/modules/data-table.md)。

## 约束与坑

- 当前仓库里 `DataTable/` 可能只有 conf 与脚本、没有完整 xlsx；缺表时生成会失败，这是源数据问题不是模块 API 问题。
- 本地化走单独 conf（`luban_localization.conf`）。

## 相关源码

- `DataTable/gen_client.sh`
- `DataTable/luban.conf`
- `Client/Assets/HoweFramework/DataTable/DataTableModule.cs`
