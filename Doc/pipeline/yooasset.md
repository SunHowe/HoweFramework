# YooAsset

## 职责

`ResModule.UseYooAsset()` 把核心加载器设为 `YooAssetResLoader`。初始化模式是另一组扩展：`InitYooAssetEditorSimulateMode`、`InitYooAssetHostPlayMode` 等（`YooAssetExtensions`）。

编辑器收集器：`HoweFramework.Editor/YooAsset/CollectTextAsset.cs`、`CollectTexture.cs`。

## 用法

业务只 `CreateResLoader` 加载资源。打包与清单在 YooAsset 窗口/收集器配置，不要在运行时直接 `YooAssets.Load*` 绕过 `IResLoader`。

## 约束与坑

- `UseYooAsset` 不等于已经 Init 包；启动流程里要调对应 `InitYooAsset*`。
- 局部 loader 必须 Dispose。
- 默认包名见 `YooAssetResLoader.DefaultPackageName`。

## 相关源码

- `Client/Assets/HoweFramework/Extensions/YooAssetExtensions.cs`
- `Client/Assets/HoweFramework/Res/YooAsset/YooAssetResLoader.cs`
- `Client/Assets/HoweFramework.Editor/YooAsset/`
