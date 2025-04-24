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
        private readonly ResourcePackage m_ResourcePackage;
        private readonly Action m_DisposeAction;

        private readonly Dictionary<string, AssetItemInfo> m_AssetItemDict = new();
        private readonly Dictionary<string, AssetHandle> m_AssetHandlerDict = new();
        private readonly Dictionary<string, SceneHandle> m_SceneHandlerDict = new();
        private CancellationTokenSource m_CancellationTokenSource;

        public YooAssetResLoader(ResourcePackage resourcePackage, Action disposeAction)
        {
            m_ResourcePackage = resourcePackage;
            m_DisposeAction = disposeAction;
            m_CancellationTokenSource = new CancellationTokenSource();
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

            m_DisposeAction?.Invoke();
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

        public async UniTask<Scene> LoadScene(string sceneName)
        {
            if (m_SceneHandlerDict.ContainsKey(sceneName))
            {
                throw new ErrorCodeException(ErrorCode.ResSceneAlreadyLoaded);
            }

            var operation = m_ResourcePackage.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            m_SceneHandlerDict[sceneName] = operation;

            await operation.ToUniTask();

            if (operation.Status != EOperationStatus.Succeed)
            {
                operation.Release();
                m_SceneHandlerDict.Remove(sceneName);
                throw new ErrorCodeException(ErrorCode.ResSceneLoadFailed);
            }

            return operation.SceneObject;
        }

        public async UniTask UnloadScene(string sceneName)
        {
            if (!m_SceneHandlerDict.TryGetValue(sceneName, out var operation))
            {
                throw new ErrorCodeException(ErrorCode.ResSceneNotLoad);
            }

            if (operation.Status != EOperationStatus.Succeed)
            {
                throw new ErrorCodeException(ErrorCode.ResSceneNotLoad);
            }
            
            m_SceneHandlerDict.Remove(sceneName);   

            var unloadOperation = operation.UnloadAsync();

            await unloadOperation;

            if (unloadOperation.Status != EOperationStatus.Succeed)
            {
                throw new ErrorCodeException(ErrorCode.ResSceneUnloadFailed);
            }

            operation.Release();
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