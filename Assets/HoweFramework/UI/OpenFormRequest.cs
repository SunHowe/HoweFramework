using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 打开UI请求。
    /// </summary>
    public class OpenFormRequest : RequestBase
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

        private AutoResetUniTaskCompletionSource<ResponseBase> m_Tcs;

        protected override UniTask<ResponseBase> OnExecute(CancellationToken token)
        {
            CancellationToken = token;
            m_Tcs = AutoResetUniTaskCompletionSource<ResponseBase>.Create();
            var task = m_Tcs.Task;
            UIModule.Instance.HandleOpenFormRequest(this);
            return task;
        }

        /// <summary>
        /// 设置响应。
        /// </summary>
        /// <param name="response">响应。</param>
        public void SetResponse(ResponseBase response)
        {
            if (m_Tcs == null)
            {
                return;
            }

            OnSetResponse?.Invoke(this);
            m_Tcs.TrySetResult(response);
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
            UserData = null;
        }
    }
}
