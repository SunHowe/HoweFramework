# 对象池系统

## 概述

GameObjectPoolModule 提供 Unity GameObject 的对象池功能，减少 Instantiate/Destroy 带来的性能开销。

## 核心文件

| 文件 | 路径 |
|------|------|
| GameObjectPoolModule | `Assets/HoweFramework/GameObjectPool/GameObjectPoolModule.cs` |
| IGameObjectPool | `Assets/HoweFramework/GameObjectPool/IGameObjectPool.cs` |
| GameObjectPool | `Assets/HoweFramework/GameObjectPool/GameObjectPool.cs` |

## IGameObjectPool

```csharp
public interface IGameObjectPool : IDisposable
{
    // 同步实例化（必须先预加载）
    GameObject Instantiate(string assetKey);

    // 异步实例化
    UniTask<GameObject> InstantiateAsync(string assetKey, CancellationToken token = default);

    // 预加载
    UniTask PreloadAsync(string assetKey, int count, CancellationToken token = default);

    // 释放回池
    void Release(GameObject gameObject);

    // 缓存管理
    int GetCacheCount(string assetKey);
    void SetCacheCountLimit(string assetKey, int limit);
    void ClearCache(string assetKey);
    void ClearAllCache();
}
```

## GameObjectPoolModule API

```csharp
public sealed class GameObjectPoolModule : ModuleBase<GameObjectPoolModule>
{
    // 创建新的对象池
    public IGameObjectPool CreateGameObjectPool(IResLoader resLoader = null);

    // 全局实例化
    public UniTask<GameObject> InstantiateAsync(string assetKey, CancellationToken token = default);

    // 全局预加载
    public UniTask PreloadAsync(string assetKey, int count, CancellationToken token = default);

    // 全局释放
    public void Release(GameObject gameObject);

    // 缓存信息
    public int GetCacheCount(string assetKey);
    public void SetCacheCountLimit(string assetKey, int limit);

    // 清空缓存
    public void ClearCache(string assetKey);
    public void ClearAllCache();
}
```

## 基本用法

### 1. 预加载

```csharp
// 预加载 20 个子弹到池中
await GameObjectPoolModule.Instance.PreloadAsync("Prefabs/Bullet", 20);
```

### 2. 异步获取

```csharp
// 从池中获取（如果没有缓存则异步加载）
var bullet = await GameObjectPoolModule.Instance.InstantiateAsync("Prefabs/Bullet");

// 设置位置
bullet.transform.position = spawnPoint;
bullet.transform.rotation = Quaternion.identity;
```

### 3. 释放回池

```csharp
// 使用完毕后释放
GameObjectPoolModule.Instance.Release(bullet);
```

### 4. 同步实例化（需要预加载）

```csharp
// 先预加载
await GameObjectPoolModule.Instance.PreloadAsync("Prefabs/Explosion", 5);

// 同步获取（更快）
var explosion = GameObjectPoolModule.Instance.Instantiate("Prefabs/Explosion");
```

## 内部机制

### PooledGameObject 组件

池化的 GameObject 会自动添加 `PooledGameObject` 组件：

```csharp
private sealed class PooledGameObject : MonoBehaviour, ICustomDestroy
{
    public string AssetKey { get; set; }
    public IGameObjectPool GameObjectPool { get; set; }

    public void CustomDestroy()
    {
        if (GameObjectPool != null)
            GameObjectPool.Release(gameObject);
        else
            Object.Destroy(gameObject);
    }
}
```

### ICustomDestroy 接口

```csharp
public interface ICustomDestroy
{
    void CustomDestroy();
}
```

使用 `CustomDestroy()` 而非直接 `Destroy()` 可以正确归还到池。

## 完整示例

### 子弹系统

```csharp
public class BulletManager
{
    private const string BulletPath = "Prefabs/Bullet";

    public async UniTask InitializeAsync()
    {
        // 预加载 100 发子弹
        await GameObjectPoolModule.Instance.PreloadAsync(BulletPath, 100);
    }

    public Bullet Spawn(Vector3 position, Vector3 direction)
    {
        var bulletGO = GameObjectPoolModule.Instance.Instantiate(BulletPath);
        bulletGO.transform.position = position;
        bulletGO.transform.rotation = Quaternion.LookRotation(direction);

        var bullet = bulletGO.GetComponent<Bullet>();
        bullet.Initialize(direction);

        return bullet;
    }

    public void Despawn(Bullet bullet)
    {
        GameObjectPoolModule.Instance.Release(bullet.gameObject);
    }
}

public class Bullet : MonoBehaviour
{
    private Vector3 _direction;

    public void Initialize(Vector3 direction)
    {
        _direction = direction;
    }

    private void Update()
    {
        transform.position += _direction * Time.deltaTime * 100f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 碰撞后回池
        GameObjectPoolModule.Instance.Release(gameObject);
    }
}
```

### 特效系统

```csharp
public class EffectManager
{
    private IGameObjectPool _pool;

    public async UniTask InitializeAsync()
    {
        _pool = GameObjectPoolModule.Instance.CreateGameObjectPool();

        // 预加载特效
        await _pool.PreloadAsync("Prefabs/Effects/Explosion", 10);
        await _pool.PreloadAsync("Prefabs/Effects/Hit", 20);
    }

    public async UniTask PlayEffectAsync(string effectPath, Vector3 position)
    {
        var effect = await _pool.InstantiateAsync(effectPath);
        effect.transform.position = position;

        // 等待特效播放完毕
        var particle = effect.GetComponent<ParticleSystem>();
        if (particle != null)
        {
            await UniTask.Delay((int)(particle.main.duration * 1000));
        }

        _pool.Release(effect);
    }

    public void Release(ParticleSystem effect)
    {
        _pool.Release(effect.gameObject);
    }
}
```

## 缓存限制

```csharp
// 设置最大缓存数量
GameObjectPoolModule.Instance.SetCacheCountLimit("Prefabs/Bullet", 50);

// 获取当前缓存数量
int count = GameObjectPoolModule.Instance.GetCacheCount("Prefabs/Bullet");

// 超过限制时，最旧的会被销毁
```

## 最佳实践

### 1. 预加载在游戏初始化时完成

```csharp
public class GameInitializer
{
    public async UniTask InitializeAsync()
    {
        // 等待所有资源预加载完成
        await UniTask.WhenAll(
            GameObjectPoolModule.Instance.PreloadAsync("Prefabs/Bullet", 100),
            GameObjectPoolModule.Instance.PreloadAsync("Prefabs/Enemy", 50),
            GameObjectPoolModule.Instance.PreloadAsync("Prefabs/Effect/Explosion", 20)
        );
    }
}
```

### 2. 使用合适的预加载数量

```csharp
// 根据预估同时存在数量决定
await GameObjectPoolModule.Instance.PreloadAsync("Prefabs/Bullet", maxBulletsExpected * 2);

// 不要过度预加载，浪费内存
// 也不要太少，导致频繁加载
```

### 3. 对象复用而非重建

```csharp
// 好：复用
var bullet = await _pool.InstantiateAsync("Prefabs/Bullet");
bullet.transform.position = newPos;
bullet.SetActive(true);

// 不好：每次都销毁重建
Destroy(bullet);
var bullet = Instantiate(prefab);
```

### 4. 特效播放完毕后自动回池

```csharp
public class AutoReturnToPool : MonoBehaviour
{
    public string AssetKey;
    public IGameObjectPool Pool;

    private ParticleSystem _particle;

    private void Awake()
    {
        _particle = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (_particle.isPlaying)
            return;

        Pool.Release(gameObject);
    }
}
```

## 与 ResModule 的区别

| 特性 | ResModule | GameObjectPoolModule |
|------|-----------|---------------------|
| 对象类型 | Asset (Texture, Prefab) | GameObject |
| 获取方式 | 加载 | 从池获取或加载 |
| 释放方式 | UnloadAsset | Release |
| 性能 | 有 IO 开销 | 无 IO（复用） |
| 适用场景 | 资源常驻 | 频繁创建销毁 |