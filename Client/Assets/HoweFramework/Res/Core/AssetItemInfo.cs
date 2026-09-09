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
        /// <summary>
        /// 是否存在引用.
        /// </summary>
        public bool AnyRef => m_RefCount > 0;

        /// <summary>
        /// 资源实例id.
        /// </summary>
        public int AssetInstanceId { get; private set; }

        /// <summary>
        /// 资源类型。
        /// </summary>
        public Type AssetType => m_AssetType;

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
            if (m_RefCount > 0)
            {
                --m_RefCount;
            }
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
                task.TrySetException(new ErrorCodeException(FrameworkErrorCode.ResLoaderDisposed));
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
            Object asset = null;
            try
            {
                asset = await m_AssetLoadDelegate(m_AssetKey, m_AssetType, token);
            }
            catch (Exception e)
            {
                // 加载失败（含取消）：记录日志后以 null 结果通知等待者，调用方按 null 即失败处理。
                if (e is not OperationCanceledException)
                {
                    Log.Error($"加载资源 '{m_AssetKey}' 失败：{e.Message}\n{e.StackTrace}");
                }
            }

            if (token.IsCancellationRequested)
            {
                // 已取消：复位状态，等待者由 Dispose 路径排空。
                m_LoadState = 0;
                return;
            }

            if (asset == null)
            {
                // 加载失败：复位状态以允许后续重试，并以 null 通知所有等待者。
                m_LoadState = 0;

                while (m_TaskQueue.Count > 0)
                {
                    var task = m_TaskQueue.Dequeue();
                    task.TrySetResult(null);
                }

                return;
            }

            m_Asset = asset;
            m_LoadState = 2;
            AssetInstanceId = m_Asset.GetInstanceID();

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
