# 安全区域系统

## 概述

SafeAreaModule 提供设备安全区域（刘海屏、挖孔屏等）的实时检测。

## 核心文件

| 文件 | 路径 |
|------|------|
| SafeAreaModule | `Assets/HoweFramework/SafeArea/SafeAreaModule.cs` |

## SafeAreaModule API

```csharp
public sealed class SafeAreaModule : ModuleBase<SafeAreaModule>
{
    // 当前安全区域（自动更新）
    public Rect SafeArea { get; }
}
```

## 事件

| 事件 | 说明 |
|------|------|
| `SafeAreaChangeEventArgs` | 安全区域变化时触发 |

## 基本用法

### 1. 获取安全区域

```csharp
Rect safeArea = SafeAreaModule.Instance.SafeArea;
```

### 2. 订阅变化事件

```csharp
EventModule.Instance.Subscribe(SafeAreaChangeEventArgs.EventId, OnSafeAreaChanged);

private void OnSafeAreaChanged(object sender, GameEventArgs e)
{
    var args = (SafeAreaChangeEventArgs)e;
    Debug.Log($"Safe area changed: {args.SafeArea}");
}
```

## SafeAreaChangeEventArgs

```csharp
public sealed class SafeAreaChangeEventArgs : GameEventArgs
{
    public static readonly int EventId = typeof(SafeAreaChangeEventArgs).GetHashCode();
    public override int Id => EventId;

    public Rect SafeArea { get; set; }

    public override void Clear()
    {
        SafeArea = default;
    }
}
```

## 应用示例

### 全屏 UI 适配

```csharp
public class SafeAreaFitter : MonoBehaviour
{
    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    private void OnEnable()
    {
        EventModule.Instance.Subscribe(SafeAreaChangeEventArgs.EventId, OnSafeAreaChanged);
    }

    private void OnDisable()
    {
        EventModule.Instance.Unsubscribe(SafeAreaChangeEventArgs.EventId, OnSafeAreaChanged);
    }

    private void OnSafeAreaChanged(object sender, GameEventArgs e)
    {
        ApplySafeArea();
    }

    private void ApplySafeArea()
    {
        var safeArea = SafeAreaModule.Instance.SafeArea;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        _rect.anchorMin = anchorMin;
        _rect.anchorMax = anchorMax;
    }
}
```

## 内部实现

### UnitySafeAreaHelper

```csharp
// 在真机上使用 UnityEngine.Screen.safeArea
private sealed class UnitySafeAreaHelper : ISafeAreaHelper
{
    public Rect GetSafeArea()
    {
        return Screen.safeArea;
    }
}
```

### DebuggableSafeAreaHelper

```csharp
// 在编辑器中使用模拟值
private sealed class DebuggableSafeAreaHelper : ISafeAreaHelper
{
    public Rect GetSafeArea()
    {
        // 返回编辑器模拟的安全区域
        return new Rect(0, 0, Screen.width, Screen.height);
    }
}
```

## 最佳实践

### 1. 组件化适配逻辑

```csharp
// 将适配逻辑封装为组件
public class SafeAreaAdapter : MonoBehaviour
{
    [SerializeField] private bool _applyTop = true;
    [SerializeField] private bool _applyBottom = true;

    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        ApplySafeArea();
        EventModule.Instance.Subscribe(SafeAreaChangeEventArgs.EventId, OnSafeAreaChanged);
    }

    private void OnDestroy()
    {
        EventModule.Instance.Unsubscribe(SafeAreaChangeEventArgs.EventId, OnSafeAreaChanged);
    }

    private void OnSafeAreaChanged(object sender, GameEventArgs e)
    {
        ApplySafeArea();
    }

    private void ApplySafeArea()
    {
        var safeArea = SafeAreaModule.Instance.SafeArea;
        var width = Screen.width;
        var height = Screen.height;

        Vector2 minAnchor = new Vector2(
            _applyBottom ? safeArea.x / width : 0,
            _applyBottom ? safeArea.y / height : 0
        );

        Vector2 maxAnchor = new Vector2(
            _applyTop ? (safeArea.xMax / width) : 1,
            _applyTop ? (safeArea.yMax / height) : 1
        );

        _rect.anchorMin = minAnchor;
        _rect.anchorMax = maxAnchor;
    }
}
```

### 2. 处理横屏和竖屏

```csharp
private Rect GetSafeArea()
{
    var safeArea = SafeAreaModule.Instance.SafeArea;

    // 横屏时可能需要交换
    if (Screen.width > Screen.height)
    {
        // 横屏安全区域
        return safeArea;
    }
    else
    {
        // 竖屏安全区域
        return safeArea;
    }
}
```

### 3. 使用 inset 而非 padding

```csharp
// 使用 anchor 而非 position
_rect.anchorMin = safeArea.position / new Vector2(Screen.width, Screen.height);
_rect.anchorMax = safeArea.position + safeArea.size / new Vector2(Screen.width, Screen.height);
```