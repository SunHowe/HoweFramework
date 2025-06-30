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
            m_Root = null;
        }

        public void ClearAllCache()
        {
            foreach (var queue in m_GameObjectDict.Values)
            {
                while (queue.Count > 0)
                {
                    var gameObject = queue.Dequeue();
                    if (gameObject != null)
                    {
                        Object.Destroy(gameObject);
                    }
                }
            }

            m_GameObjectDict.Clear();
        }

        public void ClearCache(string assetKey)
        {
            if (!m_GameObjectDict.TryGetValue(assetKey, out var gameObjects))
            {
                return;
            }

            while (gameObjects.Count > 0)
            {
                var gameObject = gameObjects.Dequeue();
                if (gameObject != null)
                {
                    Object.Destroy(gameObject);
                }
            }

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

            var prefab = await m_ResLoader.LoadAssetAsync<GameObject>(assetKey, token);
            if (prefab == null)
            {
                throw new ErrorCodeException(ErrorCode.ResNotFound, $"Load asset '{assetKey}' failed.");
            }

            gameObject = Object.Instantiate(prefab);
            gameObject.GetOrAddComponent<PooledGameObject>().AssetKey = assetKey;
            return gameObject;
        }

        public async UniTask PreloadAsync(string assetKey, int count, CancellationToken token = default)
        {
            if (count <= 0)
            {
                return;
            }

            if (m_CacheCountLimitDict.TryGetValue(assetKey, out var limit) && count > limit)
            {
                count = limit;
            }

            if (!m_GameObjectDict.TryGetValue(assetKey, out var gameObjects))
            {
                gameObjects = ReusableQueue<GameObject>.Create();
                m_GameObjectDict.Add(assetKey, gameObjects);
            }
            else
            {
                count -= gameObjects.Count;

                if (count <= 0)
                {
                    return;
                }
            }

            var prefab = await m_ResLoader.LoadAssetAsync<GameObject>(assetKey, token);
            if (prefab == null)
            {
                throw new ErrorCodeException(ErrorCode.ResNotFound, $"Load asset '{assetKey}' failed.");
            }

            while (count > 0)
            {
                --count;

                var gameObject = Object.Instantiate(prefab);
                gameObject.GetOrAddComponent<PooledGameObject>().AssetKey = assetKey;
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
            
            var component = gameObject.GetComponent<PooledGameObject>();
            if (component == null || string.IsNullOrEmpty(component.AssetKey))
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, $"Invalid game object '{gameObject.name}'.");
            }

            if (!m_GameObjectDict.TryGetValue(component.AssetKey, out var gameObjects))
            {
                gameObjects = ReusableQueue<GameObject>.Create();
                m_GameObjectDict.Add(component.AssetKey, gameObjects);
            }
            else if (m_CacheCountLimitDict.TryGetValue(component.AssetKey, out var limit) && gameObjects.Count >= limit)
            {
                // 超过缓存数量限制，销毁对象。
                Object.Destroy(gameObject);
                return;
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

        private sealed class PooledGameObject : MonoBehaviour
        {
            public string AssetKey { get; set; }
        }
    }
}
