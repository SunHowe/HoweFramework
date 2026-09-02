# DataTableModule

## 职责

挂接一个或多个 `IDataTableSource`，按 `DataTableLoadMode` 加载 Luban 生成表。

## 关键类型

- `DataTableModule`：`LoadMode`（默认 `LazyLoadAndPreloadAsync`）、`AddDataTableSource`、`RemoveDataTableSource`、`Preload` / `PreloadAsync`
- `DataTableLoadMode`：`AsyncLoad`、`SyncLoad`、`LazyLoad`、`LazyLoadAndPreloadAsync`、`LazyLoadAndPreloadSync`
- 业务源：`GameMainDataTableSource`（Luban 生成 + 手写部分在 `GameMain/Scripts/DataTable/`）

`AddDataTableSource` 时立刻 `dataTableSource.Init(LoadMode)`。模块销毁会 `Dispose` 所有源。

## 用法

启动流程 `ProcedureLoadDataTable` 负责接入与预加载。改表走 `DataTable/` 与生成脚本，见 [`../../pipeline/luban.md`](../../pipeline/luban.md)。

## 扩展点

`IDataTableSource` 可接非 Luban 源。加载模式要在加源之前设好。

## 约束与坑

- 不要手改 `Scripts/DataTable/AutoGen/`。
- 源加进去后 `LoadMode` 再改不会自动重新 Init。

## 相关源码

- `Client/Assets/HoweFramework/DataTable/DataTableModule.cs`
- `Client/Assets/HoweFramework/DataTable/DataTableLoadMode.cs`
- `Client/Assets/GameMain/Scripts/DataTable/`
