# 本地化系统

## 概述

LocalizationModule 提供多语言文本管理功能。

## 核心文件

| 文件 | 路径 |
|------|------|
| LocalizationModule | `Assets/HoweFramework/Localization/LocalizationModule.cs` |

## Language 枚举

```csharp
public enum Language
{
    en,
    zh_CN,
    zh_TW,
    ja,
    Ko,
    // ...
}
```

## LocalizationModule API

```csharp
public sealed class LocalizationModule : ModuleBase<LocalizationModule>
{
    // 默认语言
    public Language DefaultLanguage { get; }

    // 当前语言
    public Language Language { get; set; }

    // 系统语言
    public Language SystemLanguage { get; }

    // 获取文本
    public string GetText(string key);

    // 添加文本
    public void AddText(string key, string text);

    // 清空文本
    public void ClearText();

    // 添加数据源
    public void AddSource(ILocalizationSource source);

    // 移除数据源
    public void RemoveSource(ILocalizationSource source);

    // 异步加载
    public UniTask LoadAsync();
}
```

## ILocalizationSource

```csharp
public interface ILocalizationSource
{
    // 源名称
    string SourceName { get; }

    // 异步加载
    UniTask LoadAsync(Language language);

    // 获取文本
    string GetText(string key);
}
```

## 基本用法

### 1. 添加数据源

```csharp
// 在 ProcedureLoadLocalization 中
var source = new LubanLocalizationSource();
LocalizationModule.Instance.AddSource(source);
await LocalizationModule.Instance.LoadAsync();
```

### 2. 设置语言

```csharp
// 设置为中文
LocalizationModule.Instance.Language = Language.zh_CN;
```

### 3. 获取文本

```csharp
string title = LocalizationModule.Instance.GetText("ui.main_menu.title");
string welcome = LocalizationModule.Instance.GetText("ui.login.welcome");
```

### 4. 运行时添加文本

```csharp
// 动态添加
LocalizationModule.Instance.AddText("custom.key", "Custom Text");
```

## 数据源实现

### 自定义数据源

```csharp
public class JsonLocalizationSource : ILocalizationSource
{
    public string SourceName => "JsonSource";

    private Dictionary<Language, Dictionary<string, string>> _texts = new();

    public async UniTask LoadAsync(Language language)
    {
        // 加载对应语言的 JSON 文件
        var json = await ResModule.Instance.LoadAssetAsync<TextAsset>($"Localization/{language}.json");
        var data = JsonUtility.FromJson<LocalizationData>(json.text);

        _texts[language] = data.ToDictionary();
    }

    public string GetText(string key)
    {
        if (_texts.TryGetValue(LocalizationModule.Instance.Language, out var dict))
        {
            if (dict.TryGetValue(key, out var text))
                return text;
        }
        return key; // 返回 key 如果未找到
    }
}
```

## 完整示例

### 本地化管理器

```csharp
public class LocalizationManager
{
    public static LocalizationManager Instance { get; private set; }

    public event Action<Language> OnLanguageChanged;

    public Language CurrentLanguage => LocalizationModule.Instance.Language;

    public void Initialize()
    {
        Instance = this;

        // 添加数据源
        var source = new JsonLocalizationSource();
        LocalizationModule.Instance.AddSource(source);

        // 设置初始语言
        SetLanguage(DetectLanguage());
    }

    public void SetLanguage(Language language)
    {
        LocalizationModule.Instance.Language = language;
        OnLanguageChanged?.Invoke(language);

        // 通知 UI 刷新
        EventModule.Instance.Dispatch(this, LanguageChangedEventArgs.Create(language));
    }

    private Language DetectLanguage()
    {
        // 使用系统语言或保存的设置
        var savedLanguage = SettingModule.Instance.GetString("Language", "en");
        if (Enum.TryParse<Language>(savedLanguage, out var lang))
            return lang;

        return LocalizationModule.Instance.SystemLanguage;
    }

    public string Get(string key)
    {
        return LocalizationModule.Instance.GetText(key);
    }

    public string Get(string key, params object[] args)
    {
        var format = Get(key);
        return string.Format(format, args);
    }
}

// 语言切换事件
public sealed class LanguageChangedEventArgs : GameEventArgs
{
    public static readonly int EventId = typeof(LanguageChangedEventArgs).GetHashCode();
    public override int Id => EventId;

    public Language Language { get; set; }

    public override void Clear()
    {
        Language = Language.en;
    }

    public static LanguageChangedEventArgs Create(Language language)
    {
        var args = ReferencePool.Acquire<LanguageChangedEventArgs>();
        args.Language = language;
        return args;
    }
}
```

### UI 中的使用

```csharp
public class LocalizedText : MonoBehaviour
{
    public string TextKey;

    private Text _text;

    private void Awake()
    {
        _text = GetComponent<Text>();
        Refresh();

        LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void OnDestroy()
    {
        LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(Language language)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (_text != null && !string.IsNullOrEmpty(TextKey))
        {
            _text.text = LocalizationManager.Instance.Get(TextKey);
        }
    }
}
```

## JSON 格式

```json
{
  "ui.main_menu.title": "Main Menu",
  "ui.main_menu.start": "Start Game",
  "ui.main_menu.settings": "Settings",
  "ui.login.welcome": "Welcome, {0}!",
  "ui.common.confirm": "Confirm",
  "ui.common.cancel": "Cancel"
}
```

## 最佳实践

### 1. 使用常量定义键名

```csharp
public static class LocalizationKeys
{
    public const string MainMenuTitle = "ui.main_menu.title";
    public const string MainMenuStart = "ui.main_menu.start";
    public const string LoginWelcome = "ui.login.welcome";
}

// 使用
string title = LocalizationManager.Instance.Get(LocalizationKeys.MainMenuTitle);
```

### 2. 支持格式化参数

```csharp
// JSON: "ui.greeting": "Hello, {0}! You have {1} messages."
string greeting = LocalizationManager.Instance.Get("ui.greeting", playerName, messageCount);
```

### 3. 语言切换时刷新 UI

```csharp
// 订阅语言切换事件
LocalizationManager.Instance.OnLanguageChanged += RefreshUI;

private void RefreshUI(Language language)
{
    // 刷新所有 LocalizedText 组件
}
```

### 4. 使用 Key 作为默认值

```csharp
public string Get(string key)
{
    var text = LocalizationModule.Instance.GetText(key);
    return string.IsNullOrEmpty(text) ? key : text;
}
```