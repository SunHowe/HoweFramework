# 引用池

## 概述

ReferencePool 提供对象的复用池，避免频繁的 GC 分配和销毁。

## 核心文件

| 文件 | 路径 |
|------|------|
| ReferencePool | `Assets/HoweFramework/Reference/ReferencePool.cs` |
| IReference | `Assets/HoweFramework/Reference/IReference.cs` |
| IReferenceWithId | `Assets/HoweFramework/Reference/IReferenceWithId.cs` |
| ReferenceCache | `Assets/HoweFramework/Reference/ReferenceCache.cs` |

## IReference 接口

```csharp
public interface IReference
{
    void Clear();
}
```

实现此接口的类可以被放入池中复用。

## IReferenceWithId 接口

```csharp
public interface IReferenceWithId : IReference
{
    int InstanceId { get; set; } // 框架自动分配
}
```

用于需要唯一标识的对象池。

## ReferencePool API

```csharp
public static class ReferencePool
{
    // 从池中获取实例（池空时创建新实例）
    public static T Acquire<T>() where T : class, IReference, new();
    public static IReference Acquire(Type type);

    // 归还实例到池
    public static void Release(IReference instance);

    // 清空指定类型的缓存
    public static void ClearCache<T>() where T : class, IReference;

    // 清空所有缓存
    public static void ClearAllCache();
}
```

## 基本用法

### 1. 定义可池化类

```csharp
public class EffectData : IReference
{
    public int EffectId { get; set; }
    public Vector3 Position { get; set; }
    public float Duration { get; set; }

    public void Clear()
    {
        EffectId = 0;
        Position = Vector3.zero;
        Duration = 0f;
    }
}
```

### 2. 获取和归还

```csharp
// 获取
var data = ReferencePool.Acquire<EffectData>();
data.EffectId = 1001;
data.Position = new Vector3(1, 2, 3);

// 使用...

// 归还到池
ReferencePool.Release(data);
```

### 3. 工厂方法模式（推荐）

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

    // 工厂方法
    public static EffectData Create(int effectId, Vector3 position)
    {
        var data = ReferencePool.Acquire<EffectData>();
        data.EffectId = effectId;
        data.Position = position;
        return data;
    }
}

// 使用
var data = EffectData.Create(1001, new Vector3(1, 2, 3));
// 使用完毕后
ReferencePool.Release(data);
```

## 与 GameEventArgs 集成

所有 `GameEventArgs` 默认支持池化：

```csharp
public abstract class GameEventArgs : EventArgs, IReference
{
    // 触发后自动归还（默认开启）
    public virtual bool IsReleaseAfterFire => true;

    public void SetIsReleaseAfterFire(bool isReleaseAfterFire);
}

public sealed class MyEventArgs : GameEventArgs
{
    public static readonly int EventId = typeof(MyEventArgs).GetHashCode();
    public override int Id => EventId;

    public int Value { get; set; }

    public override void Clear()
    {
        Value = 0;
    }

    public static MyEventArgs Create(int value)
    {
        var args = ReferencePool.Acquire<MyEventArgs>();
        args.Value = value;
        return args;
    }
}

// 分发（自动归还）
EventModule.Instance.Dispatch(this, MyEventArgs.Create(100));
// 无需手动 Release
```

## ReferenceCache 内部实现

```csharp
internal sealed class ReferenceCache<T> where T : class, IReference, new()
{
    private readonly Stack<T> m_Stack = new();

    public T Acquire()
    {
        return m_Stack.Count > 0 ? m_Stack.Pop() : new T();
    }

    public void Release(T instance)
    {
        instance.Clear();
        m_Stack.Push(instance);
    }
}
```

## 带 Id 的池

```csharp
public interface IReferenceWithId : IReference
{
    int InstanceId { get; set; }
}

// 使用 ReferenceWithIdCache
internal sealed class ReferenceWithIdCache<T> where T : class, IReferenceWithId, new()
{
    private readonly Dictionary<int, T> m_CacheDict = new();
    private readonly Stack<int> m_IdStack = new();
    private int m_NextId = 0;

    public T Acquire()
    {
        T instance = m_Stack.Count > 0 ? m_Stack.Pop() : new T();
        instance.InstanceId = m_NextId++;
        return instance;
    }
}
```

## 最佳实践

### 1. 始终实现 Clear 方法

```csharp
public void Clear()
{
    // 重置所有字段
    Id = 0;
    Name = null;
    Data = null;
}
```

### 2. 创建工厂方法

```csharp
public static MyClass Create(T1 arg1, T2 arg2)
{
    var obj = ReferencePool.Acquire<MyClass>();
    obj.Field1 = arg1;
    obj.Field2 = arg2;
    return obj;
}
```

### 3. 组合替代继承时的池化

如果类继承自具体类（如 Unity 的 `MonoBehaviour`），不能直接池化。可以使用组件组合：

```csharp
// 不能池化 MonoBehaviour
public class MyMB : MonoBehaviour { }

// 但可以池化数据类
public class MyData : IReference { }

// 在 MonoBehaviour 中使用数据类
public class MyMB : MonoBehaviour
{
    private MyData _data;

    public void SetData(MyData data)
    {
        _data = data;
    }
}
```

### 4. 避免在池化对象中持有 Unity 资源引用

```csharp
// 不好
public class EffectData : IReference
{
    public GameObject Prefab { get; set; } // Unity 对象
    public void Clear() { Prefab = null; }
}

// 好：只持有资源路径或键
public class EffectData : IReference
{
    public string PrefabPath { get; set; }
    public void Clear() { PrefabPath = null; }
}
```

## 性能优势

| 操作 | 栈分配 | 池化 |
|------|--------|------|
| 获取 | ~1ms | ~0.01ms |
| 归还 | 触发 GC | ~0.01ms |
| GC 压力 | 高 | 极低 |

## 清空缓存

```csharp
// 清空特定类型
ReferencePool.ClearCache<MyClass>();

// 清空所有
ReferencePool.ClearAllCache();
```

通常在场景切换或游戏退出时调用。