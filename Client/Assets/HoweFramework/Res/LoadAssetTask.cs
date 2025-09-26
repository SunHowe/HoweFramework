using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 加载资源任务。
    /// </summary>
    internal sealed class LoadAssetTask : ILoadable, IDisposable, IReference
    {
        private IResLoader m_ResLoader;
        private string m_AssetKey;
        private Type m_AssetType;

        public void Clear()
        {
            m_ResLoader = null;
            m_AssetKey = null;
            m_AssetType = null;
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public async UniTask LoadAsync(LoadableProgress onProgress = null, CancellationToken cancellationToken = default)
        {
            await m_ResLoader.LoadAssetAsync(m_AssetKey, m_AssetType, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            onProgress?.Invoke(1f);
        }
        
        public static LoadAssetTask Create(IResLoader resLoader, string assetKey, Type assetType)
        {
            if (resLoader == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, nameof(resLoader));
            }

            if (string.IsNullOrEmpty(assetKey))
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, nameof(assetKey));
            }
            
            if (assetType == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, nameof(assetType));
            }

            var loadAssetTask = ReferencePool.Acquire<LoadAssetTask>();
            loadAssetTask.m_ResLoader = resLoader;
            loadAssetTask.m_AssetKey = assetKey;
            loadAssetTask.m_AssetType = assetType;
            return loadAssetTask;
        }
    }

    /// <summary>
    /// 加载资源任务扩展。
    /// </summary>
    public static class LoadAssetTaskExtension
    {
        /// <summary>
        /// 创建加载资源任务。
        /// </summary>
        /// <param name="resLoader">资源加载器。</param>
        /// <param name="assetKey">资源路径。</param>
        /// <param name="assetType">资源类型。</param>
        /// <returns>加载资源任务。</returns>
        public static ILoadable CreateLoadAssetTask(this IResLoader resLoader, string assetKey, Type assetType)
        {
            return LoadAssetTask.Create(resLoader, assetKey, assetType);
        }
    }
}