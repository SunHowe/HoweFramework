# IOC 依赖注入

## 概述

IOCModule 是框架的依赖注入容器，提供控制反转能力，使模块间解耦。

## 核心文件

| 文件 | 路径 |
|------|------|
| IOCModule | `Assets/HoweFramework/IOC/IOCModule.cs` |
| InjectAttribute | `Assets/HoweFramework/IOC/InjectAttribute.cs` |
| IOCModuleExtensions | `Assets/HoweFramework/Extensions/IOCModuleExtensions.cs` |

## IOCModule API

```csharp
public sealed class IOCModule : ModuleBase<IOCModule>
{
    // 注册实例（覆盖已存在的）
    public void Register<T>(T instance);

    // 注销实例
    public void UnRegister<T>();
    public void UnRegister<T>(T instance);
    public void UnRegister(Type type);

    // 获取已注册的实例
    public T Get<T>();
    public object Get(Type type);

    // 注入（将已注册实例填充到目标对象的 [Inject] 成员）
    public void Inject(object instance);
}
```

## Inject 属性

```csharp
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class InjectAttribute : Attribute
```

## 使用方式

### 1. 字段注入

```csharp
public class MyService
{
    [Inject]
    private UIModule _uiModule;

    [Inject]
    private EventModule _eventModule;

    public void Init()
    {
        this.InjectThis(); // 触发注入
    }
}
```

### 2. 属性注入

```csharp
public class MyService
{
    [Inject]
    public UIModule UI { get; set; }

    [Inject]
    public IResLoader ResLoader { get; set; }

    public void Init()
    {
        this.InjectThis();
    }
}
```

### 3. 私有成员注入

```csharp
public class MyService
{
    [Inject]
    private UIModule _uiModule; // 私有字段也会被注入

    public void Init()
    {
        this.InjectThis();
    }
}
```

## 注入流程

1. `IOCModule.Inject(instance)` 被调用
2. 首次注入时，通过反射缓存类型的 `[Inject]` 成员信息
3. 根据成员类型从注册表中查找对应实例
4. 通过反射设置成员值（支持 public 和 private）

## 自动注册

模块初始化时自动注册到 IOC：

```csharp
// ModuleBase<T> 的 Init 方法
internal sealed override void Init()
{
    Instance = (T)this;
    OnInit();
    if (RegisterIOC)
        IOCModule.Instance.Register(Instance);
}
```

## 扩展方法

```csharp
// IOCModuleExtensions.cs
public static void InjectThis(this object obj)
{
    IOCModule.Instance.Inject(obj);
}
```

## 最佳实践

### 正确的做法

```csharp
public class MyService
{
    [Inject]
    private UIModule _uiModule;

    public void Init()
    {
        this.InjectThis(); // Init 时调用
    }
}
```

### 在构造函数中注入（不推荐）

```csharp
public class MyService
{
    private UIModule _uiModule;

    public MyService()
    {
        this.InjectThis(); // 不推荐，IOC 可能在构造时未初始化
    }
}
```

## 获取已注册实例

```csharp
// 方式一：通过 Get<T>
var uiModule = IOCModule.Instance.Get<UIModule>();

// 方式二：通过 IOC 扩展
var uiModule = IOCModule.Instance.Get<UIModule>();

// 方式三：通过模块单例（最常用）
var uiModule = UIModule.Instance;
```

## 示例：完整的服务类

```csharp
public interface ILoginService
{
    void Login(string username, string password);
}

public sealed class LoginService : ILoginService
{
    [Inject]
    private NetworkModule _networkModule;

    [Inject]
    private EventModule _eventModule;

    public void Init()
    {
        this.InjectThis();
    }

    public void Login(string username, string password)
    {
        // 使用注入的模块
        _networkModule.Send(new LoginRequest { Username = username, Password = password });
    }
}
```

## 与模块系统的集成

所有模块默认自动注册到 IOC，但可以通过重写 `RegisterIOC` 属性禁用：

```csharp
public sealed class MyModule : ModuleBase<MyModule>
{
    // 禁用自动 IOC 注册
    protected override bool RegisterIOC => false;

    protected override void OnInit() { }
    protected override void OnDestroy() { }
    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds) { }
}
```