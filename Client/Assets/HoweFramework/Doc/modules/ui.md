# UI 系统

## 概述

UIModule 管理游戏中的所有 UI 界面，基于 FairyGUI 实现。

## 核心文件

| 文件 | 路径 |
|------|------|
| UIModule | `Assets/HoweFramework/UI/UIModule.cs` |
| IUIFormLogic | `Assets/HoweFramework/UI/Core/IUIFormLogic.cs` |
| UIFormType | `Assets/HoweFramework/UI/Core/UIFormType.cs` |
| FairyGUIFormLogicBase | `Assets/HoweFramework/UI/FairyGUI/FairyGUIFormLogicBase.cs` |
| UIModuleExtensions | `Assets/HoweFramework/Extensions/UIModuleExtensions.cs` |

## UIFormType 枚举

```csharp
public enum UIFormType
{
    Main = 0,    // 主界面，唯一存在，关闭其他所有 Form
    Normal = 1,  // 普通界面，打开时隐藏下层
    Popup = 2,    // 弹窗，总是在最上层
    Fixed = 4,   // 固定界面，不参与层级排序
}
```

## UI Form 基类

### FairyGUIFormLogicBase

所有 UI Form 逻辑类继承自 `FairyGUIFormLogicBase`：

```csharp
public abstract class FairyGUIFormLogicBase : IUIFormLogic
{
    public IUIForm UIForm { get; private set; }
    public GComponent ContentPane { get; private set; }

    public abstract int FormId { get; }
    public abstract int FormGroupId { get; }
    public abstract UIFormType FormType { get; }
    public abstract bool IsAllowMutiple { get; }
    public abstract FairyGUIScreenAdaptorType ScreenAdaptorType { get; }

    public DisposableGroup DisposableGroup { get; } = new DisposableGroup();

    // 框架自动注入
    public GComponent Frame { get; protected set; }
    public GObject CloseButton { get; set; }    // 自动绑定关闭
    public GObject BackButton { get; set; }      // 自动绑定返回

    protected abstract void OnInit();
    public abstract void OnOpen();
    public abstract void OnClose();
    public abstract void OnUpdate();
    public abstract void OnVisible();
    public abstract void OnInvisible();

    protected virtual void OnCloseButtonClick() => UIForm.CloseForm();
    protected virtual void OnBackButtonClick() => OnCloseButtonClick();
}
```

## UI Form 类型

### 1. FullScreenFormLogicBase

适用于全屏普通界面：

```csharp
// GameMain/Scripts/UI/Base/FullScreenFormLogicBase.cs
public abstract class FullScreenFormLogicBase : FairyGUIFormLogicBase
{
    public override int FormGroupId => (int)UIGroupId.Main;
    public override UIFormType FormType => UIFormType.Normal;
    public override bool IsAllowMutiple => false;
    public override FairyGUIScreenAdaptorType ScreenAdaptorType => FairyGUIScreenAdaptorType.FullScreen;
}
```

### 2. MainFormLogicBase

适用于主菜单：

```csharp
public abstract class MainFormLogicBase : FairyGUIFormLogicBase
{
    public override int FormGroupId => (int)UIGroupId.Background;
    public override UIFormType FormType => UIFormType.Main;
    public override bool IsAllowMutiple => false;
    public override FairyGUIScreenAdaptorType ScreenAdaptorType => FairyGUIScreenAdaptorType.FullScreen;

    // CloseButton 被禁用
    // BackButton 显示退出游戏提示
}
```

### 3. FixedFormLogicBase

适用于固定元素（提示、Toast 等）：

```csharp
public abstract class FixedFormLogicBase : FairyGUIFormLogicBase
{
    public override int FormGroupId => (int)UIGroupId.Tips;
    public override UIFormType FormType => UIFormType.Fixed;
    public override bool IsAllowMutiple => true;
    public override FairyGUIScreenAdaptorType ScreenAdaptorType => FairyGUIScreenAdaptorType.None;
}
```

### 4. PopupFormLogicBase

适用于模态弹窗：

```csharp
public abstract class PopupFormLogicBase : FairyGUIFormLogicBase
{
    public override int FormGroupId => (int)UIGroupId.Main;
    public override UIFormType FormType => UIFormType.Popup;
    public override bool IsAllowMutiple => false;
    public override FairyGUIScreenAdaptorType ScreenAdaptorType => FairyGUIScreenAdaptorType.ConstantCenter;
}
```

## UI Group 层级

```csharp
public enum UIGroupId
{
    Background,    // 背景层
    Main,          // 主界面层
    ScreenEffect,  // 屏幕效果层
    Tips,          // 提示层
    Count,         // 总数
}
```

## 创建 UI Form

### 1. 创建 FormLogic 类

```csharp
namespace GameMain.UI
{
    public sealed class MainMenuForm : FullScreenFormLogicBase
    {
        public override int FormId => (int)UIFormId.MainMenu;

        // 初始化（仅调用一次）
        protected override void OnInit()
        {
            // 查找组件
            _startButton = ContentPane.GetChild("btn_start").asButton;
            _startButton.onClick.Add(OnStartClick);
        }

        // 打开时调用
        public override void OnOpen()
        {
            // 可以获取传入的 userData
        }

        // 关闭时调用
        public override void OnClose()
        {
        }

        // 每帧调用
        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }

        // 可见时调用
        public override void OnVisible()
        {
        }

        // 不可见时调用
        public override void OnInvisible()
        {
        }

        private void OnStartClick()
        {
            UIForm.CloseForm();
        }
    }
}
```

### 2. 注册 FormId

在 `UIFormId.cs` 中添加：
```csharp
public enum UIFormId
{
    None = 0,
    MainMenu = 1,
    // ...
}
```

### 3. 注册 Form 绑定

在 `UIFormBindings.cs` 中添加：
```csharp
public static readonly FairyGUIFormBinding[] Bindings = new FairyGUIFormBinding[]
{
    new FairyGUIFormBinding((int)UIFormId.MainMenu, "GameMain", "MainMenuForm"),
    // ...
};
```

## UIModule API

```csharp
public sealed class UIModule : ModuleBase<UIModule>
{
    // 设置 helpers
    public void SetUIFormHelper(IUIFormHelper uiFormHelper);
    public void SetUIFormGroupHelper(IUIFormGroupHelper uiFormGroupHelper);

    // Group 管理
    public void CreateUIFormGroup(int groupId, string groupName);
    public IUIFormGroup GetUIFormGroup(int groupId);

    // 通过请求打开/关闭
    public void HandleOpenFormRequest(OpenFormRequest request);
    public void HandleCloseFormRequest(CloseFormRequest request);

    // 销毁缓存的 Form
    public void DestroyCacheForms();
}
```

## UIModuleExtensions 扩展方法

```csharp
// 打开 Form 并等待响应
public static UniTask<IResponse> OpenUIForm(this UIModule module, int uiFormId, object userData, CancellationToken token = default);
public static UniTask<IResponse> OpenUIForm(this UIModule module, int uiFormId, CancellationToken token = default);

// 打开 Form，只关心是否成功
public static UniTask<int> OpenUIFormOnlyCareAboutFormOpen(this UIModule module, int uiFormId, object userData, CancellationToken token = default);

// 关闭 Form
public static UniTask<IResponse> CloseUIForm(this UIModule module, int uiFormId);

// 打开错误提示
public static UniTask OpenErrorTips(this UIModule module, UniTask operation, CancellationToken token = default);
```

## 生命周期

```
OnInit()      ← Form 创建时调用，仅一次
     ↓
OnOpen()      ← Form 打开时调用
     ↓
OnUpdate()     ← 每帧调用（Form 可见时）
     ↓
OnClose()     ← Form 关闭时调用
     ↓
OnDestroy()   ← Form 销毁时调用
```

可见性变化：
```
OnVisible()   ← Form 变为可见时
OnInvisible() ← Form 变为不可见时
```

## 打开/关闭 Form

```csharp
// 打开（异步等待）
var response = await UIModule.Instance.OpenUIForm((int)UIFormId.MainMenu, userData);
if (response.ErrorCode == 0)
{
    // 成功
}

// 打开（不等待）
UIModule.Instance.OpenUIFormOnlyCareAboutFormOpen((int)UIFormId.MainMenu, userData).Forget();

// 关闭
await UIModule.Instance.CloseUIForm((int)UIFormId.MainMenu);
```

## UserData 传递

```csharp
// 打开时传递数据
await UIModule.Instance.OpenUIForm((int)UIFormId.Dialog, dialogData);

// OnOpen 中接收
public override void OnOpen()
{
    var userData = UIForm.UserData; // 获取传递的数据
    if (userData is DialogData data)
    {
        // 使用数据
    }
}
```

## 最佳实践

### 1. 使用合适的基类

| 场景 | 基类 |
|------|------|
| 普通全屏界面 | FullScreenFormLogicBase |
| 主菜单 | MainFormLogicBase |
| 提示/Toast | FixedFormLogicBase |
| 模态弹窗 | PopupFormLogicBase |

### 2. 在 OnInit 中查找组件

```csharp
protected override void OnInit()
{
    _button = ContentPane.GetChild("btn").asButton;
    _list = ContentPane.GetChild("list").asList;
    _text = ContentPane.GetChild("txt").asTextField;
}
```

### 3. 取消订阅事件

```csharp
public override void OnClose()
{
    _button.onClick.RemoveAll();
    EventModule.Instance.Unsubscribe(MyEventArgs.EventId, OnHandler);
}
```

### 4. 使用 DisposableGroup

```csharp
protected override void OnInit()
{
    DisposableGroup.Add(EventModule.Instance.Subscribe(...));
}
```