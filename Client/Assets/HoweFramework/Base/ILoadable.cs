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
}