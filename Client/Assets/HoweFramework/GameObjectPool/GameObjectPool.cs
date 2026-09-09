using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 游戏对象池。
    /// </summary>
    internal sealed class GameObjectPool : IGameObjectPool, IReference
    {
        private IResLoader m_ResLoader;
        private readonly Dictionary<string, ReusableQueue<GameObject>> m_GameObjectDict = new();
        private readonly Dictionary<string, int> m_CacheCountLimitDict = new();
        private readonly Dictionary<string, GameObject> m_PrefabDict = new();
        private Transform m_Root;

        public static GameObjectPool Create(IResLoader resLoader)
        {
            var pool = ReferencePool.Acquire<GameObjectPool>();
            pool.m_ResLoader = ResModule.Instance.CreateResLoader(resLoader);

            var gameObject = new GameObject("GameObjectPool");
            gameObject.SetActive(false);

            Object.DontDestroyOnLoad(gameObject);
            pool.m_Root = gameObject.transform;
            return pool;
        }

        public void Dispose()
        {
            ClearAllCache();

            m_PrefabDict.Clear();
            m_ResLoader.Dispose();

            if (m_Root != null)
            {
                Object.Destroy(m_Root.gameObject);
            }

            ReferencePool.Release(this);
        }

        public void Clear()
        {
            m_ResLoader = null;
            m_GameObjectDict.Clear();
            m_CacheCountLimitDict.Clear();
            m_PrefabDict.Clear();
            m_Root = null;
        }

        public void ClearAllCache()
        {
            foreach (var queue in m_GameObjectDict.Values)
            {
                DestroyCachedObjects(queue);
                queue.Dispose();
            }

            m_GameObjectDict.Clear();
        }

        public void ClearCache(string assetKey)
        {
            if (!m_GameObjectDict.TryGetValue(assetKey, out var gameObjects))
            {
                return;
            }

            DestroyCachedObjects(gameObjects);
            gameObjects.Dispose();
            m_GameObjectDict.Remove(assetKey);
        }

        public int GetCacheCount(string assetKey)
        {
            if (!m_GameObjectDict.TryGetValue(assetKey, out var gameObjects))
            {
                return 0;
            }

            return gameObjects.Count;
        }

        public GameObject Instantiate(string assetKey)
        {
            if (m_GameObjectDict.TryGetValue(assetKey, out var gameObjects))
            {
                while (gameObjects.Count > 0)
                {
                    var gameObject = gameObjects.Dequeue();
                    if (gameObject != null)
                    {
                        gameObject.SetParent(null);
                        return gameObject;
                    }
                }
            }

            return null;
        }

        public async UniTask<GameObject> InstantiateAsync(string assetKey, CancellationToken token = default)
        {
            GameObject gameObject;
            if (m_GameObjectDict.TryGetValue(assetKey, out var gameObjects))
            {
                while (gameObjects.Count > 0)
                {
                    gameObject = gameObjects.Dequeue();
                    if (gameObject != null)
                    {
                        gameObject.SetParent(null);
                        return gameObject;
                    }
                }
            }

            var prefab = await EnsurePrefabLoadedAsync(assetKey, token);
            if (m_Root == null)
            {
                throw new ErrorCodeException(FrameworkErrorCode.InvalidOperationException, "GameObjectPool has been disposed.");
            }

            gameObject = Object.Instantiate(prefab);

            var pooledGameObject = gameObject.GetOrAddComponent<PooledGameObject>();
            pooledGameObject.AssetKey = assetKey;
            pooledGameObject.GameObjectPool = this;

            return gameObject;
        }

        public async UniTask PreloadAsync(string assetKey, int count, CancellationToken token = default)
        {
            if (count <= 0)
            {
                return;
            }

            int target = GetPreloadTargetCount(assetKey, count);
            if (GetCacheCount(assetKey) >= target)
            {
                return;
            }

            var prefab = await EnsurePrefabLoadedAsync(assetKey, token);
            if (m_Root == null)
            {
                return;
            }

            target = GetPreloadTargetCount(assetKey, count);
            if (!m_GameObjectDict.TryGetValue(assetKey, out var gameObjects))
            {
                gameObjects = ReusableQueue<GameObject>.Create();
                m_GameObjectDict.Add(assetKey, gameObjects);
            }

            int toCreate = target - gameObjects.Count;
            while (toCreate > 0)
            {
                --toCreate;

                var gameObject = Object.Instantiate(prefab);
                var pooledGameObject = gameObject.GetOrAddComponent<PooledGameObject>();
                pooledGameObject.AssetKey = assetKey;
                pooledGameObject.GameObjectPool = this;

                gameObject.transform.SetParent(m_Root);
                gameObjects.Enqueue(gameObject);
            }
        }

        public void Release(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }
            
            var pooledGameObject = gameObject.GetComponent<PooledGameObject>();
            if (pooledGameObject == null)
            {
                Object.Destroy(gameObject);
                Log.Error($"Invalid pooled game object '{gameObject.name}'.");
                return;
            }

            if (string.IsNullOrEmpty(pooledGameObject.AssetKey) || pooledGameObject.GameObjectPool != this)
            {
                Object.Destroy(gameObject);
                Log.Error($"Invalid pooled game object '{gameObject.name}'.");
                return;
            }

            // 池已销毁（根节点已 Destroy 或 Clear），归还的对象直接销毁，避免泄漏。
            if (m_Root == null)
            {
                Object.Destroy(gameObject);
                return;
            }

            if (!m_GameObjectDict.TryGetValue(pooledGameObject.AssetKey, out var gameObjects))
            {
                gameObjects = ReusableQueue<GameObject>.Create();
                m_GameObjectDict.Add(pooledGameObject.AssetKey, gameObjects);
            }
            else
            {
                // 重复归还检测：同一实例重复入池会导致后续 Instantiate 出同一对象两次。
                if (gameObjects.Contains(gameObject))
                {
                    Log.Error($"Pooled game object '{gameObject.name}' is already in pool.");
                    return;
                }

                if (m_CacheCountLimitDict.TryGetValue(pooledGameObject.AssetKey, out var limit) && gameObjects.Count >= limit)
                {
                    // 超过缓存数量限制，销毁对象。
                    Object.Destroy(gameObject);
                    Log.Debug($"Pooled game object '{gameObject.name}' is over cache count limit.");
                    return;
                }
            }

            gameObject.transform.SetParent(m_Root);
            gameObjects.Enqueue(gameObject);
        }

        public void SetCacheCountLimit(string assetKey, int limit)
        {
            if (limit <= 0)
            {
                m_CacheCountLimitDict.Remove(assetKey);
            }
            else
            {
                m_CacheCountLimitDict[assetKey] = limit;
            }
        }

        /// <summary>
        /// 每个资源 key 只 LoadAsset 一次，避免缓存未命中时反复加引用。预制体随池 Dispose（ResLoader）一并卸载。
        /// </summary>
        private async UniTask<GameObject> EnsurePrefabLoadedAsync(string assetKey, CancellationToken token)
        {
            if (m_PrefabDict.TryGetValue(assetKey, out var cached) && cached != null)
            {
                return cached;
            }

            var prefab = await m_ResLoader.LoadAssetAsync<GameObject>(assetKey, token);
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            if (prefab == null)
            {
                throw new ErrorCodeException(FrameworkErrorCode.ResNotFound, $"Load asset '{assetKey}' failed.");
            }

            if (m_PrefabDict.TryGetValue(assetKey, out cached) && cached != null)
            {
                // 并发加载同一 key 会多一次引用，卸掉多余的那次。
                m_ResLoader.UnloadAsset(assetKey);
                return cached;
            }

            m_PrefabDict[assetKey] = prefab;
            return prefab;
        }

        private int GetPreloadTargetCount(string assetKey, int count)
        {
            if (m_CacheCountLimitDict.TryGetValue(assetKey, out var limit) && count > limit)
            {
                return limit;
            }

            return count;
        }

        private static void DestroyCachedObjects(ReusableQueue<GameObject> gameObjects)
        {
            while (gameObjects.Count > 0)
            {
                var gameObject = gameObjects.Dequeue();
                if (gameObject != null)
                {
                    Object.Destroy(gameObject);
                }
            }
        }

        private sealed class PooledGameObject : MonoBehaviour, ICustomDestroy
        {
            public string AssetKey { get; set; }
            public IGameObjectPool GameObjectPool { get; set; }

            public void CustomDestroy()
            {
                if (GameObjectPool != null)
                {
                    GameObjectPool.Release(gameObject);
                }
                else
                {
                    Object.Destroy(gameObject);
                }
            }
        }
    }
}
