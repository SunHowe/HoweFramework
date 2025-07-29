using Cysharp.Threading.Tasks;
using FairyGUI;

namespace HoweFramework
{
    /// <summary>
    /// FairyGUI扩展方法。
    /// </summary>
    public static class FairyGUIExtensions
    {
        private static FairyGUIFormHelper s_FairyGUIFormHelper;

        /// <summary>
        /// 设置UI模块使用FairyGUI。
        /// </summary>
        /// <param name="module">UI模块。</param>
        /// <param name="settings">FairyGUI设置。</param>
        public static UniTask UseFairyGUI(this UIModule module, FairyGUISettings settings)
        {
            s_FairyGUIFormHelper = new FairyGUIFormHelper(settings);

            module.SetUIFormHelper(s_FairyGUIFormHelper);
            module.SetUIFormGroupHelper(new FairyGUIFormGroupHelper());

            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 设置是否开启预加载包模式。
        /// </summary>
        /// <param name="module">UI模块。</param>
        /// <param name="preloadPackageMode">是否开启预加载包模式。</param>
        public static void SetPreloadPackageMode(this UIModule module, bool preloadPackageMode)
        {
            s_FairyGUIFormHelper.SetPreloadPackageMode(preloadPackageMode);
        }

        /// <summary>
        /// 加载FairyGUI包列表。
        /// </summary>
        /// <param name="module">UI模块。</param>
        /// <param name="assetKey">PackageMapping映射文件加载路径。</param>
        /// <returns>加载任务。</returns>
        public static UniTask LoadFairyGUIPackagesAsync(this UIModule module, string assetKey)
        {
            return s_FairyGUIFormHelper.LoadUIPackagesAsync(assetKey);
        }

        /// <summary>
        /// 添加FairyGUI界面绑定。
        /// </summary>
        /// <param name="module">UI模块。</param>
        /// <param name="bindings">界面绑定。</param>
        public static void AddFairyGUIFormBindings(this UIModule module, FairyGUIFormBinding[] bindings)
        {
            s_FairyGUIFormHelper.AddUIFormBindings(bindings);
        }

        /// <summary>
        /// 添加FairyGUI组件绑定。
        /// </summary>
        /// <param name="module">UI模块。</param>
        /// <param name="bindings">组件绑定。</param>
        public static void AddFairyGUIComponentBindings(this UIModule module, FairyGUIComponentBinding[] bindings)
        {
            foreach (var binding in bindings)
            {
                UIObjectFactory.SetPackageItemExtension(binding.ComponentURL, binding.Creator);
            }
        }

        /// <summary>
        /// 获取列表项索引。
        /// </summary>
        /// <param name="list">列表。</param>
        /// <param name="item">列表项。</param>
        /// <returns>列表项索引。</returns>
        public static int GetItemIndex(this GList list, GObject item)
        {
            return list.ChildIndexToItemIndex(list.GetChildIndex(item));
        }
    }
}
