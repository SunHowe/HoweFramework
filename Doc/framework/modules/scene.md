# SceneModule

## 职责

经核心 `IResLoader` 加载/卸载场景，维护顺序并刷新 ActiveScene。框架场景与业务场景分开记录。

## 关键类型

- `SceneModule`：`LoadSceneAsync`、`UnloadSceneAsync`、`SetSceneOrder`
- 场景状态错误码：`ResSceneAlreadyLoaded`、`ResSceneLoading`、`ResSceneNotLoad` 等（201 段）

加载走 `ResModule.Instance.GetResCoreLoader().LoadScene`。未登记顺序时默认 order 0。空名字 `SetSceneOrder` 只打日志。

## 用法

```csharp
await SceneModule.Instance.LoadSceneAsync("MyScene");
SceneModule.Instance.SetSceneOrder("MyScene", 10);
await SceneModule.Instance.UnloadSceneAsync("MyScene");
```

## 扩展点

多场景叠加时用 order 决定谁成为 Active。玩法内另有 `GameSceneManager`，职责是玩法场景对象，不是 Unity Scene。

## 约束与坑

- 依赖 Res 核心加载器已设置。
- 卸载会从 order 表移除。

## 相关源码

- `Client/Assets/HoweFramework/Scene/SceneModule.cs`
