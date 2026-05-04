# 场景管理

## 概述

SceneModule 管理 Unity 场景的异步加载和卸载。

## 核心文件

| 文件 | 路径 |
|------|------|
| SceneModule | `Assets/HoweFramework/Scene/SceneModule.cs` |

## SceneModule API

```csharp
public sealed class SceneModule : ModuleBase<SceneModule>
{
    // 加载场景
    public UniTask LoadSceneAsync(string sceneAssetName);

    // 卸载场景
    public UniTask UnloadSceneAsync(string sceneAssetName);

    // 设置场景顺序
    public void SetSceneOrder(string sceneAssetName, int sceneOrder);

    // 检查状态
    public bool SceneIsLoaded(string sceneAssetName);
    public bool SceneIsLoading(string sceneAssetName);
    public bool SceneIsUnloading(string sceneAssetName);

    // 工具方法
    public static string GetSceneName(string sceneAssetName);
}
```

## 基本用法

### 1. 加载场景

```csharp
// 异步加载场景
await SceneModule.Instance.LoadSceneAsync("Scenes/Gameplay");

// 加载完成后自动切换到新场景
```

### 2. 卸载场景

```csharp
// 卸载不需要的场景
await SceneModule.Instance.UnloadSceneAsync("Scenes/Menu");
```

### 3. 检查状态

```csharp
if (SceneModule.Instance.SceneIsLoading("Scenes/Gameplay"))
{
    Debug.Log("Still loading...");
}

// 加载完成
if (SceneModule.Instance.SceneIsLoaded("Scenes/Gameplay"))
{
    Debug.Log("Loaded!");
}
```

### 4. 设置加载优先级

```csharp
// 设置场景加载顺序
SceneModule.Instance.SetSceneOrder("Scenes/Gameplay", 1);
SceneModule.Instance.SetSceneOrder("Scenes/UI", 2);
```

## 完整示例

### 场景切换

```csharp
public class SceneTransition
{
    public async UniTask TransitionToGameplayAsync()
    {
        // 显示加载界面
        await UIModule.Instance.OpenUIForm((int)UIFormId.LoadingScreen);

        // 加载游戏场景
        await SceneModule.Instance.LoadSceneAsync("Scenes/Gameplay");

        // 卸载主菜单
        if (SceneModule.Instance.SceneIsLoaded("Scenes/Menu"))
        {
            await SceneModule.Instance.UnloadSceneAsync("Scenes/Menu");
        }

        // 关闭加载界面
        await UIModule.Instance.CloseUIForm((int)UIFormId.LoadingScreen);
    }
}
```

### 进度监控

```csharp
public class LoadingScreen : FullScreenFormLogicBase
{
    private float _loadingProgress;

    public override void OnOpen()
    {
        // 启动进度更新
        TimerModule.Instance.AddFrameTimer(UpdateProgress);
    }

    private void UpdateProgress()
    {
        // 更新进度条
        // 注意：SceneModule 不直接提供进度回调
        // 可以通过异步操作的状态来估算
    }
}
```

## 注意事项

- 场景名称应与 Unity 中定义的场景名称一致
- 加载场景会自动激活新场景
- 卸载场景会移除场景中的所有 GameObject