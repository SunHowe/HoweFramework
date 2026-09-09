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

- Editor 与真机 Helper 不同，但两者产出的 `SafeArea` 语义一致：物理像素、左下原点（与 `Screen.safeArea` 相同）。Editor 调试器（`DebuggableSafeAreaHelper`）的四个偏移量中 Top/Bottom 分别对应屏幕顶部/底部 inset。
- **FairyGUI 适配坐标换算**：`SafeArea` 是物理像素、左下原点；FairyGUI 是逻辑坐标、左上原点。框架适配器（`FairyGUISafeAreaAdaptor`/`FairyGUIFullScreenAdaptor`）内部统一经 `ConvertSafeAreaToUICoordinates` 做 Y 翻转与缩放换算，业务直接使用即可，不要自行把 `SafeArea` 的 xy 当 UI 坐标。
- 模块销毁会 `Dispose` Helper。

## 相关源码

- `Client/Assets/HoweFramework/SafeArea/SafeAreaModule.cs`
