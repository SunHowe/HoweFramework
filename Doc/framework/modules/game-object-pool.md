# GameObjectPoolModule

## 职责

全局对象池 + 创建局部池。局部池用完必须 `Dispose`。

## 关键类型

- `GameObjectPoolModule`：`CreateGameObjectPool`、`InstantiateAsync`、`PreloadAsync`、`Release`、`SetCacheCountLimit`、`ClearCache`/`ClearAllCache`
- `IGameObjectPool`

`CreateGameObjectPool(null)` 会分配新的资源加载器（见实现 `GameObjectPool.Create`）。Context 的 `UseManagedGameObjectPool` 会用上下文的 `ResLoader` 建池并随 Context 释放。

## 用法

```csharp
var go = await GameObjectPoolModule.Instance.InstantiateAsync(assetKey);
GameObjectPoolModule.Instance.Release(go);

var pool = GameObjectPoolModule.Instance.CreateGameObjectPool(resLoader);
// 用完
pool.Dispose();
```

## 扩展点

限缓存数量、预加载。玩法用 Context 托管池，不要再挂一个忘 Dispose 的局部池。

## 约束与坑

- 模块级 Instantiate 走内部全局池；Destroy 模块时会释放该池。
- 局部池不 Dispose 会拖住 ResLoader。

## 相关源码

- `Client/Assets/HoweFramework/GameObjectPool/GameObjectPoolModule.cs`
- `Client/Assets/HoweFramework/GameObjectPool/IGameObjectPool.cs`
