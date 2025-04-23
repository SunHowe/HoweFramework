using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 资源代理加载器。其内部会使用真正的资源加载器加载资源。
    /// </summary>
    internal sealed class ResProxyLoader : IResLoader, IReference
    {
        private IResLoader m_ResLoader;

        private readonly Dictionary<string, AssetItemInfo> m_AssetItemDict = new();
        private CancellationTokenSource m_CancellationTokenSource;

        public void Dispose()
        {
            m_CancellationTokenSource.Cancel();
            m_CancellationTokenSource.Dispose();
            m_CancellationTokenSource = null;

            // 归还所有资源。
            foreach (var item in m_AssetItemDict.Values)
            {
                item.Dispose();
            }

            m_AssetItemDict.Clear();

            ReferencePool.Release(this);
        }

        public void Clear()
        {
            m_ResLoader = null;
        }

        public async UniTask<Object> LoadAssetAsync(string assetKey, System.Type assetType, CancellationToken token = default)
        {
            var resLoaderToken = m_CancellationTokenSource.Token;

            if (!m_AssetItemDict.TryGetValue(assetKey, out var assetItemInfo))
            {
                assetItemInfo = AssetItemInfo.Create(assetKey, assetType, m_ResLoader.LoadAssetAsync, m_ResLoader.UnloadAsset);
                m_AssetItemDict.Add(assetKey, assetItemInfo);
            }

            var asset = await assetItemInfo.GetAssetAsync();

            if (resLoaderToken.IsCancellationRequested)
            {
                throw new ErrorCodeException(ErrorCode.ResLoaderDisposed);
            }

            if (token.IsCancellationRequested)
            {
                // 取消加载.
                assetItemInfo.Release();
                return null;
            }

            if (asset == null)
            {
                // 加载失败.
                assetItemInfo.Release();
                return null;
            }

            return asset;
        }

        public UniTask<byte[]> LoadBinaryAsync(string assetKey, CancellationToken token = default)
        {
            return m_ResLoader.LoadBinaryAsync(assetKey, token);
        }

        public byte[] LoadBinary(string assetKey)
        {
            return m_ResLoader.LoadBinary(assetKey);
        }

        public void UnloadAsset(string assetKey)
        {
            if (!m_AssetItemDict.TryGetValue(assetKey, out var assetItemInfo))
            {
                return;
            }

            assetItemInfo.Release();
        }

        public void UnloadUnusedAsset()
        {
            using var removeSet = ReusableHashSet<string>.Create();

            foreach (var item in m_AssetItemDict)
            {
                if (item.Value.AnyRef)
                {
                    continue;
                }

                removeSet.Add(item.Key);
                item.Value.Dispose();
            }

            foreach (var item in removeSet)
            {
                m_AssetItemDict.Remove(item);
            }

            m_ResLoader.UnloadUnusedAsset();
        }

        /// <summary>
        /// 创建资源代理加载器。
        /// </summary>
        /// <param name="resLoader">资源加载器。</param>
        /// <returns>资源代理加载器。</returns>
        public static ResProxyLoader Create(IResLoader resLoader)
        {
            var proxy = ReferencePool.Acquire<ResProxyLoader>();
            proxy.m_ResLoader = resLoader;
            proxy.m_CancellationTokenSource = new CancellationTokenSource();
            return proxy;
        }
    }
}
