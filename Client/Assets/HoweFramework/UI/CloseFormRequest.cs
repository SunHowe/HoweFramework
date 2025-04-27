using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 关闭UI请求。
    /// </summary>
    public class CloseFormRequest : RequestBase
    {
        /// <summary>
        /// UI界面编号。
        /// </summary>
        public int FormId { get; set; }

        /// <summary>
        /// 界面序列编号。若有指定则会判断指定的界面序列编号是否与当前打开的界面序列编号一致，若不一致则不关闭。
        /// </summary>
        public int FormSerialId { get; set; }

        /// <summary>
        /// 若存在多个实例，是否关闭所有实例。
        /// </summary>
        public bool CloseMutiple { get; set; }

        private AutoResetUniTaskCompletionSource<IResponse> m_Tcs;

        protected override UniTask<IResponse> OnExecute(CancellationToken token)
        {
            m_Tcs = AutoResetUniTaskCompletionSource<IResponse>.Create();
            var task = m_Tcs.Task;
            UIModule.Instance.HandleCloseFormRequest(this);
            return task;
        }

        /// <summary>
        /// 设置响应。
        /// </summary>
        /// <param name="response">响应。</param>
        public void SetResponse(IResponse response)
        {
            m_Tcs?.TrySetResult(response);
        }

        /// <summary>
        /// 设置UI界面编号。
        /// </summary>
        public CloseFormRequest SetFormId(int formId)
        {
            FormId = formId;
            return this;
        }

        /// <summary>
        /// 设置界面序列编号。
        /// </summary>
        public CloseFormRequest SetFormSerialId(int formSerialId)
        {
            FormSerialId = formSerialId;
            return this;
        }

        /// <summary>
        /// 设置是否关闭所有实例。
        /// </summary>
        public CloseFormRequest SetCloseMutiple(bool closeMutiple)
        {
            CloseMutiple = closeMutiple;
            return this;
        }

        /// <summary>
        /// 清理。
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            FormId = 0;
            FormSerialId = 0;
            CloseMutiple = false;
            m_Tcs = null;
        }
    }
}
