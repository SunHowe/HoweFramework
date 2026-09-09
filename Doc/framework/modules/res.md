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
- **失败语义**：`LoadAssetAsync` 加载失败返回 null（不抛异常），且失败结果不缓存——下次调用同 key 会重新发起加载（可重试）。调用方按 null 即失败处理。
- 同一 key 只允许一种资源类型：以不同类型重复加载同 key 会直接抛 `InvalidParam`（缓存按 key 聚合，类型错配会污染引用计数）。
- 场景加载/卸载失败会清理句柄登记，可安全重试；场景卸载中禁止并发加载同一场景（`ResSceneUnloading`）。
- `SetResCoreLoader` 覆盖旧加载器前会先 `Dispose` 旧实例，重复设置不会泄漏句柄。

## 相关源码

- `Client/Assets/HoweFramework/Res/ResModule.cs`
- `Client/Assets/HoweFramework/Res/Core/IResLoader.cs`
- `Client/Assets/HoweFramework/Extensions/YooAssetExtensions.cs`
- `Client/Assets/HoweFramework/Res/YooAsset/YooAssetResLoader.cs`
