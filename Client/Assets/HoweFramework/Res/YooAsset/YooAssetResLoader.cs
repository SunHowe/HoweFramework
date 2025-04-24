using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace HoweFramework
{
    /// <summary>
    /// YooAsset资源加载器.
    /// </summary>
    public sealed class YooAssetResLoader : IResLoader
    {
        public const string DefaultPackageName = "DefaultPackage";

        private static AutoResetUniTaskCompletionSource s_DestroyTcs;

        private ResourcePackage m_ResourcePackage;

        private readonly Dictionary<string, AssetItemInfo> m_AssetItemDict = new();
        private readonly Dictionary<string, AssetHandle> m_AssetHandlerDict = new();
        private readonly Dictionary<string, SceneHandle> m_SceneHandlerDict = new();
        private readonly Dictionary<string, UnloadSceneOperation> m_UnloadSceneOperationDict = new();
        private CancellationTokenSource m_CancellationTokenSource;

        public YooAssetResLoader()
        {
            m_CancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// 初始化资源包。
        /// </summary>
        public async UniTask InitResourcePackageAsync(InitializeParameters parameters)
        {
            if (s_DestroyTcs != null)
            {
                await s_DestroyTcs.Task;
            }

            YooAssets.Initialize();

            var resourcePackage = YooAssets.CreatePackage(DefaultPackageName);
            YooAssets.SetDefaultPackage(resourcePackage);

            await resourcePackage.InitializeAsync(parameters).ToUniTask();

            m_ResourcePackage = resourcePackage;
        }

        /// <summary>
        /// 释放资源包。
        /// </summary>
        private async UniTask DisposeResourcePackageAsync()
        {
            var tcs = AutoResetUniTaskCompletionSource.Create();
            s_DestroyTcs = tcs;

            await m_ResourcePackage.DestroyAsync().ToUniTask();

            YooAssets.RemovePackage(DefaultPackageName);

            s_DestroyTcs = null;
            tcs.TrySetResult();
        }

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

            foreach (var handle in m_AssetHandlerDict.Values)
            {
                handle.Release();
            }

            m_AssetHandlerDict.Clear();

            foreach (var handle in m_SceneHandlerDict.Values)
            {
                handle.Release();
            }

            m_SceneHandlerDict.Clear();
            m_UnloadSceneOperationDict.Clear();

            DisposeResourcePackageAsync().Forget();
        }

        public async UniTask<UnityEngine.Object> LoadAssetAsync(string assetKey, Type assetType, CancellationToken token = default)
        {
            var resLoaderToken = m_CancellationTokenSource.Token;

            if (!m_AssetItemDict.TryGetValue(assetKey, out var assetItemInfo))
            {
                assetItemInfo = AssetItemInfo.Create(assetKey, assetType, LoadAssetWithYooAssets, UnloadAssetWithYooAssets);
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

        public async UniTask<byte[]> LoadBinaryAsync(string assetKey, CancellationToken token = default)
        {
            // 目前使用TextAsset来加载二进制数据。
            var asset = await LoadAssetAsync(assetKey, typeof(TextAsset), token);
            if (asset == null)
            {
                return null;
            }

            var bytes = ((TextAsset)asset).bytes;
            UnloadAsset(assetKey);
            return bytes;
        }

        public byte[] LoadBinary(string assetKey)
        {
            // 目前使用TextAsset来加载二进制数据。
            var operation = m_ResourcePackage.LoadAssetSync(assetKey, typeof(TextAsset));
            var asset = operation.AssetObject;
            if (asset == null)
            {
                return null;
            }

            byte[] bytes = null;
            var textAsset = asset as TextAsset;
            if (textAsset != null)
            {
                bytes = textAsset.bytes;
            }
            
            operation.Release();
            return bytes;
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

            m_ResourcePackage.UnloadUnusedAssetsAsync();
        }

        /// <summary>
        /// 加载场景。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <returns>场景。</returns>
        /// <exception cref="ErrorCodeException"></exception>
        public async UniTask<Scene> LoadScene(string sceneAssetName)
        {
            if (m_SceneHandlerDict.TryGetValue(sceneAssetName, out var operation))
            {
                if (operation.Status == EOperationStatus.Succeed)
                {
                throw new ErrorCodeException(ErrorCode.ResSceneAlreadyLoaded);
                }

                throw new ErrorCodeException(ErrorCode.ResSceneLoading);
            }

            operation = m_ResourcePackage.LoadSceneAsync(sceneAssetName, LoadSceneMode.Additive);
            m_SceneHandlerDict[sceneAssetName] = operation;

            await operation.ToUniTask();

            if (operation.Status != EOperationStatus.Succeed)
            {
                operation.Release();
                m_SceneHandlerDict.Remove(sceneAssetName);
                throw new ErrorCodeException(ErrorCode.ResSceneLoadFailed);
            }

            return operation.SceneObject;
        }

        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <exception cref="ErrorCodeException"></exception>
        public async UniTask UnloadScene(string sceneAssetName)
        {
            if (m_UnloadSceneOperationDict.ContainsKey(sceneAssetName))
            {
                throw new ErrorCodeException(ErrorCode.ResSceneUnloading);
            }

            if (!m_SceneHandlerDict.TryGetValue(sceneAssetName, out var operation))
            {
                throw new ErrorCodeException(ErrorCode.ResSceneNotLoad);
            }

            if (operation.Status != EOperationStatus.Succeed)
            {
                throw new ErrorCodeException(ErrorCode.ResSceneLoading);
            }
            
            m_SceneHandlerDict.Remove(sceneAssetName);   

            var unloadOperation = operation.UnloadAsync();
            m_UnloadSceneOperationDict[sceneAssetName] = unloadOperation;

            await unloadOperation;

            if (unloadOperation.Status != EOperationStatus.Succeed)
            {
                throw new ErrorCodeException(ErrorCode.ResSceneUnloadFailed);
            }

            operation.Release();
        }

        /// <summary>
        /// 获取场景是否已加载。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <returns>场景是否已加载。</returns>
        public bool SceneIsLoaded(string sceneAssetName)
        {
            if (m_SceneHandlerDict.TryGetValue(sceneAssetName, out var operation))
            {
                return operation.Status == EOperationStatus.Succeed;
            }

            return false;
        }

        /// <summary>
        /// 获取场景是否正在加载。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <returns>场景是否正在加载。</returns>
        public bool SceneIsLoading(string sceneAssetName)
        {
            if (m_SceneHandlerDict.TryGetValue(sceneAssetName, out var operation))
            {
                return operation.Status == EOperationStatus.Processing;
            }

            return false;
        }

        /// <summary>
        /// 获取场景是否正在卸载。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <returns>场景是否正在卸载。</returns>
        public bool SceneIsUnloading(string sceneAssetName)
        {
            if (m_UnloadSceneOperationDict.TryGetValue(sceneAssetName, out var operation))
            {
                return operation.Status == EOperationStatus.Processing;
            }

            return false;
        }

        private async UniTask<UnityEngine.Object> LoadAssetWithYooAssets(string assetKey, Type assetType, CancellationToken token)
        {
            var operation = m_ResourcePackage.LoadAssetAsync(assetKey, assetType);
            m_AssetHandlerDict[assetKey] = operation;

            await operation.ToUniTask();

            return operation.AssetObject;
        }

        private void UnloadAssetWithYooAssets(string assetKey)
        {
            if (!m_AssetHandlerDict.Remove(assetKey, out var assetHandle))
            {
                return;
            }

            assetHandle.Release();
        }
    }
}