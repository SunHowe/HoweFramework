# 基础工具

## 概述

BaseModule 提供基础的 Json 序列化和文本模板功能。

## 核心文件

| 文件 | 路径 |
|------|------|
| BaseModule | `Assets/HoweFramework/Base/BaseModule.cs` |

## BaseModule API

```csharp
public sealed class BaseModule : ModuleBase<BaseModule>
{
    // 设置 JSON 序列化工具
    public static void SetJsonHelper(IJsonHelper jsonHelper);

    // 设置文本模板工具
    public static void SetTextTemplateHelper(ITextTemplateHelper textTemplateHelper);
}
```

## IJsonHelper

```csharp
public interface IJsonHelper
{
    // 序列化
    string ToJson(object obj);

    // 反序列化
    T FromJson<T>(string json);
}
```

## ITextTemplateHelper

```csharp
public interface ITextTemplateHelper
{
    // 解析模板
    string ParseTemplate(string template, Dictionary<string, string> variables);
}
```

## 基本用法

### Json 序列化

```csharp
// 设置序列化工具
BaseModule.SetJsonHelper(new MyJsonHelper());

// 序列化
var data = new MyData { Name = "test", Value = 123 };
string json = JsonHelper.ToJson(data);

// 反序列化
var loaded = JsonHelper.FromJson<MyData>(json);
```

### 文本模板

```csharp
// 设置模板工具
BaseModule.SetTextTemplateHelper(new MyTextTemplateHelper());

// 定义模板
string template = "Hello, ${Name}! You have ${Count} items.";

// 解析
string result = TextTemplateHelper.ParseTemplate(template, new Dictionary<string, string>
{
    { "Name", "Player" },
    { "Count", "5" }
});

// 结果: "Hello, Player! You have 5 items."
```

## 内置工具

框架可能内置了基于 Unity JsonUtility 或 Newtonsoft.Json 的实现：

```csharp
// Unity 内置
public class UnityJsonHelper : IJsonHelper
{
    public string ToJson(object obj)
    {
        return JsonUtility.ToJson(obj);
    }

    public T FromJson<T>(string json)
    {
        return JsonUtility.FromJson<T>(json);
    }
}
```

## 常用数据结构

### 可序列化类

```csharp
[Serializable]
public class PlayerData
{
    public string Name;
    public int Level;
    public List<ItemData> Items;
}

[Serializable]
public class ItemData
{
    public int Id;
    public int Count;
}
```

## 最佳实践

### 1. 使用 Dictionary 存储键值对

```csharp
// 好
var variables = new Dictionary<string, string>
{
    ["PlayerName"] = player.Name,
    ["Score"] = player.Score.ToString()
};
string result = templateHelper.ParseTemplate(template, variables);
```

### 2. 检查 null

```csharp
public string ToJson(object obj)
{
    if (obj == null)
        return "null";
    return JsonUtility.ToJson(obj);
}
```

### 3. 使用嵌套字典处理复杂模板

```csharp
// 模板: ${user.name} - ${user.level}
var context = new Dictionary<string, object>
{
    ["user"] = new Dictionary<string, string>
    {
        ["name"] = player.Name,
        ["level"] = player.Level.ToString()
    }
};
```