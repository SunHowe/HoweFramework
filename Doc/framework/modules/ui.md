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
await UIModule.Instance.CloseUIForm(UIFormId.Xxx, closeMultiple: true);
```

未设 Helper 时创建分组会抛 `UIFormGroupHelperNotSet`。分组 id 重复：`UIFormGroupAlreadyExists`。

## 扩展点

- `IUIFormHelper` / `IUIFormGroupHelper` 替换 UI 实现。加载必须同时提供成功与失败回调。
- 新界面：FairyGUI 出包 + 生成绑定，逻辑类继承对应 `*FormLogicBase`，加入 `UIFormId` / bindings。

## 约束与坑

- 打开是请求队列，不要绕过模块直接 `UIPackage`。凡入队的请求都会 `SetResponse`；取消令牌在出队时已取消会回 `RequestCanceled`，不要让 `Execute` 挂死。
- `OpenUIForm` 默认等到界面交互结束才完成；只要打开完成用 `OpenUIFormOnlyCareAboutFormOpen`。单例界面已打开再调后者会走 `OnUpdate` 并完成打开等待，不会挂起。
- 关闭后的界面会进 `UIForm` 缓存（含打开 `UIFormType.Main` 时框架关掉的其它界面）。再次打开必须复用已 `OnInit` 的 `IUIFormLogic`，只换 `FormSerialId`。若对缓存实例再 `CreateUIFormLogic` + `Init`，新逻辑的 `UIForm` 仍为 null，已加载路径上的 `OnOpen` 访问 `Request` / `RequestUserData` 会空引用。
- 每次打开（含单例复开、缓存命中）都会分配新的 `FormSerialId`。`IUIForm.CloseForm` 带当前序列号，避免多实例时关掉最旧的那个。不指定序列号的 `CloseUIForm(formId)` 仍关最旧实例；`closeMultiple: true` 关掉该 FormId 全部打开实例。
- 加载失败会完成打开请求、移出打开列表并 `Destroy`（不进缓存）。加载完成时若已被栈隐藏，会调 `OnInvisible`。
- 模块销毁时先给队列剩余请求回 `UIFormWhileDestroying`，再关已打开界面；单个界面关闭/销毁异常不中断销毁链。
- **回调重入契约**：在 `OnOpen`/`OnUpdate`/`OnInit` 回调中调用 `CloseForm` 是允许的，框架检测到界面已关闭后不再继续后续打开流程；`OnClose`/`OnInvisible` 回调中重入关闭会被忽略（不会重复执行关闭流程）。
- FairyGUI 包字节加载失败会兜底回调（bytes=null），界面加载失败流程正常走完，不会挂起。
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
