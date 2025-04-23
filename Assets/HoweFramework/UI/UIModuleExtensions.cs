using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
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
        /// <param name="token">取消令牌。</param>
        /// <returns>打开界面响应。</returns>
        public static UniTask<ResponseBase> OpenUIForm(this UIModule module, int uiFormId, CancellationToken token = default)
        {
            var request = ReferencePool.Acquire<OpenFormRequest>();
            request.FormId = uiFormId;
            return request.Execute(token);
        }

        /// <summary>
        /// 关闭界面。
        /// </summary>
        /// <param name="module">UI模块。</param>
        /// <param name="uiFormId">界面Id。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>关闭界面响应。</returns>
        public static UniTask<ResponseBase> CloseUIForm(this UIModule module, int uiFormId, CancellationToken token = default)
        {
            var request = ReferencePool.Acquire<CloseFormRequest>();
            request.FormId = uiFormId;
            return request.Execute(token);
        }
    }
}
