using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 加载场景任务。
    /// </summary>
    internal sealed class LoadSceneTask : ILoadable, IDisposable, IReference
    {
        private string m_SceneAssetName;

        public void Clear()
        {
            m_SceneAssetName = null;
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public async UniTask LoadAsync(LoadableProgress onProgress = null, CancellationToken cancellationToken = default)
        {
            await SceneModule.Instance.LoadSceneAsync(m_SceneAssetName);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            onProgress?.Invoke(1f);
        }

        public static LoadSceneTask Create(string sceneAssetName)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                throw new ErrorCodeException(ErrorCode.InvalidParam, nameof(sceneAssetName));
            }

            var loadSceneTask = ReferencePool.Acquire<LoadSceneTask>();
            loadSceneTask.m_SceneAssetName = sceneAssetName;
            return loadSceneTask;
        }
    }

    /// <summary>
    /// 加载场景任务扩展。
    /// </summary>
    public static class LoadSceneTaskExtension
    {
        /// <summary>
        /// 创建加载场景任务。
        /// </summary>
        /// <param name="sceneModule">场景模块。</param>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <returns>加载场景任务。</returns>
        public static ILoadable CreateLoadSceneTask(this SceneModule sceneModule, string sceneAssetName)
        {
            return LoadSceneTask.Create(sceneAssetName);
        }
    }
}