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
        /// 关闭界面。不指定序列号时关闭该 FormId 最旧的一个实例。
        /// </summary>
        /// <param name="module">UI模块。</param>
        /// <param name="uiFormId">界面Id。</param>
        /// <returns>关闭界面响应。</returns>
        public static UniTask<IResponse> CloseUIForm(this UIModule module, int uiFormId)
        {
            return CloseUIForm(module, uiFormId, 0, false);
        }

        /// <summary>
        /// 关闭指定序列号的界面实例。
        /// </summary>
        /// <param name="module">UI模块。</param>
        /// <param name="uiFormId">界面Id。</param>
        /// <param name="formSerialId">界面序列编号。</param>
        /// <returns>关闭界面响应。</returns>
        public static UniTask<IResponse> CloseUIForm(this UIModule module, int uiFormId, int formSerialId)
        {
            return CloseUIForm(module, uiFormId, formSerialId, false);
        }

        /// <summary>
        /// 关闭界面。
        /// </summary>
        /// <param name="module">UI模块。</param>
        /// <param name="uiFormId">界面Id。</param>
        /// <param name="closeMultiple">为 true 时关闭该 FormId 的全部打开实例。</param>
        /// <returns>关闭界面响应。</returns>
        public static UniTask<IResponse> CloseUIForm(this UIModule module, int uiFormId, bool closeMultiple)
        {
            return CloseUIForm(module, uiFormId, 0, closeMultiple);
        }

        /// <summary>
        /// 关闭界面。
        /// </summary>
        /// <param name="module">UI模块。</param>
        /// <param name="uiFormId">界面Id。</param>
        /// <param name="formSerialId">界面序列编号。为 0 且 closeMultiple 为 false 时关闭最旧实例。</param>
        /// <param name="closeMultiple">为 true 时关闭该 FormId 的全部打开实例。</param>
        /// <returns>关闭界面响应。</returns>
        public static UniTask<IResponse> CloseUIForm(this UIModule module, int uiFormId, int formSerialId, bool closeMultiple)
        {
            var request = ReferencePool.Acquire<CloseFormRequest>();
            request.FormId = uiFormId;
            request.FormSerialId = formSerialId;
            request.CloseMutiple = closeMultiple;
            return request.Execute();
        }
    }
}
