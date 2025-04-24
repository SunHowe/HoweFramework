using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using FairyGUI;
using FairyGUI.Dynamic;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// FairyGUI界面辅助器。
    /// </summary>
    public sealed class FairyGUIFormHelper : IUIFormHelper, IUIAssetLoader, IUIPackageHelper, IUIAssetManagerConfiguration
    {
        /// <summary>
        /// 加载中的取消令牌字典。
        /// </summary>
        private readonly Dictionary<int, CancellationTokenSource> m_LoadCancellationTokenDict = new();

        /// <summary>
        /// FairyGUI id到包名的映射字典。
        /// </summary>
        private readonly Dictionary<string, string> m_PackageNameDict = new();

        /// <summary>
        /// FairyGUI 包名到包二进制数据的映射字典。
        /// </summary>
        private readonly Dictionary<string, byte[]> m_PackageDict = new();

        /// <summary>
        /// 已加载的纹理资源路径字典。
        /// </summary>
        private readonly Dictionary<int, string> m_TextureAssetKeyDict = new();

        /// <summary>
        /// 已加载的音频资源路径字典。
        /// </summary>
        private readonly Dictionary<int, string> m_AudioClipAssetKeyDict = new();

        /// <summary>
        /// 界面绑定信息字典。
        /// </summary>
        private readonly Dictionary<int, FairyGUIFormBinding> m_FormBindingDict = new();

        /// <summary>
        /// 加载id。
        /// </summary>
        private int m_LoadId;

        /// <summary>
        /// 是否开启预加载包模式。
        /// </summary>
        private bool m_PreloadPackageMode;

        private readonly UIAssetManager m_UIAssetManager;
        private readonly IResLoader m_ResLoader;
        private readonly FairyGUISettings m_Settings;

        #region [IUIAssetManagerConfiguration]

        public IUIPackageHelper PackageHelper => this;

        public IUIAssetLoader AssetLoader => this;

        public bool UnloadUnusedUIPackageImmediately => m_Settings.UnloadUnusedUIPackageImmediately;

        #endregion

        public FairyGUIFormHelper(FairyGUISettings settings)
        {
            m_Settings = settings;

            var scaleFactor = settings.ContentScaleFactor;
            GRoot.inst.SetContentScaleFactor(scaleFactor.DesignResolutionX, scaleFactor.DesignResolutionY, scaleFactor.ScreenMatchMode);

            m_UIAssetManager = new UIAssetManager();
            m_UIAssetManager.Initialize(this);

            m_ResLoader = ResModule.Instance.CreateResLoader();

            GTextField.GetTemplateText = GetTemplateText;
        }

        public void Dispose()
        {
            m_UIAssetManager.Dispose();
            m_ResLoader.Dispose();

            m_PackageNameDict.Clear();
            m_PackageDict.Clear();
            m_TextureAssetKeyDict.Clear();
            m_AudioClipAssetKeyDict.Clear();

            UIObjectFactory.Clear();

            GTextField.GetTemplateText = null;
        }

        /// <summary>
        /// 获取模板文本。
        /// </summary>
        /// <param name="template">模板。</param>
        /// <param name="dictionary">字典。</param>
        /// <returns>模板文本。</returns>
        private string GetTemplateText(string template, Dictionary<string, string> dictionary)
        {
            return TextUtility.ParseTemplate(template, dictionary);
        }

        /// <summary>
        /// 设置是否开启预加载包模式。
        /// </summary>
        /// <param name="preloadPackageMode">是否开启预加载包模式。</param>
        public void SetPreloadPackageMode(bool preloadPackageMode)
        {
            m_PreloadPackageMode = preloadPackageMode;
        }

        /// <summary>
        /// 加载UI包列表。
        /// </summary>
        /// <param name="assetKey">PackageMapping映射文件加载路径。</param>
        public async UniTask LoadUIPackagesAsync(string assetKey)
        {
            var packageMapping = await m_ResLoader.LoadAssetAsync<UIPackageMapping>(assetKey);
            if (packageMapping == null)
            {
                throw new ErrorCodeException(ErrorCode.UIPackageMappingNotFound);
            }

            using var packageNameList = ReusableList<string>.Create();
            var count = Math.Min(packageMapping.PackageIds.Length, packageMapping.PackageNames.Length);
            for (int i = 0; i < count; i++)
            {
                var packageId = packageMapping.PackageIds[i];
                var packageName = packageMapping.PackageNames[i];
                m_PackageNameDict[packageId] = packageName;

                packageNameList.Add(packageName);
            }

            // 卸载PackageMapping映射文件。
            m_ResLoader.UnloadAsset(assetKey);

            if (!m_PreloadPackageMode)
            {
                return;
            }

            if (count == 0)
            {
                return;
            }

            // 加载所有包文件。
            using var uniTaskList = ReusableList<UniTask>.Create();
            for (int i = 0; i < packageNameList.Count; i++)
            {
                uniTaskList.Add(LoadUIPackageAsync(packageNameList[i]));
            }

            await UniTask.WhenAll(uniTaskList);

            async UniTask LoadUIPackageAsync(string packageName)
            {
                m_PackageDict[packageName] = await LoadUIPackageBytesAsync(packageName);
            }
        }

        /// <summary>
        /// 添加界面绑定。
        /// </summary>
        /// <param name="bindings">界面绑定。</param>
        public void AddUIFormBindings(FairyGUIFormBinding[] bindings)
        {
            foreach (var binding in bindings)
            {
                m_FormBindingDict[binding.FormId] = binding;
            }
        }

        /// <summary>
        /// 创建界面逻辑。
        /// </summary>
        /// <param name="uiFormId">界面编号。</param>
        /// <returns>界面逻辑。</returns>
        public IUIFormLogic CreateUIFormLogic(int uiFormId)
        {
            if (m_FormBindingDict.TryGetValue(uiFormId, out var formBinding))
            {
                return formBinding.Creator();
            }

            throw new ErrorCodeException(ErrorCode.UIFormBindingNotFound, $"界面绑定信息[{uiFormId}]未找到。");
        }

        /// <summary>
        /// 取消加载界面实例。
        /// </summary>
        /// <param name="loadId">加载任务id。</param>
        public void CancelLoadUIFormInstance(int loadId)
        {
            if (m_LoadCancellationTokenDict.Remove(loadId, out var cancellationTokenSource))
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }
        }

        /// <summary>
        /// 加载界面实例。
        /// </summary>
        /// <param name="uiFormId">界面编号。</param>
        /// <param name="onLoadSuccess">加载成功回调。</param>
        /// <returns>加载任务id。</returns>
        public int LoadUIFormInstance(int uiFormId, Action<object> onLoadSuccess)
        {
            if (!m_FormBindingDict.TryGetValue(uiFormId, out var formBinding))
            {
                throw new ErrorCodeException(ErrorCode.UIFormBindingNotFound, $"界面绑定信息[{uiFormId}]未找到。");
            }

            var loadId = ++m_LoadId;

            var cancellationTokenSource = new CancellationTokenSource();
            m_LoadCancellationTokenDict[loadId] = cancellationTokenSource;
            var token = cancellationTokenSource.Token;

            UIPackage.CreateObjectFromURLAsync(formBinding.FormURL, (obj) =>
            {
                if (obj == null)
                {
                    throw new ErrorCodeException(ErrorCode.UIFormInstantiateFailed, $"界面[{uiFormId}]实例化失败。");
                }

                if (token.IsCancellationRequested)
                {
                    // 取消加载，销毁界面实例。
                    obj.Dispose();
                    return;
                }

                onLoadSuccess(obj);
            });

            return loadId;
        }

        /// <summary>
        /// 设置界面实例是否打开。
        /// </summary>
        /// <param name="uiFormInstance">界面实例。</param>
        /// <param name="uiGroupInstance">界面组实例。</param>
        /// <param name="isOpen">是否打开。</param>
        public void SetUIFormInstanceIsOpen(object uiFormInstance, object uiGroupInstance, bool isOpen)
        {
            var gFormComponent = (GComponent)uiFormInstance;
            var gGroupComponent = (GComponent)uiGroupInstance;

            if (isOpen)
            {
                gGroupComponent.AddChild(gFormComponent);
            }
            else
            {
                gGroupComponent.RemoveChild(gFormComponent);
            }
        }

        /// <summary>
        /// 设置界面实例是否可见。
        /// </summary>
        /// <param name="uiFormInstance">界面实例。</param>
        /// <param name="visible">是否可见。</param>
        public void SetUIFormInstanceIsVisible(object uiFormInstance, bool visible)
        {
            var gFormComponent = (GComponent)uiFormInstance;
            gFormComponent.visible = visible;
        }

        /// <summary>
        /// 卸载界面实例。
        /// </summary>
        /// <param name="uiFormInstance">界面实例。</param>
        public void UnloadUIFormInstance(object uiFormInstance)
        {
            var gFormComponent = (GComponent)uiFormInstance;
            gFormComponent.Dispose();
        }

        #region [IUIAssetLoader]

        /// <summary>
        /// 获取UI包路径。
        /// </summary>
        /// <param name="packageName">包名。</param>
        /// <returns>UI包路径。</returns>
        private string GetUIPackagePath(string packageName)
        {
            return string.Format(m_Settings.UIPackagePathFormat, packageName);
        }

        /// <summary>
        /// 异步加载UI包二进制数据。
        /// </summary>
        /// <param name="packageName">包名。</param>
        /// <returns>UI包二进制数据。</returns>
        /// <exception cref="ErrorCodeException">UI包未找到。</exception>
        private async UniTask<byte[]> LoadUIPackageBytesAsync(string packageName)
        {
            var packagePath = GetUIPackagePath(packageName);
            var textAsset = await m_ResLoader.LoadAssetAsync<TextAsset>(packagePath);
            if (textAsset == null)
            {
                throw new ErrorCodeException(ErrorCode.UIPackageNotFound, $"UI包[{packageName}]未找到。");
            }

            var bytes = textAsset.bytes;
            m_ResLoader.UnloadAsset(packagePath);
            return bytes;
        }

        public void LoadUIPackageBytesAsync(string packageName, LoadUIPackageBytesCallback callback)
        {
            if (m_PreloadPackageMode && m_PackageDict.TryGetValue(packageName, out var bytes))
            {
                callback(bytes, string.Empty);
                return;
            }

            LoadUIPackageBytesAsync(packageName).ContinueWith(bytes => callback(bytes, string.Empty));
        }

        public void LoadUIPackageBytes(string packageName, out byte[] bytes, out string assetNamePrefix)
        {
            assetNamePrefix = string.Empty;
            
            if (m_PreloadPackageMode)
            {
                m_PackageDict.TryGetValue(packageName, out bytes);
                return;
            }

            bytes = m_ResLoader.LoadBinary(GetUIPackagePath(packageName));
        }

        public void LoadTextureAsync(string packageName, string assetName, string extension, LoadTextureCallback callback)
        {
            LoadMethod().Forget();

            async UniTask LoadMethod()
            {
                string assetPath = string.Format(m_Settings.UIAssetPathFormat, packageName, assetName, extension);
                var texture = await m_ResLoader.LoadAssetAsync<Texture>(assetPath);

                if (texture != null)
                {
                    m_TextureAssetKeyDict[texture.GetInstanceID()] = assetPath;
                }

                callback(texture);
            }
        }

        public void UnloadTexture(Texture texture)
        {
            if (texture == null)
            {
                return;
            }

            if (m_TextureAssetKeyDict.TryGetValue(texture.GetInstanceID(), out var assetPath))
            {
                m_ResLoader.UnloadAsset(assetPath);
                m_TextureAssetKeyDict.Remove(texture.GetInstanceID());
            }
        }

        public void LoadAudioClipAsync(string packageName, string assetName, string extension, LoadAudioClipCallback callback)
        {
            LoadMethod().Forget();

            async UniTask LoadMethod()
            {
                string assetPath = string.Format(m_Settings.UIAssetPathFormat, packageName, assetName, extension);
                var audioClip = await m_ResLoader.LoadAssetAsync<AudioClip>(assetPath);

                if (audioClip != null)
                {
                    m_AudioClipAssetKeyDict[audioClip.GetInstanceID()] = assetPath;
                }

                callback(audioClip);
            }
        }

        public void UnloadAudioClip(AudioClip audioClip)
        {
            if (audioClip == null)
            {
                return;
            }

            if (m_AudioClipAssetKeyDict.TryGetValue(audioClip.GetInstanceID(), out var assetPath))
            {
                m_ResLoader.UnloadAsset(assetPath);
                m_AudioClipAssetKeyDict.Remove(audioClip.GetInstanceID());
            }
        }

        #endregion

        #region [IUIPackageHelper]

        /// <summary>
        /// 获取包名。
        /// </summary>
        /// <param name="id">FairyGUI 包id。</param>
        /// <returns>包名。</returns>
        public string GetPackageNameById(string id)
        {
            return m_PackageNameDict.TryGetValue(id, out var packageName) ? packageName : null;
        }

        #endregion
    }
}
