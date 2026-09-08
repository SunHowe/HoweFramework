# UIModule

## 职责

按分组管理界面实例、缓存与打开请求队列。项目默认 FairyGUI。

## 关键类型

- `UIModule`：`SetUIFormHelper` / `SetUIFormGroupHelper`、`CreateUIFormGroup`、`GetUIFormGroup`、`DestroyCacheForms`；内部处理 `OpenFormRequest` / `CloseFormRequest`
- `OpenFormRequest`：`RequestBase`，`FormId`、`UserData`
- `CloseFormRequest`：`FormId`、`FormSerialId`、`CloseMutiple`
- `FairyGUIFormLogicBase`：`IUIFormLogic`，`FormId`/`FormGroupId`/`FormType`/`IsAllowMutiple`、`ContentPane`
- 业务基类：`FullScreenFormLogicBase`、`PopupFormLogicBase`、`MainFormLogicBase`、`FixedFormLogicBase`（`GameMain.UI`）

打开走扩展方法 `OpenUIForm`（框架 `HoweFramework.UIModuleExtensions` 与业务 `GameMain.UI.UIModuleExtensions`）。FairyGUI 接入：`UIModule.UseFairyGUI(settings)`（`FairyGUIExtensions`）。

## 用法

```csharp
await UIModule.Instance.UseFairyGUI(settings);
await UIModule.Instance.OpenUIForm(UIFormId.Xxx, userData);
await UIModule.Instance.CloseUIForm(UIFormId.Xxx);
await UIModule.Instance.CloseUIForm(UIFormId.Xxx, formSerialId);
await UIModule.Instance.CloseAllUIForm(UIFormId.Xxx);
```

未设 Helper 时创建分组会抛 `UIFormGroupHelperNotSet`。分组 id 重复：`UIFormGroupAlreadyExists`。

## 扩展点

- `IUIFormHelper` / `IUIFormGroupHelper` 替换 UI 实现。
- 新界面：FairyGUI 出包 + 生成绑定，逻辑类继承对应 `*FormLogicBase`，加入 `UIFormId` / bindings。

## 约束与坑

- 打开是请求队列，不要绕过模块直接 `UIPackage`。队列里的打开请求若已被取消，必须 `SetResponse(RequestCanceled)`，否则 `Execute` 的 await 不会结束、请求也无法回池。
- `OpenUIForm` 默认等到界面交互结束才完成；只要打开完成用 `OpenUIFormOnlyCareAboutFormOpen`。对已打开的非多开界面再次打开会走 `OnUpdate`，但仍会完成 `OnFormOpenSuccess`，否则 OnlyCareAboutFormOpen 会挂起。
- 关闭后的界面会进 `UIForm` 缓存。再次打开必须复用已 `OnInit` 的 `IUIFormLogic`，只换 `FormSerialId`。若对缓存实例再 `CreateUIFormLogic` + `Init`，新逻辑的 `UIForm` 仍为 null，已加载路径上的 `OnOpen` 访问 `Request` / `RequestUserData` 会空引用。
- 打开 `UIFormType.Main` 会关闭其它可被框架控制的非 Fixed 界面，并回收进缓存（不能只 `CloseImmediate` 不入缓存，否则实例泄漏、再次打开会新建）。
- `UIFormType.Fixed` 不受主界面批量关闭和栈显隐影响。业务上 `FixedFormLogicBase` 把 `IsAllowControlCloseByFramework` / `IsAllowControlVisibleByFramework` 设为 false。
- 多开界面（`IsAllowMutiple`）关闭必须带 `FormSerialId`（`IUIForm.CloseForm` 已带）。只关某个 Id 的全部实例用 `CloseAllUIForm`。
- `OpenFormRequest` 会回池，界面必须在请求结束时解绑 `CancellationToken`。不解绑的话，旧令牌取消会误伤复用后的新请求。
- 加载失败的空壳不能进缓存，应销毁；仍在加载中的界面可以进缓存，等回调里 `OnInit`。
- 错误码 100–112 见 `FrameworkErrorCode` UI 段。

## 相关源码

- `Client/Assets/HoweFramework/UI/UIModule.cs`
- `Client/Assets/HoweFramework/UI/Core/UIForm.cs`
- `Client/Assets/HoweFramework/UI/OpenFormRequest.cs`
- `Client/Assets/HoweFramework/UI/CloseFormRequest.cs`
- `Client/Assets/HoweFramework/Extensions/UIModuleExtensions.cs`
- `Client/Assets/HoweFramework/UI/FairyGUI/FairyGUIFormLogicBase.cs`
- `Client/Assets/HoweFramework/UI/FairyGUI/FairyGUIFormHelper.cs`
- `Client/Assets/HoweFramework/UI/FairyGUI/FairyGUIExtensions.cs`
- `Client/Assets/GameMain/Scripts/UI/`
