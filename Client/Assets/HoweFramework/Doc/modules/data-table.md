# 配置表系统

## 概述

DataTableModule 管理游戏配置表（DataTable）的加载和访问。

## 核心文件

| 文件 | 路径 |
|------|------|
| DataTableModule | `Assets/HoweFramework/DataTable/DataTableModule.cs` |

## DataTableLoadMode

```csharp
public enum DataTableLoadMode
{
    LazyLoad,                  // 懒加载，访问时加载
    Preload,                   // 预加载
    LazyLoadAndPreloadAsync,   // 懒加载 + 异步预加载（默认）
}
```

## DataTableModule API

```csharp
public sealed class DataTableModule : ModuleBase<DataTableModule>
{
    // 加载模式
    public DataTableLoadMode LoadMode { get; set; }

    // 添加数据源
    public void AddDataTableSource(IDataTableSource dataTableSource);

    // 移除数据源
    public void RemoveDataTableSource(IDataTableSource dataTableSource);

    // 同步预加载
    public void Preload();

    // 异步预加载
    public UniTask PreloadAsync();
}
```

## IDataTableSource

```csharp
public interface IDataTableSource
{
    // 数据源名称
    string SourceName { get; }

    // 同步读取
    void Load();

    // 异步读取
    UniTask LoadAsync();

    // 获取所有行
    IReadOnlyList<T> GetAllRows<T>() where T : class, new();

    // 获取行数
    int GetRowCount();

    // 获取指定行
    T GetRow<T>(object key) where T : class, new();

    // 检查是否存在
    bool HasRow<T>(object key) where T : class, new();
}
```

## 基本用法

### 1. 添加数据源

```csharp
// 在 ProcedureLoadDataTable 中
var source = new GameMainDataTableSource();
DataTableModule.Instance.AddDataTableSource(source);
```

### 2. 预加载

```csharp
// 同步预加载
DataTableModule.Instance.LoadMode = DataTableLoadMode.Preload;
DataTableModule.Instance.Preload();

// 异步预加载
DataTableModule.Instance.LoadMode = DataTableLoadMode.LazyLoadAndPreloadAsync;
await DataTableModule.Instance.PreloadAsync();
```

### 3. 访问数据

```csharp
// 获取所有行
var items = dataTableSource.GetAllRows<ItemData>();

// 获取指定行
var item = dataTableSource.GetRow<ItemData>(itemId);

// 检查存在
if (dataTableSource.HasRow<ItemData>(itemId))
{
    // ...
}
```

## 数据源实现

### 创建自定义数据源

```csharp
public class GameMainDataTableSource : IDataTableSource
{
    public string SourceName => "GameMain";

    private Dictionary<Type, object> _tables = new();

    public void Load()
    {
        // 同步加载配置表
        _tables[typeof(MonsterData)] = LoadJson<MonsterData>("DataTables/monsters");
        _tables[typeof(ItemData)] = LoadJson<ItemData>("DataTables/items");
    }

    public async UniTask LoadAsync()
    {
        // 异步加载
        _tables[typeof(MonsterData)] = await LoadJsonAsync<MonsterData>("DataTables/monsters");
        _tables[typeof(ItemData)] = await LoadJsonAsync<ItemData>("DataTables/items");
    }

    public IReadOnlyList<T> GetAllRows<T>() where T : class, new()
    {
        if (_tables.TryGetValue(typeof(T), out var table))
        {
            return (List<T>)table;
        }
        return new List<T>();
    }

    public T GetRow<T>(object key) where T : class, new()
    {
        var rows = GetAllRows<T>();
        // 通过键查找
        return rows.FirstOrDefault(r => GetKey(r) == key);
    }
}
```

## 数据类定义

```csharp
public class MonsterData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int HP { get; set; }
    public int Attack { get; set; }
    public float MoveSpeed { get; set; }
}

public class ItemData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ItemType Type { get; set; }
    public int Price { get; set; }
}

public enum ItemType
{
    Weapon = 1,
    Armor = 2,
    Potion = 3,
}
```

## 完整示例

### 配置表管理器

```csharp
public class DataTableManager
{
    private IDataTableSource _source;

    public async UniTask InitializeAsync()
    {
        _source = new GameMainDataTableSource();
        DataTableModule.Instance.AddDataTableSource(_source);
        DataTableModule.Instance.LoadMode = DataTableLoadMode.LazyLoadAndPreloadAsync;
        await DataTableModule.Instance.PreloadAsync();
    }

    public MonsterData GetMonster(int id)
    {
        return _source.GetRow<MonsterData>(id);
    }

    public ItemData GetItem(int id)
    {
        return _source.GetRow<ItemData>(id);
    }

    public IReadOnlyList<MonsterData> GetAllMonsters()
    {
        return _source.GetAllRows<MonsterData>();
    }
}
```

## 最佳实践

### 1. 使用实体 ID 作为主键

```csharp
public MonsterData GetMonster(int monsterId)
{
    return _source.GetRow<MonsterData>(monsterId);
}

// 使用
var monster = DataTableManager.Instance.GetMonster(1001);
```

### 2. 缓存常用数据

```csharp
public class ItemDataManager
{
    private Dictionary<int, ItemData> _itemCache = new();

    public void CacheAllItems()
    {
        var items = DataTableManager.Instance.GetAllItems();
        foreach (var item in items)
        {
            _itemCache[item.Id] = item;
        }
    }

    public ItemData GetItem(int id)
    {
        if (_itemCache.TryGetValue(id, out var item))
            return item;
        return DataTableManager.Instance.GetItem(id);
    }
}
```

### 3. 使用只读集合

```csharp
// 返回 IReadOnlyList 防止外部修改
public IReadOnlyList<MonsterData> GetAllMonsters()
{
    return _source.GetAllRows<MonsterData>();
}
```