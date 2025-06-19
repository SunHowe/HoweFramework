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
        public static UniTask<IResponse> OpenUIForm(this UIModule module, UIFormId uiFormId, object userData, CancellationToken token = default)
        {
            return module.OpenUIForm((int)uiFormId, userData, token);
        }
        
        /// <summary>
        /// 打开界面。
        /// </summary>
        /// <param name="module">UI模块。</param>
        /// <param name="uiFormId">界面Id。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>打开界面响应。</returns>
        public static UniTask<IResponse> OpenUIForm(this UIModule module, UIFormId uiFormId, CancellationToken token = default)
        {
            return module.OpenUIForm((int)uiFormId, token);
        }

        /// <summary>
        /// 关闭界面。
        /// </summary>
        /// <param name="module">UI模块。</param>
        /// <param name="uiFormId">界面Id。</param>
        /// <returns>关闭界面响应。</returns>
        public static UniTask<IResponse> CloseUIForm(this UIModule module, UIFormId uiFormId)
        {
            return module.CloseUIForm((int)uiFormId);
        }

        /// <summary>
        /// 打开错误提示。
        /// </summary>
        /// <param name="uniTask">异步任务。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>错误码。</returns>
        public static async UniTask<int> OpenErrorTips(this UniTask<int> uniTask, CancellationToken token = default)
        {
            var errorCode = await uniTask;
            if (errorCode == ErrorCode.Success || token.IsCancellationRequested)
            {
                return errorCode;
            }

            // TODO: 打开错误提示。
            Log.Error($"错误码：{errorCode}");
            return errorCode;
        }

        /// <summary>
        /// 打开错误提示。
        /// </summary>
        /// <param name="uniTask">异步任务。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>响应。</returns>
        public static async UniTask<IResponse> OpenErrorTips(this UniTask<IResponse> uniTask, CancellationToken token = default)
        {
            var response = await uniTask;
            if (response.ErrorCode == ErrorCode.Success || token.IsCancellationRequested)
            {
                return response;
            }

            // TODO: 打开错误提示。
            Log.Error($"错误码：{response.ErrorCode}");
            return response;
        }
    }
}
