# HoweFramework

Unity 游戏框架，提供模块化架构、依赖注入、UI 管理、事件系统等核心功能。

## 目录

- [框架架构](#框架架构)
- [核心概念](#核心概念)
- [模块系统](#模块系统)
- [IOC 依赖注入](#ioc-依赖注入)
- [UI 系统](#ui-系统)
- [事件系统](#事件系统)
- [引用池](#引用池)
- [资源系统](#资源系统)
- [配置表系统](#配置表系统)
- [计时器系统](#计时器系统)
- [网络系统](#网络系统)
- [基础系统](#基础系统)
- [声音系统](#声音系统)
- [设置系统](#设置系统)
- [对象池系统](#对象池系统)
- [本地化系统](#本地化系统)
- [场景系统](#场景系统)
- [相机系统](#相机系统)
- [Web请求系统](#web请求系统)
- [安全区域系统](#安全区域系统)
- [行为树系统](#行为树系统)
- [系统模块](#系统模块)
- [远程请求调度器](#远程请求调度器)
- [流程系统](#流程系统)
- [错误码规范](#错误码规范)
- [代码规范](#代码规范)

---

## 框架架构

 HoweFramework 采用模块化架构，所有功能以模块形式提供：

```
GameApp (入口)
    ├── IOCModule (依赖注入)
    ├── BaseModule (基础工具)
    ├── EventModule (事件)
    ├── RemoteRequestModule (远程请求)
    ├── NetworkModule (网络)
    ├── WebRequestModule (HTTP)
    ├── TimerModule (计时器)
    ├── SettingModule (配置)
    ├── SafeAreaModule (安全区域)
    ├── ResModule (资源)
    ├── SceneModule (场景)
    ├── CameraModule (相机)
    ├── SoundModule (声音)
    ├── GameObjectPoolModule (对象池)
    ├── DataTableModule (配置表)
    ├── LocalizationModule (本地化)
    ├── BehaviorModule (行为树)
    ├── SystemModule (系统)
    ├── UIModule (界面)
    └── ProcedureModule (流程)
```

**入口文件**: `Assets/HoweFramework/GameApp.cs`

---

## 核心概念

### 模块单例
所有框架模块均为单例模式，通过 `XXXModule.Instance` 访问：

```csharp
UIModule.Instance.OpenForm(formId);
ResModule.Instance.LoadAssetAsync<T>(assetPath);
EventModule.Instance.Subscribe(eventId, handler);
```

### 游戏入口
`GameApp` 在 Unity MonoBehaviour 中创建：

```csharp
// Assets/GameMain/Scripts/GameEntry.cs
public class GameEntry : MonoBehaviour
{
    private void Awake()
    {
        m_GameApp = new GameApp();
    }

    private void Update()
    {
        m_GameApp.Update(Time.deltaTime, Time.unscaledDeltaTime);
    }

    private void OnDestroy()
    {
        m_GameApp.Destroy();
    }
}
```

### GameApp API

```csharp
// 应用实例
static GameApp Instance { get; }

// 游戏应用销毁事件
event Action OnGameAppDestroyed;

// 逻辑帧更新
void Update(float elapseSeconds, float realElapseSeconds);

// 销毁应用
void Destroy();

// 重启游戏（重新加载主场景）
void RestartGame();
```

### GameApp 模块注册顺序

`GameApp` 构造函数中按以下顺序注册模块：

| 顺序 | 模块 | 说明 |
|------|------|------|
| 1 | IOCModule | 依赖注入 |
| 2 | BaseModule | 基础工具 |
| 3 | EventModule | 事件系统 |
| 4 | RemoteRequestModule | 远程请求 |
| 5 | NetworkModule | 网络 |
| 6 | WebRequestModule | HTTP请求 |
| 7 | TimerModule | 计时器 |
| 8 | SettingModule | 设置 |
| 9 | SafeAreaModule | 安全区域 |
| 10 | ResModule | 资源 |
| 11 | SceneModule | 场景 |
| 12 | CameraModule | 相机 |
| 13 | SoundModule | 声音 |
| 14 | GameObjectPoolModule | 对象池 |
| 15 | DataTableModule | 配置表 |
| 16 | LocalizationModule | 本地化 |
| 17 | BehaviorModule | 行为树 |
| 18 | SystemModule | 系统 |
| 19 | UIModule | UI |
| 20 | ProcedureModule | 流程 |

**核心文件**: `Assets/HoweFramework/GameApp.cs`

---

## 模块系统

### ModuleBase<T>

所有框架模块继承 `ModuleBase<T>`:

```csharp
// 声明
public sealed class UIModule : ModuleBase<UIModule>
{
    protected override void OnInit() { }
    protected override void OnDestroy() { }
    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds) { }
}
```

### ModuleBase<T> 属性和方法

```csharp
// 模块实例（单例）
static T Instance { get; }

// 是否注册到 IOC（默认为 true）
protected virtual bool RegisterIOC => true;
```

### 生命周期

1. **Init()** - 模块初始化，自动调用 `OnInit()`
2. **Update()** - 每帧调用，自动调用 `OnUpdate(elapseSeconds, realElapseSeconds)`
3. **Destroy()** - 模块销毁，自动调用 `OnDestroy()`

### IOC 注册

默认情况下，模块会自动注册到 IOC 容器。如需禁用：

```csharp
protected override bool RegisterIOC => false;
```

**核心文件**: `Assets/HoweFramework/Base/ModuleBase.cs`

---

## IOC 依赖注入

### Inject 属性

使用 `[Inject]` 标记需要注入的字段或属性：

```csharp
public class MyClass
{
    [Inject]
    private UIModule _uiModule;

    [Inject]
    public IResLoader ResLoader { get; set; }
}
```

### 注入调用

```csharp
// 方式1: 使用扩展方法
obj.InjectThis();

// 方式2: 直接调用
IOCModule.Instance.Inject(obj);
```

### IOCModule API

```csharp
// 注册
IOCModule.Instance.Register<T>(instance);

// 获取
IOCModule.Instance.Get<T>();

// 移除
IOCModule.Instance.UnRegister<T>();

// 注入
IOCModule.Instance.Inject(obj);
```

**核心文件**: `Assets/HoweFramework/IOC/IOCModule.cs`

---

## UI 系统

### 界面类型

| 类型 | 说明 | 特性 |
|------|------|------|
| `Main` | 主界面 | 打开时关闭其他所有界面 |
| `Fixed` | 固定界面 | 不参与 UI 栈排序 |
| `Normal` | 普通界面 | 参与 UI 栈排序 |
| `Popup` | 弹窗界面 | 始终显示在顶层 |

### 界面基类

```csharp
// FairyGUI 界面
public abstract class FairyGUIFormLogicBase : IUIFormLogic

// 游戏界面基类 (在 GameMain 中定义)
public abstract class MainFormLogicBase : FairyGUIFormLogicBase
public abstract class FixedFormLogicBase : FairyGUIFormLogicBase
public abstract class PopupFormLogicBase : FairyGUIFormLogicBase
public abstract class FullScreenFormLogicBase : FairyGUIFormLogicBase
```

### 创建界面

1. **定义界面 ID** (`UIFormId` 枚举)：
```csharp
public enum UIFormId
{
    MainMenu = 1,
    Settings = 2,
}
```

2. **定义界面分组** (`UIGroupId` 枚举)：
```csharp
public enum UIGroupId
{
    Background = 0,
    Main = 1,
    ScreenEffect = 2,
    Tips = 3,
}
```

3. **实现界面逻辑**：
```csharp
public class MainMenuFormLogic : MainFormLogicBase
{
    public override int FormId => (int)UIFormId.MainMenu;
    public override int FormGroupId => (int)UIGroupId.Main;
    public override UIFormType FormType => UIFormType.Main;
    public override bool IsAllowMutiple => false;
    public override FairyGUIScreenAdaptorType ScreenAdaptorType => FairyGUIScreenAdaptorType.FullScreen;

    protected override void OnInit() { }
    public override void OnOpen() { }
    public override void OnClose() { }
    public override void OnUpdate() { }
    public override void OnVisible() { }
    public override void OnInvisible() { }
}
```

### 界面生命周期

```
OnInit() → OnOpen() → OnUpdate() (循环) → OnClose() → OnDestroy()
           ↓
        OnVisible() / OnInvisible() (根据 UI 栈状态)
```

### 界面操作

```csharp
// 打开界面
UIModule.Instance.OpenForm(formId);
UIModule.Instance.OpenForm(formId, userData);

// 关闭界面
UIModule.Instance.CloseForm(formId);

// 创建分组
UIModule.Instance.CreateUIFormGroup(groupId, groupName);
```

### 屏幕适配

```csharp
public enum FairyGUIScreenAdaptorType
{
    ConstantHorizontalCenter,  // 水平居中
    ConstantVerticalCenter,    // 垂直居中
    ConstantCenter,            // 完全居中
    FullScreen,                // 全屏
    SafeAreaFullScreen,        // 安全区域全屏
}
```

**核心文件**:
- `Assets/HoweFramework/UI/UIModule.cs`
- `Assets/HoweFramework/UI/FairyGUI/FairyGUIFormLogicBase.cs`
- `Assets/GameMain/Scripts/UI/Base/MainFormLogicBase.cs`

---

## 事件系统

### 订阅事件

```csharp
EventModule.Instance.Subscribe(eventId, OnEventHandler);

private void OnEventHandler(object sender, GameEventArgs e)
{
    // 处理事件
}
```

### 派发事件

```csharp
EventModule.Instance.Dispatch(sender, eventArgs);
```

### 取消订阅

```csharp
EventModule.Instance.Unsubscribe(eventId, OnEventHandler);
```

### 创建事件参数

```csharp
public class PlayerHealthChangedEventArgs : GameEventArgs
{
    public int PlayerId { get; set; }
    public int NewHealth { get; set; }
    public int OldHealth { get; set; }

    public override int Id => EventId.PlayerHealthChanged;
}
```

### 创建独立事件调度器

```csharp
var dispatcher = EventModule.Instance.CreateEventDispatcher();
dispatcher.Subscribe(eventId, handler);
// ...
dispatcher.Dispose();
```

**核心文件**: `Assets/HoweFramework/Event/EventModule.cs`

---

## 引用池

### 实现 IReference

```csharp
public class EffectData : IReference
{
    public int EffectId { get; set; }
    public Vector3 Position { get; set; }

    public void Clear()
    {
        EffectId = 0;
        Position = Vector3.zero;
    }
}
```

### 使用引用池

```csharp
// 获取
var data = ReferencePool.Acquire<EffectData>();
data.EffectId = 1001;

// 归还
ReferencePool.Release(data);

// 清理指定类型的缓存
ReferencePool.ClearCache<EffectData>();

// 清理所有缓存
ReferencePool.ClearAllCache();
```

**核心文件**: `Assets/HoweFramework/Reference/ReferencePool.cs`

---

## 资源系统

### ResModule API

```csharp
// 设置核心资源加载器（通常在 GameApp 初始化时设置）
ResModule.Instance.SetResCoreLoader(resLoader);

// 创建资源加载器（每次加载资源前创建）
var loader = ResModule.Instance.CreateResLoader();

// 异步加载资源
UniTask<Object> LoadAssetAsync<T>(string assetKey, CancellationToken token = default);

// 同步加载资源
T LoadAsset<T>(string assetKey) where T : Object;

// 异步加载二进制
UniTask<byte[]> LoadBinaryAsync(string assetKey, CancellationToken token = default);

// 同步加载二进制
byte[] LoadBinary(string assetKey);

// 卸载资源
loader.UnloadAsset(assetKey);

// 卸载未使用资源
ResModule.Instance.UnloadUnusedAsset();

// 释放加载器
loader.Dispose();
```

### 使用示例

```csharp
// 1. 在 GameApp 初始化时设置核心加载器
public class GameApp
{
    protected override void InitModules()
    {
        var resLoader = new DefaultResLoader(); // 取决于实际使用的加载器实现
        ResModule.Instance.SetResCoreLoader(resLoader);
    }
}

// 2. 加载 Prefab
var loader = ResModule.Instance.CreateResLoader();
var prefab = await loader.LoadAssetAsync<GameObject>("Prefabs/Player");
loader.Dispose();

// 3. 加载 Sprite
var spriteLoader = ResModule.Instance.CreateResLoader();
var sprite = await spriteLoader.LoadAssetAsync<Sprite>("UI/Sprites/icon");
spriteLoader.Dispose();
```

**核心文件**: `Assets/HoweFramework/Res/ResModule.cs`, `Assets/HoweFramework/Res/Core/IResLoader.cs`

---

## 配置表系统

### DataTableModule API

```csharp
// 加载模式
DataTableModule.Instance.LoadMode = DataTableLoadMode.LazyLoadAndPreloadAsync;

// 添加配置表数据源
DataTableModule.Instance.AddDataTableSource(dataTableSource);

// 同步预加载
DataTableModule.Instance.Preload();

// 异步预加载
await DataTableModule.Instance.PreloadAsync();

// 移除数据源
DataTableModule.Instance.RemoveDataTableSource(dataTableSource);
```

### 使用示例

```csharp
// 1. 创建数据源并添加
var dataTableSource = new JsonDataTableSource(); // 或其他实现
DataTableModule.Instance.AddDataTableSource(dataTableSource);

// 2. 预加载配置表
await DataTableModule.Instance.PreloadAsync();

// 3. 在游戏中使用配置表数据
// 具体用法取决于 IDataTableSource 的实现
```

### 配置表加载模式

| 模式 | 说明 |
|------|------|
| `LazyLoad` | 懒加载，按需加载 |
| `Preload` | 预加载，同步 |
| `LazyLoadAndPreloadAsync` | 懒加载 + 异步预加载（默认）|

**核心文件**: `Assets/HoweFramework/DataTable/DataTableModule.cs`

---

## 计时器系统

### TimerModule API

```csharp
// 添加帧定时器（每帧回调）
int timerId = TimerModule.Instance.AddFrameTimer(TimerCallback callback);

// 添加帧定时器（间隔帧数）
int timerId = TimerModule.Instance.AddFrameTimer(int interval, TimerCallback callback);

// 添加帧定时器（间隔帧数 + 重复次数）
int timerId = TimerModule.Instance.AddFrameTimer(int interval, int repeatTimes, TimerCallback callback);

// 添加定时器（按时间间隔）
int timerId = TimerModule.Instance.AddTimer(float interval, TimerCallback callback);

// 添加定时器（时间间隔 + 重复次数）
int timerId = TimerModule.Instance.AddTimer(float interval, int repeatTimes, TimerCallback callback);

// 移除定时器
TimerModule.Instance.RemoveTimer(int timerId);

// 创建独立的计时器调度器
ITimerDispatcher dispatcher = TimerModule.Instance.CreateTimerDispatcher();
dispatcher.Dispose(); // 使用完毕后释放
```

### 使用示例

```csharp
// 每帧执行
int frameTimerId = TimerModule.Instance.AddFrameTimer(() =>
{
    Debug.Log("每帧执行");
});

// 每1秒执行
int secondTimerId = TimerModule.Instance.AddTimer(1f, () =>
{
    Debug.Log("每秒执行");
});

// 执行5次，每2秒一次
int repeatTimerId = TimerModule.Instance.AddTimer(2f, 5, () =>
{
    Debug.Log("重复执行");
});

// 手动停止
TimerModule.Instance.RemoveTimer(secondTimerId);
```

**核心文件**: `Assets/HoweFramework/Timer/TimerModule.cs`

---

## 网络系统

### NetworkModule API

```csharp
// 检查网络频道是否存在
bool exists = NetworkModule.Instance.HasNetworkChannel(name);

// 获取网络频道
var channel = NetworkModule.Instance.GetNetworkChannel(name);

// 获取所有网络频道
var channels = NetworkModule.Instance.GetAllNetworkChannels();

// 创建网络频道
NetworkModule.Instance.CreateNetworkChannel(name, serviceType, networkChannelHelper);

// 创建默认网络频道（只能调用一次）
NetworkModule.Instance.CreateDefaultNetworkChannel(name, serviceType, networkChannelHelper);

// 销毁网络频道
NetworkModule.Instance.DestroyNetworkChannel(name);
```

### INetworkChannel API

```csharp
// 频道属性
string Name { get; }
Socket Socket { get; }
bool Connected { get; }
ServiceType ServiceType { get; }
AddressFamily AddressFamily { get; }
int SendPacketCount { get; }
int ReceivedPacketCount { get; }
float HeartBeatInterval { get; set; }

// 连接
void Connect(IPAddress ipAddress, int port);
void Connect(IPAddress ipAddress, int port, object userData);

// 发送消息
void Send<T>(T packet) where T : Packet;

// 关闭
void Close();

// 注册消息处理
void RegisterHandler(IPacketHandler handler);
void SetDefaultHandler(GameEventHandlerFunc handler);
```

### ServiceType 枚举

| 类型 | 说明 |
|------|------|
| `Tcp` | TCP 网络服务 |
| `TcpWithSyncReceive` | 使用同步接收的 TCP 网络服务 |

### 使用示例

```csharp
// 1. 创建网络频道辅助器（需要实现 INetworkChannelHelper）
public class MyNetworkChannelHelper : INetworkChannelHelper
{
    public int PacketHeaderLength => 4;
    public IRemoteRequestDispatcher RequestDispatcher => _dispatcher;
    private IRemoteRequestDispatcher _dispatcher;

    public void Initialize(INetworkChannel networkChannel) { }
    public void PrepareForConnecting() { }
    public bool SendHeartBeat() => true;
    public bool Serialize<T>(T packet, Stream destination) where T : Packet { return true; }
    public T Deserialize<T>(Stream source) where T : Packet { return null; }
    public void Dispose() { }
}

// 2. 创建网络频道
var helper = new MyNetworkChannelHelper();
var channel = NetworkModule.Instance.CreateNetworkChannel("Game", ServiceType.Tcp, helper);

// 3. 连接服务器
channel.Connect(IPAddress.Parse("127.0.0.1"), 8888);

// 4. 发送消息
channel.Send(new MyPacket { Data = "hello" });

// 5. 关闭连接
channel.Close();
```

### 网络事件

通过 EventModule 订阅网络事件：

| 事件 | 说明 |
|------|------|
| `NetworkConnectedEventArgs` | 连接成功 |
| `NetworkClosedEventArgs` | 连接关闭 |
| `NetworkMissHeartBeatEventArgs` | 丢失心跳 |
| `NetworkErrorEventArgs` | 网络错误 |
| `NetworkCustomErrorEventArgs` | 自定义错误 |

**核心文件**:
- `Assets/HoweFramework/Network/NetworkModule.cs`
- `Assets/HoweFramework/Network/Core/INetworkChannel.cs`
- `Assets/HoweFramework/Network/Core/INetworkChannelHelper.cs`

---

## 流程系统

### 定义流程

```csharp
public class ProcedureSplash : ProcedureBase
{
    public override int Id => (int)ProcedureId.Splash;

    protected override void OnEnter()
    {
        // 进入流程
    }

    protected override void OnLeave()
    {
        // 离开流程
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        // 流程更新
    }
}
```

### 切换流程

```csharp
// 切换到指定流程
ChangeProcedure(procedureId);

// 切换到下一个流程 (Id + 1)
ChangeNextProcedure();
```

### 启动流程

```csharp
var procedures = new ProcedureBase[]
{
    new ProcedureSplash(),
    new ProcedureLoadDataTable(),
    new ProcedureLogin(),
};

ProcedureModule.Instance.Launch((int)ProcedureId.Splash, procedures);
```

**核心文件**:
- `Assets/HoweFramework/Procedure/ProcedureModule.cs`
- `Assets/HoweFramework/Procedure/ProcedureBase.cs`

---

## 基础系统

### BaseModule API

```csharp
// 设置 Json 辅助器
BaseModule.SetJsonHelper(IJsonHelper jsonHelper);

// 设置文本模板辅助器
BaseModule.SetTextTemplateHelper(ITextTemplateHelper textTemplateHelper);
```

**核心文件**: `Assets/HoweFramework/Base/BaseModule.cs`

---

## 声音系统

### SoundModule API

```csharp
// 全局音量
float Volume { get; set; }

// 设置声音辅助器
SoundModule.Instance.SetSoundHelper(ISoundHelper soundHelper);

// 创建声音组
SoundModule.Instance.CreateSoundGroup(int groupId, string groupName, float volume = 1f, int soundLimit = 8);

// 销毁声音组
SoundModule.Instance.DestroySoundGroup(int groupId);

// 播放声音
int PlaySound(int groupId, string soundAssetName, float volume = 1f, bool loop = false);

// 停止声音（通过编号）
SoundModule.Instance.StopSound(int serialId);

// 停止声音（通过资源名）
SoundModule.Instance.StopSound(string soundAssetName, int groupId = 0);

// 停止所有声音
SoundModule.Instance.StopAllSounds(int groupId = 0);

// 暂停声音
SoundModule.Instance.PauseSound(int serialId);

// 恢复声音
SoundModule.Instance.ResumeSound(int serialId);

// 是否正在加载
bool IsLoadingSound(int serialId);
```

### 使用示例

```csharp
// 1. 设置声音辅助器（需要实现 ISoundHelper）
SoundModule.Instance.SetSoundHelper(new MySoundHelper());

// 2. 创建声音组
SoundModule.Instance.CreateSoundGroup(0, "BGM", 1f, 2);
SoundModule.Instance.CreateSoundGroup(1, "SFX", 1f, 8);

// 3. 播放背景音乐
int bgmId = SoundModule.Instance.PlaySound(0, "Audio/BGM/menu", 1f, true);

// 4. 播放音效
SoundModule.Instance.PlaySound(1, "Audio/SFX/click", 0.8f, false);

// 5. 暂停/恢复
SoundModule.Instance.PauseSound(bgmId);
SoundModule.Instance.ResumeSound(bgmId);

// 6. 停止
SoundModule.Instance.StopSound(bgmId);
SoundModule.Instance.StopAllSounds(0);
```

**核心文件**: `Assets/HoweFramework/Sound/SoundModule.cs`

---

## 设置系统

### SettingModule API

```csharp
// 设置辅助器
SettingModule.Instance.SetSettingHelper(ISettingHelper settingHelper);

// 加载/保存
bool Load();
bool Save();

// 检查/移除
bool HasSetting(string settingName);
bool RemoveSetting(string settingName);
void RemoveAllSettings();

// 读取/写入 布尔值
bool GetBool(string settingName, bool defaultValue = false);
void SetBool(string settingName, bool value);

// 读取/写入 整数值
int GetInt(string settingName, int defaultValue = 0);
void SetInt(string settingName, int value);

// 读取/写入 浮点值
float GetFloat(string settingName, float defaultValue = 0f);
void SetFloat(string settingName, float value);

// 读取/写入 字符串
string GetString(string settingName, string defaultValue = "");
void SetString(string settingName, string value);

// 读取/写入 对象（序列化）
T GetObject<T>(string settingName, T defaultObj = default);
void SetObject<T>(string settingName, T obj);
```

### 使用示例

```csharp
// 1. 设置辅助器（通常在 GameApp 初始化时）
SettingModule.Instance.SetSettingHelper(new PlayerPrefsSettingHelper());

// 2. 加载设置
SettingModule.Instance.Load();

// 3. 保存设置
SettingModule.Instance.SetInt("Quality", 2);
SettingModule.Instance.SetFloat("Volume", 0.8f);
SettingModule.Instance.SetString("PlayerName", "Player1");
SettingModule.Instance.Save();

// 4. 读取设置
int quality = SettingModule.Instance.GetInt("Quality", 1);
float volume = SettingModule.Instance.GetFloat("Volume", 1f);
```

**核心文件**: `Assets/HoweFramework/Setting/SettingModule.cs`

---

## 对象池系统

### GameObjectPoolModule API

```csharp
// 创建对象池（需要时创建，不需要时释放）
IGameObjectPool CreateGameObjectPool(IResLoader resLoader = null);

// 异步实例化
UniTask<GameObject> InstantiateAsync(string assetKey, CancellationToken token = default);

// 预加载
UniTask PreloadAsync(string assetKey, int count, CancellationToken token = default);

// 释放对象
void Release(GameObject gameObject);

// 获取缓存数量
int GetCacheCount(string assetKey);

// 设置缓存限制
void SetCacheCountLimit(string assetKey, int limit);

// 清理缓存
void ClearCache(string assetKey);
void ClearAllCache();
```

### 使用示例

```csharp
// 1. 预加载对象
await GameObjectPoolModule.Instance.PreloadAsync("Prefabs/Bullet", 10);

// 2. 实例化
var bullet = await GameObjectPoolModule.Instance.InstantiateAsync("Prefabs/Bullet");
bullet.transform.position = Vector3.forward;

// 3. 回收
GameObjectPoolModule.Instance.Release(bullet);

// 4. 清理
GameObjectPoolModule.Instance.ClearCache("Prefabs/Bullet");
```

**核心文件**: `Assets/HoweFramework/GameObjectPool/GameObjectPoolModule.cs`

---

## 本地化系统

### LocalizationModule API

```csharp
// 当前语言
Language Language { get; set; }

// 默认语言
Language DefaultLanguage { get; }

// 系统语言
Language SystemLanguage { get; }

// 获取本地化文本
string GetText(string key);

// 添加文本
void AddText(string key, string text);

// 清空文本
void ClearText();

// 添加数据源
void AddSource(ILocalizationSource source);

// 移除数据源
void RemoveSource(ILocalizationSource source);

// 异步加载
UniTask LoadAsync();
```

### Language 枚举

```csharp
public enum Language
{
    Unspecified = 0,
    ChineseSimplified = 1,
    ChineseTraditional = 2,
    English = 3,
    // ... 更多语言
}
```

### 使用示例

```csharp
// 1. 添加数据源
LocalizationModule.Instance.AddSource(new JsonLocalizationSource());

// 2. 设置语言
LocalizationModule.Instance.Language = Language.ChineseSimplified;

// 3. 加载
await LocalizationModule.Instance.LoadAsync();

// 4. 获取文本
string welcome = LocalizationModule.Instance.GetText("UI_Welcome");
```

**核心文件**: `Assets/HoweFramework/Localization/LocalizationModule.cs`

---

## 场景系统

### SceneModule API

```csharp
// 加载场景
UniTask LoadSceneAsync(string sceneAssetName);

// 卸载场景
UniTask UnloadSceneAsync(string sceneAssetName);

// 设置场景顺序
void SetSceneOrder(string sceneAssetName, int sceneOrder);

// 检查场景状态
bool SceneIsLoaded(string sceneAssetName);
bool SceneIsLoading(string sceneAssetName);
bool SceneIsUnloading(string sceneAssetName);

// 获取场景名称（从资源名）
static string GetSceneName(string sceneAssetName);
```

### 使用示例

```csharp
// 1. 加载场景
await SceneModule.Instance.LoadSceneAsync("Scenes/Game");

// 2. 设置场景顺序（决定哪个场景是激活状态）
SceneModule.Instance.SetSceneOrder("Scenes/UI", 100);

// 3. 卸载场景
await SceneModule.Instance.UnloadSceneAsync("Scenes/Game");
```

**核心文件**: `Assets/HoweFramework/Scene/SceneModule.cs`

---

## 相机系统

### CameraModule API

```csharp
// 主相机
Camera MainCamera { get; }
```

### 说明

- 相机模块自动管理主相机
- 当存在多个 GameCamera 时，按优先级自动切换
- 第一个相机成为主相机并启用，其他相机禁用

**核心文件**: `Assets/HoweFramework/Camera/CameraModule.cs`

---

## Web请求系统

### WebRequestModule API

```csharp
// 设置辅助器
WebRequestModule.Instance.SetWebRequestHelper(IWebRequestHelper webRequestHelper);
```

### 请求类型

```csharp
// POST 请求
var request = new WebPostRequest
{
    Url = "https://api.example.com/data",
    RequestBody = jsonData,
    ContentType = "application/json",
    Headers = new Dictionary<string, string>()
};

// GET 请求
var request = new WebGetRequest
{
    Url = "https://api.example.com/data",
    Headers = new Dictionary<string, string>(),
    Parameters = new Dictionary<string, string>()
};
```

### 使用示例

```csharp
// 1. 设置辅助器
WebRequestModule.Instance.SetWebRequestHelper(new UnityWebRequestHelper());

// 2. 发送 POST 请求
var postRequest = new WebPostRequest
{
    Url = "https://api.example.com/login",
    RequestBody = "{\"username\":\"test\"}",
    ContentType = "application/json"
};
var response = await WebRequestModule.Instance.Post(postRequest, CancellationToken.None);

// 3. 发送 GET 请求
var getRequest = new WebGetRequest
{
    Url = "https://api.example.com/user",
    Parameters = new Dictionary<string, string> { { "id", "1" } }
};
var getResponse = await WebRequestModule.Instance.Get(getRequest, CancellationToken.None);
```

**核心文件**: `Assets/HoweFramework/WebRequest/WebRequestModule.cs`

---

## 安全区域系统

### SafeAreaModule API

```csharp
// 安全区域范围
Rect SafeArea { get; }
```

### 说明

- 自动监听设备安全区域变化（刘海屏等）
- 通过 EventModule 订阅 `SafeAreaChangeEventArgs` 事件

### 使用示例

```csharp
// 订阅安全区域变化事件
EventModule.Instance.Subscribe(EventId.SafeAreaChange, OnSafeAreaChanged);

private void OnSafeAreaChanged(object sender, GameEventArgs e)
{
    var eventArgs = (SafeAreaChangeEventArgs)e;
    Rect safeArea = eventArgs.SafeArea;
    // 应用安全区域到 UI
}
```

**核心文件**: `Assets/HoweFramework/SafeArea/SafeAreaModule.cs`

---

## 行为树系统

### BehaviorModule API

```csharp
// 全局行为树加载器
IBehaviorLoader GlobalBehaviorLoader { get; }

// 创建行为树加载器
IBehaviorLoader CreateBehaviorLoader(IResLoader resLoader = null);
```

### 说明

- 提供行为树的加载和执行能力
- 通常配合 Gameplay 框架中的 AI 组件使用

**核心文件**: `Assets/HoweFramework/BehaviorTree/BehaviorModule.cs`

---

## 系统模块

### SystemModule API

```csharp
// 获取系统
T GetSystem<T>() where T : ISystem;

// 注册系统（泛型）
T RegisterSystem<T>() where T : ISystem, new();

// 注册系统（接口+实现）
TInterface RegisterSystem<TInterface, TSystem>() where TInterface : ISystem where TSystem : TInterface, new();

// 注册系统（实例）
void RegisterSystem<T>(T system) where T : ISystem;

// 销毁系统
void DestroySystem<T>() where T : ISystem;

// 系统销毁事件
event Action<ISystem> OnSystemDestroyed;
```

### ISystem 接口

```csharp
public interface ISystem
{
    void Init();
    void Destroy();
}
```

### 使用示例

```csharp
// 1. 定义系统
public class MySystem : ISystem
{
    public void Init() { /* 初始化 */ }
    public void Destroy() { /* 清理 */ }
}

// 2. 注册系统
SystemModule.Instance.RegisterSystem<MySystem>();

// 3. 获取系统
var system = SystemModule.Instance.GetSystem<MySystem>();

// 4. 销毁系统
SystemModule.Instance.DestroySystem<MySystem>();
```

**核心文件**: `Assets/HoweFramework/System/SystemModule.cs`

---

## 远程请求调度器

### RemoteRequestModule API

```csharp
// 创建远程请求调度器
IRemoteRequestDispatcher CreateRemoteRequestDispatcher();
```

### 说明

- 用于管理网络请求的发送和接收
- 通常与 NetworkModule 配合使用

**核心文件**: `Assets/HoweFramework/RemoteRequest/RequestModule.cs`

---

## 错误码规范

### 框架错误码

定义在 `Assets/HoweFramework/FrameworkErrorCode.cs`:

| 范围 | 分类 |
|------|------|
| 0 | 成功 |
| 1 | 异常 |
| 2 | 框架异常 |
| 3 | 参数无效 |
| 4 | 操作无效 |
| 5 | 未知错误 |
| 100-199 | UI |
| 200-299 | 资源 |
| 300-399 | 流程 |
| 400-499 | 网络 |
| 500-599 | Web请求 |
| 600-699 | 声音 |
| 700-799 | 请求 |
| 800-899 | 登录 |
| 900-999 | 行为树 |

### 游戏错误码

定义在 `Assets/GameMain/Scripts/ErrorCode.cs`:

```csharp
public static class ErrorCode
{
    public const int Success = FrameworkErrorCode.Success;
    // 扩展游戏特定错误码...
}
```

### 抛出异常

```csharp
throw new ErrorCodeException(FrameworkErrorCode.InvalidParam, "参数不能为空");
```

**核心文件**: `Assets/HoweFramework/Base/ErrorCodeException.cs`

---

## 代码规范

### 命名规范

| 类型 | 规范 | 示例 |
|------|------|------|
| 模块类 | `XXXModule` | `UIModule`, `ResModule` |
| 界面逻辑类 | `XXXFormLogic` | `MainMenuFormLogic` |
| 界面分组枚举 | `UIGroupId` | `Background`, `Main` |
| 界面ID枚举 | `UIFormId` | `MainMenu`, `Settings` |
| 流程ID枚举 | `ProcedureId` | `Splash`, `Login` |
| 事件参数类 | `XXXEventArgs` | `PlayerDeadEventArgs` |
| 错误码类 | `FrameworkErrorCode` / `ErrorCode` | `Success`, `InvalidParam` |

### 目录结构

```
Assets/
├── HoweFramework/              # 框架核心
│   ├── Base/                   # 基础类 (ModuleBase, ErrorCodeException)
│   ├── IOC/                    # 依赖注入 (IOCModule, InjectAttribute)
│   ├── UI/                     # UI系统
│   │   ├── Core/               # 核心接口
│   │   └── FairyGUI/           # FairyGUI实现
│   ├── Event/                  # 事件系统
│   ├── Network/                # 网络系统
│   ├── Res/                    # 资源系统
│   ├── Procedure/              # 流程系统
│   ├── Reference/              # 引用池
│   └── ...
├── HoweFramework.Editor/       # 编辑器工具
└── GameMain/                   # 游戏逻辑
    └── Scripts/
        ├── UI/                 # 游戏界面
        │   ├── Base/           # 界面基类
        │   └── FormId.cs       # 界面ID枚举
        ├── Procedure/           # 游戏流程
        ├── Network/            # 网络协议
        ├── Sound/              # 声音配置
        └── ...
```

### Unity 层级规范

- **GameMain/Scripts** - 游戏逻辑代码
- **GameMain/Scenes** - 游戏场景
- **HoweFramework** - 框架代码 (不依赖游戏逻辑)

### 依赖方向

```
GameMain ──→ HoweFramework
            (单向依赖)
```

游戏代码可以引用框架，框架不应引用游戏代码。

---

## 快速开始

### 1. 创建游戏入口

```csharp
// Assets/GameMain/Scripts/GameEntry.cs
public class GameEntry : MonoBehaviour
{
    private GameApp _gameApp;

    private void Awake()
    {
        _gameApp = new GameApp();
    }

    private void Start()
    {
        var procedures = new ProcedureBase[]
        {
            new ProcedureSplash(),
            new ProcedureLogin(),
        };
        ProcedureModule.Instance.Launch((int)ProcedureId.Splash, procedures);
    }

    private void Update()
    {
        _gameApp.Update(Time.deltaTime, Time.unscaledDeltaTime);
    }

    private void OnDestroy()
    {
        _gameApp.Destroy();
    }
}
```

### 2. 创建界面

```csharp
// 1. 定义界面ID
public enum UIFormId
{
    MainMenu = 1,
}

// 2. 实现界面逻辑
public class MainMenuFormLogic : MainFormLogicBase
{
    public override int FormId => (int)UIFormId.MainMenu;
    // ... 实现 OnInit, OnOpen 等方法
}

// 3. 打开界面
UIModule.Instance.OpenForm((int)UIFormId.MainMenu);
```

### 3. 使用依赖注入

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

---

## 常见问题

### Q: 如何获取模块实例？
A: 使用 `XXXModule.Instance`，如 `UIModule.Instance`

### Q: 如何进行跨模块通信？
A: 使用事件系统 (`EventModule`) 或 IOC 直接注入

### Q: 界面不响应点击？
A: 检查界面分组是否正确，确认 UI 栈顺序

### Q: 如何处理资源加载？
A: 使用 `ResModule.Instance.CreateResLoader()` 创建加载器

### Q: 模块显示已初始化？
A: 检查是否重复创建模块，模块只能初始化一次
