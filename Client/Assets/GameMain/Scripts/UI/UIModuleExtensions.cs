using System.Threading;
using Cysharp.Threading.Tasks;
using HoweFramework;

namespace GameMain.UI
{
    /// <summary>
    /// UI模块扩展。
    /// </summary>
    public static class UIModuleExtensions
    {
        /// <summary>
        /// 打开界面。
        /// </summary>
        /// <param name="module">UI模块。</param>
        /// <param name="uiFormId">界面Id。</param>
        /// <param name="userData">业务透传数据。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>打开界面响应。</returns>
        public static UniTask<IResponse> OpenUIForm(this UIModule module, UIFormId uiFormId, object userData = null, CancellationToken token = default)
        {
            return module.OpenUIForm((int)uiFormId, userData, token);
        }

        /// <summary>
        /// 关闭界面。
        /// </summary>
        /// <param name="module">UI模块。</param>
        /// <param name="uiFormId">界面Id。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>关闭界面响应。</returns>
        public static UniTask<IResponse> CloseUIForm(this UIModule module, UIFormId uiFormId, CancellationToken token = default)
        {
            return module.CloseUIForm((int)uiFormId, token);
        }
    }
}
