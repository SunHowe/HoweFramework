using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 游戏对象池模块。
    /// </summary>
    public sealed class GameObjectPoolModule : ModuleBase<GameObjectPoolModule>
    {
        /// <summary>
        /// 全局游戏对象池。
        /// </summary>
        private IGameObjectPool m_GameObjectPool;

        /// <summary>
        /// 创建游戏对象池。当不需要使用时，调用Dispose方法释放。
        /// </summary>
        /// <param name="resLoader">资源加载器。若为null，则会分配一个新的资源加载器。</param>
        public IGameObjectPool CreateGameObjectPool(IResLoader resLoader = null)
        {
            return GameObjectPool.Create(resLoader);
        }

        /// <summary>
        /// 异步实例化游戏对象。
        /// </summary>
        /// <param name="assetKey">资源Key。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>实例化后的游戏对象。</returns>
        public UniTask<GameObject> InstantatieAsync(string assetKey, CancellationToken token = default) => m_GameObjectPool.InstantatieAsync(assetKey, token);

        /// <summary>
        /// 预加载游戏对象。
        /// </summary>
        /// <param name="assetKey">资源Key。</param>
        /// <param name="count">预加载数量。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>预加载后的游戏对象。</returns>
        public UniTask PreloadAsync(string assetKey, int count, CancellationToken token = default) => m_GameObjectPool.PreloadAsync(assetKey, count, token);

        /// <summary>
        /// 释放游戏对象。
        /// </summary>
        /// <param name="gameObject">游戏对象。</param>
        public void Release(GameObject gameObject) => m_GameObjectPool.Release(gameObject);

        /// <summary>
        /// 获取缓存数量。
        /// </summary>
        /// <param name="assetKey">资源Key。</param>
        /// <returns>缓存数量。</returns>
        public int GetCacheCount(string assetKey) => m_GameObjectPool.GetCacheCount(assetKey);

        /// <summary>
        /// 设置缓存数量限制。
        /// </summary>
        /// <param name="assetKey">资源Key。</param>
        /// <param name="limit">限制数量。</param>
        public void SetCacheCountLimit(string assetKey, int limit) => m_GameObjectPool.SetCacheCountLimit(assetKey, limit);

        /// <summary>
        /// 清理缓存。
        /// </summary>
        /// <param name="assetKey">资源Key。</param>
        public void ClearCache(string assetKey) => m_GameObjectPool.ClearCache(assetKey);

        /// <summary>
        /// 清理所有缓存。
        /// </summary>
        public void ClearAllCache() => m_GameObjectPool.ClearAllCache();

        protected override void OnInit()
        {
        }

        protected override void OnDestroy()
        {
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}
