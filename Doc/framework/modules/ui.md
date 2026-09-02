# UIModule

## 职责

按分组管理界面实例、缓存与打开请求队列。项目默认 FairyGUI。

## 关键类型

- `UIModule`：`SetUIFormHelper` / `SetUIFormGroupHelper`、`CreateUIFormGroup`、`GetUIFormGroup`、`DestroyCacheForms`；内部处理 `OpenFormRequest`
- `OpenFormRequest`：`RequestBase`，`FormId`、`UserData`
- `FairyGUIFormLogicBase`：`IUIFormLogic`，`FormId`/`FormGroupId`/`FormType`/`IsAllowMutiple`、`ContentPane`
- 业务基类：`FullScreenFormLogicBase`、`PopupFormLogicBase`、`MainFormLogicBase`、`FixedFormLogicBase`（`GameMain.UI`）

打开走扩展方法 `OpenUIForm`（框架 `HoweFramework.UIModuleExtensions` 与业务 `GameMain.UI.UIModuleExtensions`）。FairyGUI 接入：`UIModule.UseFairyGUI(settings)`（`FairyGUIExtensions`）。

## 用法

```csharp
await UIModule.Instance.UseFairyGUI(settings);
await UIModule.Instance.OpenUIForm(UIFormId.Xxx, userData);
await UIModule.Instance.CloseUIForm(UIFormId.Xxx);
```

未设 Helper 时创建分组会抛 `UIFormGroupHelperNotSet`。分组 id 重复：`UIFormGroupAlreadyExists`。

## 扩展点

- `IUIFormHelper` / `IUIFormGroupHelper` 替换 UI 实现。
- 新界面：FairyGUI 出包 + 生成绑定，逻辑类继承对应 `*FormLogicBase`，加入 `UIFormId` / bindings。

## 约束与坑

- 打开是请求队列，不要绕过模块直接 `UIPackage`。
- `OpenUIForm` 默认等到界面交互结束才完成；只要打开完成用 `OpenUIFormOnlyCareAboutFormOpen`。
- 错误码 100–112 见 `FrameworkErrorCode` UI 段。

## 相关源码

- `Client/Assets/HoweFramework/UI/UIModule.cs`
- `Client/Assets/HoweFramework/UI/OpenFormRequest.cs`
- `Client/Assets/HoweFramework/Extensions/UIModuleExtensions.cs`
- `Client/Assets/HoweFramework/UI/FairyGUI/FairyGUIFormLogicBase.cs`
- `Client/Assets/HoweFramework/UI/FairyGUI/FairyGUIExtensions.cs`
- `Client/Assets/GameMain/Scripts/UI/`
