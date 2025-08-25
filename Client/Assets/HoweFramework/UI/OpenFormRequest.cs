using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 打开UI请求。
    /// </summary>
    public sealed class OpenFormRequest : RequestBase
    {
        /// <summary>
        /// 设置响应事件。
        /// </summary>
        public event Action<OpenFormRequest> OnSetResponse;

        /// <summary>
        /// UI界面编号。
        /// </summary>
        public int FormId { get; set; }

        /// <summary>
        /// 业务透传数据。
        /// </summary>
        public object UserData { get; set; }

        /// <summary>
        /// 取消令牌。
        /// </summary>
        public CancellationToken CancellationToken { get; private set; }

        private AutoResetUniTaskCompletionSource<IResponse> m_Tcs;
        private AutoResetUniTaskCompletionSource<int> m_TcsFormOpen;

        protected override UniTask<IResponse> OnExecute(CancellationToken token)
        {
            CancellationToken = token;
            m_Tcs = AutoResetUniTaskCompletionSource<IResponse>.Create();
            var task = m_Tcs.Task;
            UIModule.Instance.HandleOpenFormRequest(this);
            return task;
        }

        /// <summary>
        /// 设置响应。
        /// </summary>
        /// <param name="response">响应。</param>
        public void SetResponse(IResponse response)
        {
            if (m_Tcs == null)
            {
                response.Dispose();
                return;
            }

            var errorCode = response.ErrorCode;

            OnSetResponse?.Invoke(this);
            m_Tcs.TrySetResult(response);
            m_TcsFormOpen?.TrySetResult(errorCode);
        }

        /// <summary>
        /// 设置响应错误码。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        public void SetResponse(int errorCode)
        {
            SetResponse(CommonResponse.Create(errorCode));
        }

        /// <summary>
        /// 界面打开成功回调。
        /// </summary>
        public void OnFormOpenSuccess()
        {
            if (m_TcsFormOpen == null)
            {
                return;
            }

            var tcs = m_TcsFormOpen;
            m_TcsFormOpen = null;
            tcs.TrySetResult(0);
        }

        /// <summary>
        /// 设置UI界面编号。
        /// </summary>
        public OpenFormRequest SetFormId(int formId)
        {
            FormId = formId;
            return this;
        }

        /// <summary>
        /// 设置业务透传数据。
        /// </summary>
        public OpenFormRequest SetUserData(object userData)
        {
            UserData = userData;
            return this;
        }

        /// <summary>
        /// 设置界面打开任务完成源。
        /// </summary>
        /// <param name="tcs">界面打开任务完成源。</param>
        /// <returns>打开界面请求。</returns>
        public OpenFormRequest SetFormOpenTcs(AutoResetUniTaskCompletionSource<int> tcs)
        {
            m_TcsFormOpen = tcs;
            return this;
        }

        /// <summary>
        /// 清理。
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            FormId = 0;
            CancellationToken = default;
            m_Tcs = null;
            m_TcsFormOpen = null;
            OnSetResponse = null;

            if (UserData is IReference reference)
            {
                ReferencePool.Release(reference);
            }

            UserData = null;
        }
    }
}
