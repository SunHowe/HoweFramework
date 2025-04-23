using System;
using System.Threading;
using Cysharp.Threading.Tasks;

using Object = UnityEngine.Object;

namespace HoweFramework
{
    /// <summary>
    /// 加载资源委托。
    /// </summary>
    /// <param name="assetKey">资源路径。</param>
    /// <param name="assetType">资源类型。</param>
    /// <param name="token">取消令牌。</param>
    /// <returns>资源。</returns>
    public delegate UniTask<Object> LoadAssetAsyncDelegate(string assetKey, Type assetType, CancellationToken token);

    /// <summary>
    /// 卸载资源委托。
    /// </summary>
    /// <param name="assetKey">资源路径。</param>
    public delegate void UnloadAssetDelegate(string assetKey);

    /// <summary>
    /// 资源加载器接口。在不需要使用时，需要调用Dispose方法释放资源。
    /// </summary>
    public interface IResLoader : IDisposable
    {
        /// <summary>
        /// 加载资源。
        /// </summary>
        /// <param name="assetKey">资源路径。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>资源。</returns>
        UniTask<Object> LoadAssetAsync(string assetKey, Type assetType, CancellationToken token = default);

        /// <summary>
        /// 加载二进制数据。
        /// </summary>
        /// <param name="assetKey">资源路径。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>二进制数据。</returns>
        UniTask<byte[]> LoadBinaryAsync(string assetKey, CancellationToken token = default);

        /// <summary>
        /// 加载二进制数据。
        /// </summary>
        /// <param name="assetKey">资源路径。</param>
        /// <returns>二进制数据。</returns>
        byte[] LoadBinary(string assetKey);

        /// <summary>
        /// 卸载资源。
        /// </summary>
        /// <param name="assetKey">资源路径。</param>
        void UnloadAsset(string assetKey);

        /// <summary>
        /// 卸载未使用的资源。
        /// </summary>
        void UnloadUnusedAsset();
    }
}
