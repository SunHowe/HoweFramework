using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 游戏对象预加载任务。
    /// </summary>
    internal sealed class GameObjectPreloadTask : ILoadable, IDisposable, IReference
    {
        private IGameObjectPool m_GameObjectPool;
        private string m_AssetKey;
        private int m_Count;

        public void Clear()
        {
            m_AssetKey = null;
            m_Count = 0;
            m_GameObjectPool = null;
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public async UniTask LoadAsync(LoadableProgress onProgress = null, CancellationToken cancellationToken = default)
        {
            await m_GameObjectPool.PreloadAsync(m_AssetKey, m_Count, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            onProgress?.Invoke(1f);
        }

        /// <summary>
        /// 创建游戏对象预加载任务。
        /// </summary>
        /// <param name="gameObjectPool">游戏对象池。</param>
        /// <param name="assetKey">资源Key。</param>
        /// <param name="count">预加载数量。</param>
        /// <returns>创建的游戏对象预加载任务。</returns>
        /// <exception cref="ErrorCodeException"></exception>
        public static GameObjectPreloadTask Create(IGameObjectPool gameObjectPool, string assetKey, int count)
        {
            if (gameObjectPool == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, nameof(gameObjectPool));
            }

            if (string.IsNullOrEmpty(assetKey))
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, nameof(assetKey));
            }
            
            if (count <= 0)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, nameof(count));
            }

            var gameObjectPreloadTask = ReferencePool.Acquire<GameObjectPreloadTask>();
            gameObjectPreloadTask.m_GameObjectPool = gameObjectPool;
            gameObjectPreloadTask.m_AssetKey = assetKey;
            gameObjectPreloadTask.m_Count = count;
            return gameObjectPreloadTask;
        }
    }

    /// <summary>
    /// 游戏对象预加载任务扩展。
    /// </summary>
    public static class GameObjectPreloadTaskExtension
    {
        /// <summary>
        /// 创建游戏对象预加载任务。
        /// </summary>
        /// <param name="gameObjectPool">游戏对象池。</param>
        /// <param name="assetKey">资源Key。</param>
        /// <param name="count">预加载数量。</param>
        /// <returns>创建的游戏对象预加载任务。</returns>
        public static ILoadable CreatePreloadTask(this IGameObjectPool gameObjectPool, string assetKey, int count)
        {
            return GameObjectPreloadTask.Create(gameObjectPool, assetKey, count);
        }
    }
}