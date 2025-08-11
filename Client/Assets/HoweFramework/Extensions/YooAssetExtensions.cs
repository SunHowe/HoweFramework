using Cysharp.Threading.Tasks;
using YooAsset;

namespace HoweFramework
{
    /// <summary>
    /// YooAsset拓展方法.
    /// </summary>
    public static class YooAssetExtensions
    {
        private static YooAssetResLoader s_YooAssetResLoader;

        /// <summary>
        /// 使用YooAsset资源管线.
        /// </summary>
        public static ResModule UseYooAsset(this ResModule module)
        {
            s_YooAssetResLoader = new YooAssetResLoader();
            module.SetResCoreLoader(s_YooAssetResLoader);
            return module;
        }

        /// <summary>
        /// 使用编辑器模拟模式初始化YooAsset资源管线.
        /// </summary>
        public static UniTask InitYooAssetEditorSimulateMode(this ResModule module)
        {
            var buildResult = EditorSimulateModeHelper.SimulateBuild(YooAssetResLoader.DefaultPackageName);
            var packageRoot = buildResult.PackageRootDirectory;
            var editorFileSystemParams = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
            var initParameters = new EditorSimulateModeParameters();
            initParameters.EditorFileSystemParameters = editorFileSystemParams;

            return s_YooAssetResLoader.InitResourcePackageAsync(initParameters);
        }

        /// <summary>
        /// 使用联机更新模式初始化YooAsset资源管线.
        /// </summary>
        public static UniTask InitYooAssetHostPlayMode(this ResModule module, string hostServer, string fallbackHostServer)
        {
            IRemoteServices remoteServices = new YooAssetRemoteServices(hostServer, fallbackHostServer);
            var cacheFileSystemParams = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
            var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();

            var initParameters = new HostPlayModeParameters();
            initParameters.BuildinFileSystemParameters = buildinFileSystemParams;
            initParameters.CacheFileSystemParameters = cacheFileSystemParams;

            return s_YooAssetResLoader.InitResourcePackageAsync(initParameters);
        }

        /// <summary>
        /// 使用WebGL模式初始化YooAsset资源管线.
        /// </summary>
        public static UniTask InitYooAssetWebGLMode(this ResModule module, string hostServer, string fallbackHostServer)
        {
            var remoteServices = new YooAssetRemoteServices(hostServer, fallbackHostServer);
    
            var initParameters = new WebPlayModeParameters();
            initParameters.WebRemoteFileSystemParameters = FileSystemParameters.CreateDefaultWebRemoteFileSystemParameters(remoteServices);
            initParameters.WebServerFileSystemParameters = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();

            return s_YooAssetResLoader.InitResourcePackageAsync(initParameters);
        }

        /// <summary>
        /// 使用离线模式初始化YooAsset资源管线.
        /// </summary>
        public static UniTask InitYooAssetOfflineMode(this ResModule module)
        {
            var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
            var initParameters = new OfflinePlayModeParameters();
            initParameters.BuildinFileSystemParameters = buildinFileSystemParams;

            return s_YooAssetResLoader.InitResourcePackageAsync(initParameters);
        }

        /// <summary>
        /// 更新资源清单.
        /// </summary>
        /// <param name="module">资源模块.</param>
        /// <param name="packageName">资源包名.</param>
        /// <param name="packageVersion">资源包版本号,若为空则使用YooAssets默认的版本号请求功能.</param>
        /// <returns></returns>
        public static async UniTask<bool> RequestUpdatePackageManifest(this ResModule module, string packageVersion = null)
        {
            var package = YooAssets.GetPackage(YooAssetResLoader.DefaultPackageName);
            if (string.IsNullOrEmpty(packageVersion))
            {
                var requestVersionOperation = package.RequestPackageVersionAsync();

                await requestVersionOperation.ToUniTask();

                if (requestVersionOperation.Status != EOperationStatus.Succeed)
                {
                    return false;
                }

                packageVersion = requestVersionOperation.PackageVersion;
            }

            var updateOperation = package.UpdatePackageManifestAsync(packageVersion);

            await updateOperation.ToUniTask();

            return updateOperation.Status == EOperationStatus.Succeed;
        }
    }
}