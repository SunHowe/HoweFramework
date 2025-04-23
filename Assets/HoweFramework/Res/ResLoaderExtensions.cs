using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 资源加载器扩展方法。
    /// </summary>
    public static class ResLoaderExtensions
    {
        /// <summary>
        /// 加载资源。
        /// </summary>
        /// <param name="resLoader">资源加载器。</param>
        /// <param name="assetKey">资源路径。</param>
        /// <returns>资源。</returns>
        public static UniTask<T> LoadAssetAsync<T>(this IResLoader resLoader, string assetKey, CancellationToken token = default) where T : Object
        {
            return resLoader.LoadAssetAsync(assetKey, typeof(T), token).ContinueWith(t => t as T);
        }
    }
}