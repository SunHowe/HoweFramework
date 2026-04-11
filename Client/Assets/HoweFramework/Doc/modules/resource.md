# 资源系统

## 概述

ResModule 管理游戏资源的加载和卸载，基于 YooAsset 实现。

## 核心文件

| 文件 | 路径 |
|------|------|
| ResModule | `Assets/HoweFramework/Res/ResModule.cs` |
| IResLoader | `Assets/HoweFramework/Res/Core/IResLoader.cs` |
| YooAssetResLoader | `Assets/HoweFramework/Res/YooAsset/YooAssetResLoader.cs` |
| ResProxyLoader | `Assets/HoweFramework/Res/Core/ResProxyLoader.cs` |

## IResLoader 接口

```csharp
public interface IResLoader : IDisposable
{
    // 加载资源
    UniTask<Object> LoadAssetAsync(string assetKey, Type assetType, CancellationToken token = default);
    UniTask<byte[]> LoadBinaryAsync(string assetKey, CancellationToken token = default);
    byte[] LoadBinary(string assetKey);

    // 卸载资源
    void UnloadAsset(string assetKey);
    void UnloadUnusedAsset();

    // 场景操作
    UniTask<Scene> LoadScene(string sceneAssetName);
    UniTask UnloadScene(string sceneAssetName);
    bool SceneIsLoaded(string sceneAssetName);
    bool SceneIsLoading(string sceneAssetName);
    bool SceneIsUnloading(string sceneAssetName);
}
```

## ResModule API

```csharp
public sealed class ResModule : ModuleBase<ResModule>
{
    // 设置核心加载器
    public void SetResCoreLoader(IResLoader resLoader);

    // 创建新的加载器（需要手动释放）
    public IResLoader CreateResLoader(IResLoader resLoader = null);

    // 卸载未使用资源
    public void UnloadUnusedAsset();
}
```

## ResLoaderExtensions

```csharp
// 泛型扩展方法
public static UniTask<T> LoadAssetAsync<T>(this IResLoader resLoader, string assetKey, CancellationToken token = default) where T : Object;
```

## YooAssetResLoader

基于 YooAsset 的资源加载器实现：

```csharp
public sealed class YooAssetResLoader : IResLoader
{
    public const string DefaultPackageName = "DefaultPackage";

    // 初始化
    public async UniTask InitResourcePackageAsync(InitializeParameters parameters);

    // 异步加载资源
    public async UniTask<UnityEngine.Object> LoadAssetAsync(string assetKey, Type assetType, CancellationToken token = default);

    // 同步加载二进制
    public byte[] LoadBinary(string assetKey);

    // 异步加载二进制
    public UniTask<byte[]> LoadBinaryAsync(string assetKey, CancellationToken token = default);

    // 卸载资源
    public void UnloadAsset(string assetKey);

    // 卸载未使用资源
    public void UnloadUnusedAsset();
}
```

## ResProxyLoader

代理加载器，提供引用计数功能：

```csharp
public sealed class ResProxyLoader : IResLoader
{
    // 构造时指定父加载器
    public ResProxyLoader(IResLoader parent);

    // 引用计数 +1
    public void Retain();

    // 引用计数 -1，计数为 0 时真正卸载
    public void Release();

    // 引用计数为 0 时真正卸载
    public void ForceRelease();
}
```

## 基本用法

### 1. 初始化（通常在 ProcedureSplash）

```csharp
// 在流程初始化中
var resLoader = new YooAssetResLoader();
await resLoader.InitResourcePackageAsync(new InitializeParameters());
ResModule.Instance.SetResCoreLoader(resLoader);
```

### 2. 创建加载器（推荐方式）

```csharp
// 创建一个加载器，使用完毕后释放
var loader = ResModule.Instance.CreateResLoader();

// 加载资源
var prefab = await loader.LoadAssetAsync<GameObject>("Prefabs/Player");
var sprite = await loader.LoadAssetAsync<Sprite>("UI/Sprites/icon");
var bytes = await loader.LoadBinaryAsync("Data/config");

// 使用完毕后释放
loader.Dispose();
```

### 3. 直接加载

```csharp
// 同步加载二进制
var bytes = ResModule.Instance.Get<IResLoader>().LoadBinary("Data/config");

// 异步加载（通过 GameApp 获取默认加载器）
var resLoader = new YooAssetResLoader();
ResModule.Instance.SetResCoreLoader(resLoader);
var texture = await resLoader.LoadAssetAsync<Texture2D>("Textures/bg");
```

## 场景加载

```csharp
// 加载场景
await ResModule.Instance.LoadSceneAsync("Scenes/Gameplay");

// 卸载场景
await ResModule.Instance.UnloadSceneAsync("Scenes/Gameplay");

// 检查状态
bool isLoaded = ResModule.Instance.SceneIsLoaded("Scenes/Gameplay");
bool isLoading = ResModule.Instance.SceneIsLoading("Scenes/Gameplay");
```

## 最佳实践

### 1. 使用 CreateResLoader 管理生命周期

```csharp
public async UniTask LoadLevelAsync(string levelName)
{
    var loader = ResModule.Instance.CreateResLoader();
    try
    {
        // 加载场景
        await SceneManager.LoadSceneAsync(levelName);

        // 加载依赖资源
        var prefab = await loader.LoadAssetAsync<GameObject>($"Levels/{levelName}/Player");
        var textures = await loader.LoadAssetAsync<Texture2D[]>($"Levels/{levelName}/Textures");

        // 实例化
        Instantiate(prefab);
    }
    finally
    {
        loader.Dispose();
    }
}
```

### 2. 资源键命名规范

```csharp
// 推荐：使用完整路径
await loader.LoadAssetAsync<GameObject>("Prefabs/Enemies/Slime");
await loader.LoadAssetAsync<Sprite>("UI/Sprites/Icons/health_icon");

// 不推荐：使用简短名称
await loader.LoadAssetAsync<GameObject>("slime");
```

### 3. 及时卸载不需要的资源

```csharp
// 卸载单个资源
loader.UnloadAsset("Prefabs/Temp");

// 卸载所有未使用资源（消耗较大，适量使用）
ResModule.Instance.UnloadUnusedAsset();
```

### 4. 使用 ReferencePool 管理加载的数据

```csharp
public class LevelData : IReference
{
    public string Name { get; set; }
    public GameObject[] Prefabs { get; set; }

    public void Clear()
    {
        Name = null;
        Prefabs = null;
    }
}

public async UniTask<LevelData> LoadLevelDataAsync(string levelName)
{
    var loader = ResModule.Instance.CreateResLoader();
    var data = ReferencePool.Acquire<LevelData>();

    try
    {
        data.Name = levelName;
        data.Prefabs = await loader.LoadAssetAsync<GameObject[]>($"Levels/{levelName}/Prefabs");
        return data;
    }
    finally
    {
        loader.Dispose();
    }
}
```

## YooAsset 初始化参数

```csharp
// 编辑器模拟模式
var editorParams = new EditorSimulateParameters();

// WebGL 平台
var webGLParams = new WebGLPlayModeParameters();

// 离线模式
var offlineParams = new OfflinePlayModeParameters();

// 初始化
await resLoader.InitResourcePackageAsync(offlineParams);
```

## 与其他模块配合

### 与 GameObjectPoolModule 配合

```csharp
// 预加载到对象池
await GameObjectPoolModule.Instance.PreloadAsync("Prefabs/Bullet", 20);

// 从对象池获取
var bullet = await GameObjectPoolModule.Instance.InstantiateAsync("Prefabs/Bullet");

// 归还到对象池
GameObjectPoolModule.Instance.Release(bullet);
```

### 与 UIModule 配合

```csharp
// UI 通常使用 ResModule 加载 FairyGUI Package
// 在 ProcedureSplash 中已完成初始化
```