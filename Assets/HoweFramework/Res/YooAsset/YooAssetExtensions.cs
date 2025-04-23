using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using YooAsset;

namespace HoweFramework
{
    /// <summary>
    /// YooAsset拓展方法.
    /// </summary>
    public static class YooAssetExtensions
    {
        private const string DefaultPackageName = "DefaultPackage";

        /// <summary>
        /// 使用YooAsset资源管线.
        /// </summary>
        public static async UniTask UseYooAsset(this ResModule module, string packageName, InitializeParameters parameters)
        {
            YooAssets.Initialize();

            var resourcePackage = YooAssets.CreatePackage(packageName);
            YooAssets.SetDefaultPackage(resourcePackage);

            await resourcePackage.InitializeAsync(parameters).ToUniTask();

            module.SetResCoreLoader(new YooAssetResLoader(resourcePackage, Dispose));

            void Dispose()
            {
                resourcePackage.DestroyAsync().Completed += (op) =>
                {
                    YooAssets.RemovePackage(packageName);
                };
            }
        }

        /// <summary>
        /// 使用编辑器模拟模式的YooAsset资源管线.
        /// </summary>
        public static UniTask UseYooAssetEditorSimulateMode(this ResModule module, string packageName = DefaultPackageName)
        {
            var buildResult = EditorSimulateModeHelper.SimulateBuild(packageName);
            var packageRoot = buildResult.PackageRootDirectory;
            var editorFileSystemParams = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
            var initParameters = new EditorSimulateModeParameters();
            initParameters.EditorFileSystemParameters = editorFileSystemParams;

            return module.UseYooAsset(packageName, initParameters);
        }

        /// <summary>
        /// 使用联机更新模式的YooAsset资源管线.
        /// </summary>
        public static UniTask UseYooAssetHostPlayMode(this ResModule module, string hostServer, string fallbackHostServer, string packageName = DefaultPackageName)
        {
            IRemoteServices remoteServices = new YooAssetRemoteServices(hostServer, fallbackHostServer);
            var cacheFileSystemParams = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
            var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();

            var initParameters = new HostPlayModeParameters();
            initParameters.BuildinFileSystemParameters = buildinFileSystemParams;
            initParameters.CacheFileSystemParameters = cacheFileSystemParams;

            return module.UseYooAsset(packageName, initParameters);
        }

        /// <summary>
        /// 更新资源清单.
        /// </summary>
        /// <param name="module">资源模块.</param>
        /// <param name="packageName">资源包名.</param>
        /// <param name="packageVersion">资源包版本号,若为空则使用YooAssets默认的版本号请求功能.</param>
        /// <returns></returns>
        public static async UniTask<bool> RequestUpdatePackageManifest(this ResModule module, string packageName = DefaultPackageName, string packageVersion = null)
        {
            var package = YooAssets.GetPackage(packageName);
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