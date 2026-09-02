# SafeAreaModule

## 职责

提供 `SafeArea` 矩形；变化时派发 `SafeAreaChangeEventArgs`。

## 关键类型

- `SafeAreaModule.SafeArea`
- `ISafeAreaHelper`：Editor 用 `DebuggableSafeAreaHelper`，运行时 `UnitySafeAreaHelper`

`OnUpdate` 转给 Helper，便于 Editor 调试拖安全区。

## 用法

```csharp
var rect = SafeAreaModule.Instance.SafeArea;
EventModule.Instance.Subscribe(SafeAreaChangeEventArgs.EventId, OnSafeArea);
```

UI 适配应订阅事件，不要只在打开界面时读一次（旋转/刘海变化）。

## 扩展点

旧设备 Unity API 不准时可实现 `ISafeAreaHelper`（当前模块在 `OnInit` 写死默认 Helper，改扩展方式需动框架）。

## 约束与坑

- Editor 与真机 Helper 不同。
- 模块销毁会 `Dispose` Helper。

## 相关源码

- `Client/Assets/HoweFramework/SafeArea/SafeAreaModule.cs`
