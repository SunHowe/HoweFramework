# ResModule

## 职责

不提供「用完就扔」的全局加载器。核心 `IResLoader` 由辅助器设置；业务通过 `CreateResLoader` 拿**局部代理**，用完 `Dispose`。

## 关键类型

- `ResModule`：`SetResCoreLoader`、`CreateResLoader`、`UnloadUnusedAsset`
- `IResLoader`：`LoadAssetAsync`、`LoadBinary`/`LoadBinaryAsync`、`UnloadAsset`、`UnloadUnusedAsset`、场景加载/卸载/状态查询
- `YooAssetExtensions.UseYooAsset`：`GameApp` 已调用；另有 EditorSimulate / Host / Offline 等初始化扩展

未设核心加载器：`ResCoreLoaderNotSet`。加载器已释放：`ResLoaderDisposed`。

## 用法

```csharp
using var loader = ResModule.Instance.CreateResLoader();
var asset = await loader.LoadAssetAsync(key, typeof(GameObject));
```

玩法上下文默认 `UseManagedResLoader()`，随 Context `Dispose` 释放。

## 扩展点

`SetResCoreLoader` 可换实现。场景模块用 `GetResCoreLoader()` 加载场景，不要再包一层长期泄漏的 loader。

## 约束与坑

- 局部 loader 是 `ResProxyLoader`，Dispose 才会卸资源。
- 不要长期持有核心加载器当业务缓存。
- YooAsset 初始化是独立步骤，与 `UseYooAsset` 设核心加载器分开。

## 相关源码

- `Client/Assets/HoweFramework/Res/ResModule.cs`
- `Client/Assets/HoweFramework/Res/Core/IResLoader.cs`
- `Client/Assets/HoweFramework/Extensions/YooAssetExtensions.cs`
- `Client/Assets/HoweFramework/Res/YooAsset/YooAssetResLoader.cs`
