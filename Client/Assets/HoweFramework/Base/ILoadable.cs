using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 加载进度回调。
    /// </summary>
    /// <param name="progress">加载进度。0-1。</param>
    public delegate void LoadableProgress(float progress);

    /// <summary>
    /// 可加载接口。
    /// </summary>
    public interface ILoadable
    {
        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="onProgress">加载进度回调。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>异步加载任务。</returns>
        UniTask LoadAsync(LoadableProgress onProgress = null, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 加载组。用于将一系列加载任务组合在一起。
    /// </summary>
    public sealed class LoadableGroup : ILoadable, IDisposable, IReference
    {
        private readonly List<LoadableTask> m_LoadableTasks = new();
        private int m_TotalWeight = 0;

        /// <summary>
        /// 创建加载组。
        /// </summary>
        /// <returns>加载组。</returns>
        public static LoadableGroup Create()
        {
            return ReferencePool.Acquire<LoadableGroup>();
        }

        public void Clear()
        {
            m_LoadableTasks.DisposeItems();
            m_TotalWeight = 0;
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        /// <summary>
        /// 添加加载任务。
        /// </summary>
        /// <param name="loadable">加载对象。</param>
        /// <param name="weight">加载权重。</param>
        public LoadableGroup AddLoadable(ILoadable loadable, int weight = 1)
        {
            if (weight <= 0)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, nameof(weight));
            }

            if (loadable == null)
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, nameof(loadable));
            }

            m_LoadableTasks.Add(LoadableTask.Create(loadable, weight));
            m_TotalWeight += weight;
            return this;
        }

        /// <summary>
        /// 异步加载资源。  
        /// </summary>
        /// <param name="onProgress">加载进度回调。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>异步加载任务。</returns>
        public async UniTask LoadAsync(LoadableProgress onProgress = null, CancellationToken cancellationToken = default)
        {
            // 设置各任务占比。
            foreach (var loadableTask in m_LoadableTasks)
            {
                loadableTask.WeightRate = (float)loadableTask.Weight / m_TotalWeight;
            }

            // 执行加载任务。
            using var uniTask = ReusableList<UniTask>.Create();
            foreach (var loadableTask in m_LoadableTasks)
            {
                uniTask.Add(loadableTask.Loadable.LoadAsync(onProgress, cancellationToken));
            }

            await UniTask.WhenAll(uniTask);
        }

        /// <summary>
        /// 加载任务。
        /// </summary>
        private sealed class LoadableTask : IDisposable, IReference, ILoadable
        {
            public ILoadable Loadable { get; private set; }
            public int Weight { get; private set; }
            public float WeightRate { get; set; }

            private LoadableProgress m_OnProgress;

            public void Clear()
            {
                Loadable = null;
                Weight = 0;
                WeightRate = 0;
                m_OnProgress = null;
            }

            public void Dispose()
            {
                ReferencePool.Release(this);
            }

            public UniTask LoadAsync(LoadableProgress onProgress = null, CancellationToken cancellationToken = default)
            {
                m_OnProgress = onProgress;
                return Loadable.LoadAsync(OnProgress, cancellationToken);
            }

            private void OnProgress(float progress)
            {
                m_OnProgress?.Invoke(progress * WeightRate);
            }

            /// <summary>
            /// 创建加载任务。
            /// </summary>
            /// <param name="loadable">加载对象。</param>
            /// <param name="weight">加载权重。</param>
            /// <returns></returns>
            public static LoadableTask Create(ILoadable loadable, int weight)
            {
                var loadableTask = ReferencePool.Acquire<LoadableTask>();
                loadableTask.Loadable = loadable;
                loadableTask.Weight = weight;
                return loadableTask;
            }
        }
    }
}