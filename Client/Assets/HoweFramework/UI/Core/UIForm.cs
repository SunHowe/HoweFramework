using System;
using System.Threading;

namespace HoweFramework
{
    /// <summary>
    /// 界面基类。
    /// </summary>
    internal sealed class UIForm : IUIForm
    {
        /// <summary>
        /// 界面编号。
        /// </summary>
        public int FormId => m_FormLogic.FormId;

        /// <summary>
        /// 界面组编号。
        /// </summary>
        public int FormGroupId => m_FormLogic.FormGroupId;

        /// <summary>
        /// 界面类型。
        /// </summary>
        public UIFormType FormType => m_FormLogic.FormType;

        /// <summary>
        /// 是否允许同时打开多个界面实例。
        /// </summary>
        public bool IsAllowMutiple => m_FormLogic.IsAllowMutiple;

        /// <summary>
        /// 是否允许由框架控制界面显示与隐藏。
        /// </summary>
        public bool IsAllowControlVisibleByFramework => m_FormLogic.IsAllowControlVisibleByFramework;

        /// <summary>
        /// 是否允许由框架控制界面关闭。
        /// </summary>
        public bool IsAllowControlCloseByFramework => m_FormLogic.IsAllowControlCloseByFramework;

        /// <summary>
        /// 界面序列编号。
        /// </summary>
        public int FormSerialId { get; private set; }

        /// <summary>
        /// 界面组。
        /// </summary>
        public IUIFormGroup FormGroup { get; private set; }

        /// <summary>
        /// 界面实例。
        /// </summary>
        public object FormInstance { get; private set; }

        /// <summary>
        /// 是否已加载。
        /// </summary>
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// 是否已打开。
        /// </summary>
        public bool IsOpen { get; private set; }

        /// <summary>
        /// 是否可见。
        /// </summary>
        public bool IsVisible { get; private set; }

        /// <summary>
        /// 当前的打开界面请求。
        /// </summary>
        public OpenFormRequest Request { get; private set; }

        /// <summary>
        /// 是否已绑定界面逻辑。缓存命中时为 true，此时不可再创建新的逻辑实例。
        /// </summary>
        internal bool HasFormLogic => m_FormLogic != null;

        /// <summary>
        /// 加载任务id。
        /// </summary>
        private int m_LoadId;

        /// <summary>
        /// UI界面辅助器。
        /// </summary>
        private IUIFormHelper m_UIFormHelper;

        /// <summary>
        /// 界面逻辑。
        /// </summary>
        private IUIFormLogic m_FormLogic;

        /// <summary>
        /// 排序顺序。
        /// </summary>
        private int m_SortingOrder;

        /// <summary>
        /// 打开请求取消回调注册。
        /// </summary>
        private CancellationTokenRegistration m_CancelRegistration;

        /// <summary>
        /// 取消回调对应的请求实例 id，用于对象池复用后过滤过期回调。
        /// </summary>
        private int m_CancelRequestInstanceId;

        /// <summary>
        /// 是否已处理加载失败，避免回调异常路径重入。
        /// </summary>
        private bool m_LoadFailureHandled;

        /// <summary>
        /// 初始化界面。
        /// </summary>
        /// <param name="serialId">界面序列编号。</param>
        /// <param name="group">界面所属的界面组。</param>
        /// <param name="uiFormHelper">UI界面辅助器。</param>
        /// <param name="uiFormLogic">界面逻辑。</param>
        public void Init(int serialId, IUIFormGroup group, IUIFormHelper uiFormHelper, IUIFormLogic uiFormLogic)
        {
            FormSerialId = serialId;
            FormGroup = group;
            m_UIFormHelper = uiFormHelper;
            m_FormLogic = uiFormLogic;
        }

        /// <summary>
        /// 为缓存复用的界面分配新的序列编号。
        /// </summary>
        /// <param name="serialId">界面序列编号。</param>
        internal void AssignSerialId(int serialId)
        {
            FormSerialId = serialId;
        }

        /// <summary>
        /// 销毁界面。
        /// </summary>
        public void Destroy()
        {
            UnregisterCancelCallback();

            if (IsLoaded)
            {
                m_FormLogic?.OnDestroy();

                // 卸载界面。
                m_UIFormHelper?.UnloadUIFormInstance(FormInstance);
            }
            else
            {
                if (m_LoadId != 0)
                {
                    // 卸载界面。
                    m_UIFormHelper?.CancelLoadUIFormInstance(m_LoadId);
                }

                m_FormLogic?.OnDestroy();
            }

            FormSerialId = 0;
            FormGroup = null;
            FormInstance = null;
            IsLoaded = false;
            IsOpen = false;
            IsVisible = false;
            m_LoadId = 0;
            m_IsClosing = false;
            m_LoadFailureHandled = false;
            m_SortingOrder = 0;
            m_UIFormHelper = null;
            m_FormLogic = null;
            InnerSetRequestResponse(CommonResponse.Create(FrameworkErrorCode.UIFormWhileDestroying));
        }

        /// <summary>
        /// 设置界面是否可见。
        /// </summary>
        /// <param name="visible">是否可见。</param>
        public void SetVisible(bool visible)
        {
            if (!IsOpen)
            {
                // 未打开，则触发异常。
                throw new ErrorCodeException(FrameworkErrorCode.UIFormNotOpen);
            }

            if (IsVisible == visible)
            {
                return;
            }

            IsVisible = visible;

            if (!IsLoaded)
            {
                // 未加载完成，则不处理。
                return;
            }

            if (IsVisible)
            {
                // 界面可见，则设置为可见。
                m_UIFormHelper.SetUIFormInstanceIsVisible(FormInstance, true);
                m_FormLogic.OnVisible();
            }
            else
            {
                // 界面不可见，则设置为不可见。
                m_UIFormHelper.SetUIFormInstanceIsVisible(FormInstance, false);
                m_FormLogic.OnInvisible();
            }
        }

        /// <summary>
        /// 处理打开界面请求。
        /// </summary>
        /// <param name="request">打开界面请求。</param>
        public void HandleOpenRequest(OpenFormRequest request)
        {
            // 使用错误码处理旧的请求。
            InnerSetRequestResponse(CommonResponse.Create(FrameworkErrorCode.UIFormNewOpenRequest));
            Request = request;
            request.OnSetResponse += OnRequestSetResponse;
            RegisterCancelCallback(request);

            if (!IsLoaded)
            {
                IsOpen = true;

                // 未加载完成，等待加载完成。
                if (m_LoadId == 0)
                {
                    // 加载界面。
                    m_LoadId = m_UIFormHelper.LoadUIFormInstance(FormId, OnLoadUIFormSuccess, OnLoadUIFormFailure);
                }
            }
            else
            {
                // 已加载完成，则根据状态进行打开。
                if (IsOpen)
                {
                    // 已打开，触发更新回调，并完成「只关心打开」的等待。
                    m_FormLogic.OnUpdate();

                    // 用户回调中可能已关闭界面（重入），此时不再继续。
                    if (!IsOpen)
                    {
                        return;
                    }

                    request.AsRef().Reference?.OnFormOpenSuccess();
                }
                else
                {
                    // 未打开，触发打开回调。
                    IsOpen = true;
                    m_UIFormHelper.SetUIFormInstanceIsOpen(FormInstance, FormGroup.GroupInstance, true);

                    var requestRef = request.AsRef();

                    m_FormLogic.OnOpen();

                    // 用户回调中可能已关闭界面（重入），此时不再继续。
                    if (!IsOpen)
                    {
                        return;
                    }

                    m_FormLogic.OnUpdate(); // 打开时也触发更新回调。

                    if (!IsOpen)
                    {
                        return;
                    }

                    requestRef.Reference?.OnFormOpenSuccess();
                }
            }

            // 设置为可见。
            if (!IsVisible)
            {
                SetVisible(true);
            }
        }

        /// <summary>
        /// 是否正在关闭流程中（防止 OnClose/OnInvisible 回调重入）。
        /// </summary>
        private bool m_IsClosing;

        /// <summary>
        /// 立即关闭界面。
        /// </summary>
        public void CloseImmediate()
        {
            if (!IsOpen)
            {
                // 未打开，则触发异常。
                throw new ErrorCodeException(FrameworkErrorCode.UIFormNotOpen);
            }

            if (m_IsClosing)
            {
                // 关闭流程重入（OnClose/OnInvisible 回调中再次关闭），直接返回。
                return;
            }

            m_IsClosing = true;
            try
            {
                if (!IsLoaded && m_LoadId != 0)
                {
                    m_UIFormHelper?.CancelLoadUIFormInstance(m_LoadId);
                    m_LoadId = 0;
                }

                try
                {
                    if (IsVisible)
                    {
                        SetVisible(false);
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"关闭界面时 OnInvisible 异常：{e.Message}\n{e.StackTrace}");
                }

                IsOpen = false;

                if (IsLoaded)
                {
                    try
                    {
                        m_UIFormHelper.SetUIFormInstanceIsOpen(FormInstance, FormGroup.GroupInstance, false);
                        m_FormLogic.OnClose();
                    }
                    catch (Exception e)
                    {
                        Log.Error($"关闭界面时 OnClose 异常：{e.Message}\n{e.StackTrace}");
                    }
                }

                int errorCode = m_FormLogic != null ? m_FormLogic.ErrorCodeOnClose : FrameworkErrorCode.Success;
                InnerSetRequestResponse(CommonResponse.Create(errorCode));
            }
            finally
            {
                m_IsClosing = false;
            }
        }

        /// <summary>
        /// 加载界面成功。
        /// </summary>
        /// <param name="formInstance">界面实例。</param>
        private void OnLoadUIFormSuccess(object formInstance)
        {
            m_LoadId = 0;

            try
            {
                FormInstance = formInstance;
                IsLoaded = true;

                // 触发逻辑初始化回调。
                m_FormLogic.OnInit(this);

                if (!IsOpen)
                {
                    // 未打开，则不处理。
                    return;
                }

                var requestRef = Request.AsRef();

                // 触发逻辑打开回调。
                m_UIFormHelper.SetUIFormInstanceSortingOrder(FormInstance, m_SortingOrder);
                m_UIFormHelper.SetUIFormInstanceIsOpen(FormInstance, FormGroup.GroupInstance, true);
                m_FormLogic.OnOpen();

                if (!IsOpen)
                {
                    // OnOpen 回调中界面被关闭（重入），不再继续。
                    return;
                }

                requestRef.Reference?.OnFormOpenSuccess();

                if (!IsVisible)
                {
                    // 界面不可见，设置实例状态并通知逻辑。
                    m_UIFormHelper.SetUIFormInstanceIsVisible(FormInstance, false);
                    m_FormLogic.OnInvisible();
                    return;
                }

                // 触发逻辑显示回调。
                m_UIFormHelper.SetUIFormInstanceIsVisible(FormInstance, true);
                m_FormLogic.OnVisible();
            }
            catch (Exception e)
            {
                Log.Error($"加载界面成功回调异常：{e.Message}\n{e.StackTrace}");
                OnLoadUIFormFailure(FrameworkErrorCode.Exception);
            }
        }

        /// <summary>
        /// 加载界面失败。
        /// </summary>
        /// <param name="errorCode">错误码。</param>
        private void OnLoadUIFormFailure(int errorCode)
        {
            if (m_LoadFailureHandled)
            {
                return;
            }

            m_LoadFailureHandled = true;
            m_LoadId = 0;
            InnerSetRequestResponse(CommonResponse.Create(errorCode));
            UIModule.Instance.HandleUIFormLoadFailure(this);
        }

        /// <summary>
        /// 请求设置响应回调。
        /// </summary>
        /// <param name="request">打开界面请求。</param>
        private void OnRequestSetResponse(OpenFormRequest request)
        {
            if (Request != request)
            {
                return;
            }

            UnregisterCancelCallback();

            // 清空请求引用。
            Request = null;

            // 关闭界面。
            CloseForm();
        }

        /// <summary>
        /// 请求取消。
        /// </summary>
        private void OnRequestCancel(object param)
        {
            if (param is not OpenFormRequest request)
            {
                return;
            }

            if (Request != request || request.InstanceId != m_CancelRequestInstanceId)
            {
                return;
            }

            request.SetResponse(CommonResponse.Create(FrameworkErrorCode.RequestCanceled));
        }

        /// <summary>
        /// 注册打开请求的取消回调。
        /// </summary>
        private void RegisterCancelCallback(OpenFormRequest request)
        {
            UnregisterCancelCallback();
            m_CancelRequestInstanceId = request.InstanceId;
            m_CancelRegistration = request.CancellationToken.Register(OnRequestCancel, request);
        }

        /// <summary>
        /// 注销打开请求的取消回调。
        /// </summary>
        private void UnregisterCancelCallback()
        {
            m_CancelRegistration.Dispose();
            m_CancelRegistration = default;
            m_CancelRequestInstanceId = 0;
        }

        /// <summary>
        /// 内部设置请求响应。
        /// </summary>
        private void InnerSetRequestResponse(ResponseBase response)
        {
            UnregisterCancelCallback();

            if (Request == null)
            {
                response.Dispose();
                return;
            }

            var request = Request;
            Request = null;
            request.SetResponse(response);
        }

        /// <summary>
        /// 关闭界面。
        /// </summary>
        public void CloseForm()
        {
            UIModule.Instance.CloseUIForm(FormId, FormSerialId);
        }

        /// <summary>
        /// 更新排序顺序。
        /// </summary>
        /// <param name="sortingOrder">排序顺序。</param>
        public void SetSortingOrder(int sortingOrder)
        {
            m_SortingOrder = sortingOrder;

            if (IsLoaded)
            {
                m_UIFormHelper.SetUIFormInstanceSortingOrder(FormInstance, sortingOrder);
            }
        }
    }
}
