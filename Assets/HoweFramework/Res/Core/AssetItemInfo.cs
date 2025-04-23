using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

using Object = UnityEngine.Object;

namespace HoweFramework
{
    /// <summary>
    /// 资源项信息。
    /// </summary>
    public class AssetItemInfo : IReference, IDisposable
    {
        public int ReferenceId { get; set; }

        /// <summary>
        /// 是否存在引用.
        /// </summary>
        public bool AnyRef => m_RefCount > 0;

        /// <summary>
        /// 资源实例id.
        /// </summary>
        public int AssetInstanceId { get; private set; }

        private string m_AssetKey;
        private Object m_Asset;
        private int m_RefCount;
        private Type m_AssetType;
        private int m_LoadState;

        private readonly Queue<AutoResetUniTaskCompletionSource<Object>> m_TaskQueue = new Queue<AutoResetUniTaskCompletionSource<Object>>();
        private CancellationTokenSource m_CancellationTokenSource;

        private LoadAssetAsyncDelegate m_AssetLoadDelegate;
        private UnloadAssetDelegate m_UnloadAssetDelegate;

        /// <summary>
        /// 获取资源对象.
        /// </summary>
        public UniTask<Object> GetAssetAsync()
        {
            ++m_RefCount;

            if (m_LoadState == 2)
            {
                return UniTask.FromResult(m_Asset);
            }

            var tcs = AutoResetUniTaskCompletionSource<Object>.Create();
            var task = tcs.Task;

            m_TaskQueue.Enqueue(tcs);

            if (m_LoadState == 0)
            {
                CreateLoadTask().Forget();
            }

            return task;
        }

        /// <summary>
        /// 释放资源对象.
        /// </summary>
        public void Release()
        {
            --m_RefCount;
        }

        public void Clear()
        {
            m_AssetKey = null;
            m_AssetType = null;
            m_RefCount = 0;
            m_LoadState = 0;
            AssetInstanceId = 0;

            m_AssetLoadDelegate = null;
            m_UnloadAssetDelegate = null;
        }

        public void Dispose()
        {
            if (m_CancellationTokenSource != null)
            {
                m_CancellationTokenSource.Cancel();
                m_CancellationTokenSource.Dispose();
                m_CancellationTokenSource = null;
            }

            if (m_Asset != null && m_UnloadAssetDelegate != null)
            {
                m_UnloadAssetDelegate(m_AssetKey);
                m_Asset = null;
            }

            while (m_TaskQueue.Count > 0)
            {
                var task = m_TaskQueue.Dequeue();
                task.TrySetException(new ErrorCodeException(ErrorCode.ResLoaderDisposed));
            }

            ReferencePool.Release(this);
        }

        private async UniTask CreateLoadTask()
        {
            if (m_LoadState != 0)
            {
                return;
            }

            m_LoadState = 1;

            var token = m_CancellationTokenSource.Token;
            var asset = await m_AssetLoadDelegate(m_AssetKey, m_AssetType, token);

            if (token.IsCancellationRequested)
            {
                return;
            }

            m_Asset = asset;
            m_LoadState = 2;
            AssetInstanceId = m_Asset != null ? AssetInstanceId.GetHashCode() : 0;

            while (m_TaskQueue.Count > 0)
            {
                var task = m_TaskQueue.Dequeue();
                task.TrySetResult(asset);
            }
        }

        public static AssetItemInfo Create(string assetKey, Type assetType, LoadAssetAsyncDelegate assetLoadDelegate, UnloadAssetDelegate unloadAssetDelegate)
        {
            var assetItemInfo = ReferencePool.Acquire<AssetItemInfo>();
            assetItemInfo.m_AssetKey = assetKey;
            assetItemInfo.m_AssetType = assetType;

            assetItemInfo.m_CancellationTokenSource = new CancellationTokenSource();
            assetItemInfo.m_AssetLoadDelegate = assetLoadDelegate;
            assetItemInfo.m_UnloadAssetDelegate = unloadAssetDelegate;

            return assetItemInfo;
        }
    }
}
