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
        /// <param name="userData">业务透传数据。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>打开界面响应。</returns>
        public static UniTask<IResponse> OpenUIForm(this UIModule module, int uiFormId, object userData, CancellationToken token = default)
        {
            return ReferencePool.Acquire<OpenFormRequest>()
                .SetFormId(uiFormId)
                .SetUserData(userData)
                .Execute(token);
        }
        
        /// <summary>
        /// 打开界面。
        /// </summary>
        /// <param name="module">UI模块。</param>
        /// <param name="uiFormId">界面Id。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>打开界面响应。</returns>
        public static UniTask<IResponse> OpenUIForm(this UIModule module, int uiFormId, CancellationToken token = default)
        {
            return ReferencePool.Acquire<OpenFormRequest>()
                .SetFormId(uiFormId)
                .Execute(token);
        }
        
        /// <summary>
        /// 打开界面(这界面打开完成时立即返回）。
        /// </summary>
        /// <param name="module">UI模块。</param>
        /// <param name="uiFormId">界面Id。</param>
        /// <param name="userData">业务透传数据。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>打开界面错误码。</returns>
        public static UniTask<int> OpenUIFormOnlyCareAboutFormOpen(this UIModule module, int uiFormId, object userData, CancellationToken token = default)
        {
            var tcs = AutoResetUniTaskCompletionSource<int>.Create();
            var task = tcs.Task;

            ReferencePool.Acquire<OpenFormRequest>()
                .SetFormId(uiFormId)
                .SetUserData(userData)
                .SetFormOpenTcs(tcs)
                .Execute(token)
                .Forget();

            return task;
        }
        
        /// <summary>
        /// 打开界面(这界面打开完成时立即返回）。
        /// </summary>
        /// <param name="module">UI模块。</param>
        /// <param name="uiFormId">界面Id。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>打开界面错误码。</returns>
        public static UniTask<int> OpenUIFormOnlyCareAboutFormOpen(this UIModule module, int uiFormId, CancellationToken token = default)
        {
            var tcs = AutoResetUniTaskCompletionSource<int>.Create();
            var task = tcs.Task;

            ReferencePool.Acquire<OpenFormRequest>()
                .SetFormId(uiFormId)
                .SetFormOpenTcs(tcs)
                .Execute(token)
                .Forget();

            return task;
        }

        /// <summary>
        /// 关闭界面。
        /// </summary>
        /// <param name="module">UI模块。</param>
        /// <param name="uiFormId">界面Id。</param>
        /// <returns>关闭界面响应。</returns>
        public static UniTask<IResponse> CloseUIForm(this UIModule module, int uiFormId)
        {
            return CloseUIForm(module, uiFormId, 0);
        }

        /// <summary>
        /// 关闭指定序列号的界面。序列号为 0 时关闭该 Id 最先打开的实例。
        /// </summary>
        /// <param name="module">UI模块。</param>
        /// <param name="uiFormId">界面Id。</param>
        /// <param name="formSerialId">界面序列编号。</param>
        /// <returns>关闭界面响应。</returns>
        public static UniTask<IResponse> CloseUIForm(this UIModule module, int uiFormId, int formSerialId)
        {
            var request = ReferencePool.Acquire<CloseFormRequest>();
            request.FormId = uiFormId;
            request.FormSerialId = formSerialId;
            return request.Execute();
        }

        /// <summary>
        /// 关闭指定 Id 的全部界面实例。
        /// </summary>
        /// <param name="module">UI模块。</param>
        /// <param name="uiFormId">界面Id。</param>
        /// <returns>关闭界面响应。</returns>
        public static UniTask<IResponse> CloseAllUIForm(this UIModule module, int uiFormId)
        {
            var request = ReferencePool.Acquire<CloseFormRequest>();
            request.FormId = uiFormId;
            request.CloseMutiple = true;
            return request.Execute();
        }
    }
}
