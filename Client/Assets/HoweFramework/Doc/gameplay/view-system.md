# 视图系统

## 概述

视图系统将 Unity GameObject（表现层）与 Entity（逻辑层）分离，实现关注点分离。

## 核心文件

| 文件 | 路径 |
|------|------|
| ViewComponent | `Gameplay/Common/View/ViewComponent.cs` |
| ViewTransformSyncComponent | `Gameplay/Common/View/ViewTransformSyncComponent.cs` |
| IGameViewManager | `Gameplay/Framework/View/IGameViewManager.cs` |
| GameViewManager | `Gameplay/Framework/View/GameViewManager.cs` |
| IViewObject | `Gameplay/Framework/View/IViewObject.cs` |
| ViewComponentConverter | `Gameplay/Mono/ViewComponentConverter.cs` |

## IViewObject

```csharp
public interface IViewObject
{
    // 资源键
    string ResKey { get; }

    // Unity 对象
    GameObject GameObject { get; }
    Transform Transform { get; }

    // 属性
    Vector3 Position { get; set; }
    Vector3 EulerAngles { get; set; }
    Vector3 Scale { get; set; }
    bool IsVisible { get; set; }
    bool IsLoaded { get; }

    // 加载/卸载
    UniTask LoadGameObject(string resKey);
    void SetGameObject(GameObject gameObject);
    void UnloadGameObject();

    // 生命周期
    void Show();
    void Hide();
    void AttachTo(Transform parent);
}
```

## ViewComponent

```csharp
[GameComponent(GameComponentType.View)]
public sealed class ViewComponent : GameComponentBase
{
    // 资源路径
    public string ResKey { get; set; }

    // 父 Transform
    public Transform ViewParent { get; set; }

    // 变换属性（与 TransformComponent 配合）
    public Vector3 Position { get; set; }
    public Vector3 EulerAngles { get; set; }
    public Vector3 Scale { get; set; }

    // 可见性
    public bool IsVisible { get; set; }

    // 加载状态
    public bool IsLoaded { get; }

    // Blackboard（传递给 Unity 组件）
    public IBlackboard Blackboard { get; }

    // 事件
    public event Action OnViewLoaded;
    public event Action OnViewUnloaded;
}
```

## ViewTransformSyncComponent

自动将 TransformComponent 的值同步到 ViewComponent：

```csharp
[GameComponent(GameComponentType.ViewTransformSync)]
public sealed class ViewTransformSyncComponent : GameComponentBase
{
    // 暂停同步
    public bool IsPause { get; set; }

    // 插值设置
    public bool IsLerpPosition { get; set; }
    public bool IsLerpEulerAngles { get; set; }
    public bool IsLerpScale { get; set; }

    // 强制立即同步
    public void ForceSync();
}
```

## IGameViewManager

```csharp
public interface IGameViewManager : IGameManager
{
    // 视图根对象
    Transform ViewRoot { get; }

    // 创建/销毁视图对象
    IViewObject SpawnViewObject();
    void DisposeViewObject(IViewObject viewObject);
}
```

## 基本用法

### 1. 实体添加视图组件

```csharp
var entity = entityManager.CreateEntity();

// 添加 Transform 组件
var transform = entity.AddComponent<TransformComponent>();
transform.Position = new Vector3(0, 0, 0);

// 添加 View 组件
var view = entity.AddComponent<ViewComponent>();
view.ResKey = "Prefabs/Player";
view.ViewParent = viewRoot;

// 添加同步组件
entity.AddComponent<ViewTransformSyncComponent>();
```

### 2. 监听加载完成

```csharp
view.OnViewLoaded += () =>
{
    Debug.Log("View loaded!");
};
```

### 3. 控制可见性

```csharp
view.IsVisible = false; // 隐藏
view.IsVisible = true;  // 显示
```

### 4. 设置变换

```csharp
view.Position = new Vector3(10, 0, 0);
view.EulerAngles = new Vector3(0, 90, 0);
view.Scale = new Vector3(2, 2, 2);
```

## MonoConverter 工作流程

将 Unity GameObject 转换为 Entity 的组件：

### 1. GameEntityConverter

添加到 Prefab 上，将 GameObject 转换为实体：

```csharp
// 在 Prefab 上添加 GameEntityConverter
public class GameEntityConverter : MonoBehaviour
{
    // 指定组件转换器的执行顺序
    public int SortingOrder = 0;

    // 调用此方法将 GameObject 转换为实体
    public void ConvertEntity(IGameEntity entity);
}
```

### 2. 组件转换器

每个转换器负责添加特定组件：

```csharp
public interface IGameComponentConverter
{
    int SortingOrder { get; }
    void Convert(GameObject go, IGameEntity entity);
}
```

### 3. TransformComponentConverter

```csharp
[GameComponent(GameComponentType.Transform)]
public sealed class TransformComponentConverter : MonoBehaviour, IGameComponentConverter
{
    public int SortingOrder => 0;

    public void Convert(GameObject go, IGameEntity entity)
    {
        var transform = entity.AddComponent<TransformComponent>();
        transform.Position = go.transform.position;
        transform.Rotation = go.transform.rotation;
        transform.Scale = go.transform.localScale;
    }
}
```

### 4. ViewComponentConverter

```csharp
public sealed class ViewComponentConverter : MonoBehaviour, IGameComponentConverter
{
    public int SortingOrder => 1;

    [SerializeField] private string _resKey;
    [SerializeField] private Transform _viewParent;

    public void Convert(GameObject go, IGameEntity entity)
    {
        var view = entity.AddComponent<ViewComponent>();
        view.ResKey = _resKey;
        view.ViewParent = _viewParent ?? go.transform.parent;
    }
}
```

## Prefab 转实体流程

```
Prefab (with Converters)
    ↓
GameEntityConverter.ConvertEntity(entity)
    ↓
按 SortingOrder 排序所有 IGameComponentConverter
    ↓
按顺序执行每个 Converter
    ↓
Converter.Convert(go, entity)
    ↓
Entity.AddComponent<T>() 添加组件
    ↓
组件初始化完成
```

## 完整示例

### 创建 Prefab

1. 创建 Prefab `Player.prefab`
2. 添加 `GameEntityConverter` 组件
3. 添加 `TransformComponentConverter`
4. 添加 `ViewComponentConverter`，设置 `ResKey = "Prefabs/Player"`
5. 添加其他业务组件的 Converter

### 代码中实例化

```csharp
public async UniTask<IGameEntity> CreatePlayerAsync(Vector3 position)
{
    var entity = _entityManager.CreateEntity();

    // 添加视图组件
    var view = entity.AddComponent<ViewComponent>();
    view.ResKey = "Prefabs/Player";
    view.Position = position;

    // 添加变换同步
    entity.AddComponent<ViewTransformSyncComponent>();

    // 监听加载完成
    await view.OnViewLoadedAsync();

    return entity;
}
```

## 视图生命周期

```
SpawnViewObject()
    ↓
ViewComponent 添加到 Entity
    ↓
ViewComponent.OnViewLoaded 事件
    ↓
ViewTransformSync 开始同步
    ↓
... 使用中 ...
    ↓
Entity.Dispose()
    ↓
ViewComponent.OnViewUnloaded 事件
    ↓
DisposeViewObject()
```

## 最佳实践

### 1. 使用异步加载

```csharp
public async UniTask<IGameEntity> CreateEntityAsync(string prefab)
{
    var view = entity.AddComponent<ViewComponent>();
    view.ResKey = prefab;

    await view.OnViewLoadedAsync(); // 等待加载完成

    return entity;
}
```

### 2. 设置合适的 ViewParent

```csharp
// 将所有角色放入同一个父对象
view.ViewParent = _characterRoot;

// 将 UI 元素放入 Canvas
view.ViewParent = _uiRoot;
```

### 3. 控制同步

```csharp
var sync = entity.GetComponent<ViewTransformSyncComponent>();
sync.IsLerpPosition = true;  // 位置插值
sync.IsLerpEulerAngles = false; // 旋转不插值
```

### 4. 批量实例化

```csharp
public async UniTask SpawnEnemiesAsync(int count)
{
    var tasks = new List<UniTask>();
    for (int i = 0; i < count; i++)
    {
        tasks.Add(CreateEnemyAsync());
    }
    await UniTask.WhenAll(tasks);
}
```