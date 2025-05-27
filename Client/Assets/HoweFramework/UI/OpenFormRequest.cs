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

            OnSetResponse?.Invoke(this);
            m_Tcs.TrySetResult(response);
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
        /// 清理。
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            FormId = 0;
            CancellationToken = default;
            m_Tcs = null;
            OnSetResponse = null;

            if (UserData is IReference reference)
            {
                ReferencePool.Release(reference);
            }

            UserData = null;
        }
    }
}
