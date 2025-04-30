using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 游戏对象池接口。不需要使用时，调用Dispose方法释放。
    /// </summary>
    public interface IGameObjectPool : IDisposable
    {
        /// <summary>
        /// 异步实例化游戏对象。
        /// </summary>
        /// <param name="assetKey">资源Key。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>实例化后的游戏对象。</returns>
        UniTask<GameObject> InstantiateAsync(string assetKey, CancellationToken token = default);

        /// <summary>
        /// 预加载游戏对象。
        /// </summary>
        /// <param name="assetKey">资源Key。</param>
        /// <param name="count">预加载数量。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>预加载后的游戏对象。</returns>
        UniTask PreloadAsync(string assetKey, int count, CancellationToken token = default);

        /// <summary>
        /// 释放游戏对象。
        /// </summary>
        /// <param name="gameObject">游戏对象。</param>
        void Release(GameObject gameObject);

        /// <summary>
        /// 获取缓存数量。
        /// </summary>
        /// <param name="assetKey">资源Key。</param>
        /// <returns>缓存数量。</returns>
        int GetCacheCount(string assetKey);

        /// <summary>
        /// 设置缓存数量限制。
        /// </summary>
        /// <param name="assetKey">资源Key。</param>
        /// <param name="limit">限制数量。</param>
        void SetCacheCountLimit(string assetKey, int limit);

        /// <summary>
        /// 清理缓存。
        /// </summary>
        /// <param name="assetKey">资源Key。</param>
        void ClearCache(string assetKey);

        /// <summary>
        /// 清理所有缓存。
        /// </summary>
        void ClearAllCache();
    }
}
