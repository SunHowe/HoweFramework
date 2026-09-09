using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// UI模块。
    /// </summary>
    public sealed class UIModule : ModuleBase<UIModule>
    {
        /// <summary>
        /// 界面分组字典。
        /// </summary>
        private readonly Dictionary<int, IUIFormGroup> m_UIFormGroupDict = new();

        /// <summary>
        /// 界面分组列表。
        /// </summary>
        private readonly List<IUIFormGroup> m_UIFormGroupList = new();

        /// <summary>
        /// 已打开界面实例列表。
        /// </summary>
        private readonly LinkedListEx<UIForm> m_UIFormOpenedList = new();

        /// <summary>
        /// 界面实例缓存队列字典。
        /// </summary>
        private readonly Dictionary<int, ReusableQueue<UIForm>> m_UIFormCacheDict = new();

        /// <summary>
        /// UI请求队列。
        /// </summary>
        private readonly Queue<RequestBase> m_RequestQueue = new();

        /// <summary>
        /// 是否正在处理请求。
        /// </summary>
        private bool m_IsProcessingRequest = false;

        /// <summary>
        /// UI界面辅助器。
        /// </summary>
        private IUIFormHelper m_UIFormHelper;

        /// <summary>
        /// UI界面分组辅助器。
        /// </summary>
        private IUIFormGroupHelper m_UIFormGroupHelper;

        /// <summary>
        /// 界面序列编号。
        /// </summary>
        private int m_UIFormSerialId = 0;

        /// <summary>
        /// 设置UI界面辅助器。
        /// </summary>
        /// <param name="uiFormHelper">UI界面辅助器。</param>
        public void SetUIFormHelper(IUIFormHelper uiFormHelper)
        {
            m_UIFormHelper = uiFormHelper;
        }

        /// <summary>
        /// 设置UI界面分组辅助器。
        /// </summary>
        /// <param name="uiFormGroupHelper">UI界面分组辅助器。</param>
        public void SetUIFormGroupHelper(IUIFormGroupHelper uiFormGroupHelper)
        {
            m_UIFormGroupHelper = uiFormGroupHelper;
        }

        /// <summary>
        /// 创建界面分组。
        /// </summary>
        /// <param name="groupId">界面分组编号。</param>
        /// <param name="groupName">界面分组名称。</param>
        public void CreateUIFormGroup(int groupId, string groupName)
        {
            if (m_UIFormGroupDict.ContainsKey(groupId))
            {
                throw new ErrorCodeException(FrameworkErrorCode.UIFormGroupAlreadyExists);
            }

            if (m_UIFormGroupHelper == null)
            {
                throw new ErrorCodeException(FrameworkErrorCode.UIFormGroupHelperNotSet);
            }

            var instance = m_UIFormGroupHelper.CreateUIFormGroupInstance(groupName);

            m_UIFormGroupDict[groupId] = new UIFormGroup(groupId, groupName, instance);
            m_UIFormGroupList.Add(m_UIFormGroupDict[groupId]);
        }

        /// <summary>
        /// 获取界面分组。
        /// </summary>
        /// <param name="groupId">界面分组编号。</param>
        /// <returns>界面分组。</returns>
        public IUIFormGroup GetUIFormGroup(int groupId)
        {
            if (m_UIFormGroupDict.TryGetValue(groupId, out var uiFormGroup))
            {
                return uiFormGroup;
            }

            throw new ErrorCodeException(FrameworkErrorCode.UIFormGroupNotExist);
        }

        /// <summary>
        /// 销毁所有缓存界面。
        /// </summary>
        public void DestroyCacheForms()
        {
            foreach (var cache in m_UIFormCacheDict.Values)
            {
                foreach (var form in cache)
                {
                    form.Destroy();
                }

                cache.Dispose();
            }

            m_UIFormCacheDict.Clear();
        }

        /// <summary>
        /// 处理打开界面请求。
        /// </summary>
        internal void HandleOpenFormRequest(OpenFormRequest request)
        {
            // 将请求加入队列。
            m_RequestQueue.Enqueue(request);

            ProcessRequestQueue();
        }

        /// <summary>
        /// 处理关闭界面请求。
        /// </summary>
        internal void HandleCloseFormRequest(CloseFormRequest request)
        {
            // 将请求加入队列。
            m_RequestQueue.Enqueue(request);

            ProcessRequestQueue();
        }

        /// <summary>
        /// 界面加载失败时从打开列表移除并销毁，避免幽灵界面。
        /// </summary>
        internal void HandleUIFormLoadFailure(UIForm uiForm)
        {
            if (uiForm == null)
            {
                return;
            }

            if (uiForm.FormGroup != null)
            {
                ((UIFormGroup)uiForm.FormGroup).RemoveUIForm(uiForm);
            }

            m_UIFormOpenedList.Remove(uiForm);
            RemoveUIFormFromCache(uiForm);

            if (uiForm.IsOpen)
            {
                uiForm.CloseImmediate();
            }

            UpdateUIStack();
            uiForm.Destroy();
        }

        /// <summary>
        /// 处理请求队列。
        /// </summary>
        private void ProcessRequestQueue()
        {
            if (m_IsProcessingRequest)
            {
                return;
            }

            if (m_RequestQueue.Count <= 0)
            {
                return;
            }

            m_IsProcessingRequest = true;

            try
            {
                while (m_RequestQueue.Count > 0)
                {
                    var request = m_RequestQueue.Dequeue();

                    switch (request)
                    {
                        case OpenFormRequest openFormRequest:
                            if (openFormRequest.CancellationToken.IsCancellationRequested)
                            {
                                openFormRequest.SetResponse(CommonResponse.Create(FrameworkErrorCode.RequestCanceled));
                            }
                            else
                            {
                                RealHandleOpenFormRequest(openFormRequest);
                            }

                            break;
                        case CloseFormRequest closeFormRequest:
                            RealHandleCloseFormRequest(closeFormRequest);
                            break;
                    }
                }
            }
            finally
            {
                m_IsProcessingRequest = false;
            }
        }

        /// <summary>
        /// 处理打开界面请求。
        /// </summary>
        /// <param name="request">打开界面请求。</param>
        private void RealHandleOpenFormRequest(OpenFormRequest request)
        {
            try
            {
                var uiForm = FindOpenedUIForm(request.FormId);
                if (uiForm != null && !uiForm.IsAllowMutiple)
                {
                    // 若界面已存在，且不允许存在多个实例，则将旧界面从分组上移除并换新序列号。
                    ((UIFormGroup)uiForm.FormGroup).RemoveUIForm(uiForm);
                    m_UIFormOpenedList.Remove(uiForm);
                    uiForm.AssignSerialId(++m_UIFormSerialId);

                    try
                    {
                        BeforeUIFormOpen(uiForm);
                    }
                    catch
                    {
                        m_UIFormOpenedList.AddLast(uiForm);
                        ((UIFormGroup)uiForm.FormGroup).AddUIForm(uiForm);
                        throw;
                    }
                }
                else
                {
                    // 创建界面。
                    if (m_UIFormHelper == null)
                    {
                        throw new ErrorCodeException(FrameworkErrorCode.UIFormHelperNotSet);
                    }

                    uiForm = CreateUIForm(request.FormId);
                    if (uiForm.HasFormLogic)
                    {
                        // 缓存命中：复用已 OnInit 的逻辑，只换序列号。新建逻辑会导致 OnOpen 时 UIForm 仍为 null。
                        uiForm.AssignSerialId(++m_UIFormSerialId);
                    }
                    else
                    {
                        var uiFormLogic = m_UIFormHelper.CreateUIFormLogic(request.FormId);
                        if (uiFormLogic == null)
                        {
                            throw new ErrorCodeException(FrameworkErrorCode.UIFormLogicNotFound);
                        }

                        uiForm.Init(++m_UIFormSerialId, GetUIFormGroup(uiFormLogic.FormGroupId), m_UIFormHelper, uiFormLogic);
                    }

                    BeforeUIFormOpen(uiForm);
                }

                // 将界面加入已打开界面列表。
                m_UIFormOpenedList.AddLast(uiForm);

                // 将界面加入分组。
                ((UIFormGroup)uiForm.FormGroup).AddUIForm(uiForm);

                // 触发界面打开事件。
                uiForm.HandleOpenRequest(request);

                // 更新UI栈。
                UpdateUIStack();
            }
            catch (ErrorCodeException e)
            {
                Log.Error($"处理UI打开请求时发生异常：ErrorCode={e.ErrorCode}, Message={e.Message}\n{e.StackTrace}");
                request.SetResponse(CommonResponse.Create(e.ErrorCode));
            }
            catch (Exception e)
            {
                Log.Error($"处理UI打开请求时发生异常：{e.Message}\n{e.StackTrace}");
                request.SetResponse(CommonResponse.Create(FrameworkErrorCode.Exception));
            }
        }

        /// <summary>
        /// 处理关闭界面请求。
        /// </summary>
        /// <param name="request">关闭界面请求。</param>
        private void RealHandleCloseFormRequest(CloseFormRequest request)
        {
            try
            {
                if (request.CloseMutiple)
                {
                    var closed = false;
                    while (true)
                    {
                        var form = FindOpenedUIForm(request.FormId);
                        if (form == null)
                        {
                            break;
                        }

                        CloseAndCacheUIForm(form);
                        closed = true;
                    }

                    if (!closed)
                    {
                        request.SetResponse(CommonResponse.Create(FrameworkErrorCode.UIFormNotOpen));
                        return;
                    }

                    UpdateUIStack();
                    request.SetResponse(CommonResponse.Create(FrameworkErrorCode.Success));
                    return;
                }

                var uiForm = FindOpenedUIForm(request.FormId, request.FormSerialId);
                if (uiForm == null)
                {
                    // 界面未打开。
                    request.SetResponse(CommonResponse.Create(FrameworkErrorCode.UIFormNotOpen));
                    return;
                }

                CloseAndCacheUIForm(uiForm);
                UpdateUIStack();
                request.SetResponse(CommonResponse.Create(FrameworkErrorCode.Success));
            }
            catch (ErrorCodeException e)
            {
                Log.Error($"处理UI关闭请求时发生异常：ErrorCode={e.ErrorCode}, Message={e.Message}\n{e.StackTrace}");
                request.SetResponse(CommonResponse.Create(e.ErrorCode));
            }
            catch (Exception e)
            {
                Log.Error($"处理UI关闭请求时发生异常：{e.Message}\n{e.StackTrace}");
                request.SetResponse(CommonResponse.Create(FrameworkErrorCode.Exception));
            }
        }

        /// <summary>
        /// 立即关闭界面并放入缓存。不更新 UI 栈，由调用方在批量操作结束后更新。
        /// </summary>
        private void CloseAndCacheUIForm(UIForm uiForm)
        {
            if (uiForm.IsOpen)
            {
                try
                {
                    uiForm.CloseImmediate();
                }
                catch (Exception e)
                {
                    Log.Error($"关闭界面 {uiForm.FormId} 时发生异常：{e.Message}\n{e.StackTrace}");
                }
            }

            if (uiForm.FormGroup != null)
            {
                ((UIFormGroup)uiForm.FormGroup).RemoveUIForm(uiForm);
            }

            m_UIFormOpenedList.Remove(uiForm);
            CacheUIForm(uiForm);
        }

        /// <summary>
        /// 处理界面打开前逻辑。
        /// </summary>
        /// <param name="uiForm">界面实例。</param>
        private void BeforeUIFormOpen(UIForm uiForm)
        {
            // 根据界面类型进行处理。
            switch (uiForm.FormType)
            {
                case UIFormType.Main:
                    // 主界面。关闭其他所有界面。
                    {
                        var node = m_UIFormOpenedList.First;
                        while (node != null)
                        {
                            var form = node.Value;
                            var nextNode = node.Next;
                            if (!form.IsAllowControlCloseByFramework)
                            {
                                node = nextNode;
                                continue;
                            }

                            CloseAndCacheUIForm(form);
                            node = nextNode;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 更新UI栈。
        /// </summary>
        private void UpdateUIStack()
        {
            // 是否已经找到普通界面。
            var foundNormalForm = false;

            // 从栈顶开始遍历。
            var node = m_UIFormOpenedList.Last;
            int sortingOrder = m_UIFormOpenedList.Count;

            while (node != null)
            {
                var uiForm = node.Value;
                node = node.Previous;

                if (uiForm.FormType == UIFormType.Fixed)
                {
                    // 不处理固定界面。
                    continue;
                }

                uiForm.SetSortingOrder(sortingOrder);
                --sortingOrder;

                if (uiForm.IsAllowControlVisibleByFramework)
                {
                    // 设置界面是否可见。
                    uiForm.SetVisible(!foundNormalForm);
                }

                if (foundNormalForm)
                {
                    continue;
                }

                if (uiForm.FormType == UIFormType.Normal)
                {
                    foundNormalForm = true;
                }
            }
        }

        /// <summary>
        /// 查找已打开的界面实例。
        /// </summary>
        /// <param name="uiFormId">界面编号。</param>
        /// <param name="uiFormSerialId">界面序列编号。</param>
        /// <returns>已打开的界面实例。</returns>
        private UIForm FindOpenedUIForm(int uiFormId, int uiFormSerialId = 0)
        {
            var node = m_UIFormOpenedList.First;
            while (node != null)
            {
                var uiForm = node.Value;
                node = node.Next;
                if (uiForm.FormId == uiFormId && (uiFormSerialId == 0 || uiForm.FormSerialId == uiFormSerialId))
                {
                    return uiForm;
                }
            }

            return null;
        }

        /// <summary>
        /// 创建界面实例。
        /// </summary>
        /// <param name="uiFormId">界面编号。</param>
        /// <returns>界面实例。</returns>
        private UIForm CreateUIForm(int uiFormId)
        {
            // 优先从缓存中获取。
            if (m_UIFormCacheDict.TryGetValue(uiFormId, out var cache))
            {
                var uiForm = cache.Dequeue();
                if (cache.Count == 0)
                {
                    m_UIFormCacheDict.Remove(uiFormId);
                    cache.Dispose();
                }

                return uiForm;
            }

            return new UIForm();
        }

        /// <summary>
        /// 从缓存队列移除指定界面（加载失败 Destroy 前必须调用，避免幽灵缓存）。
        /// </summary>
        private void RemoveUIFormFromCache(UIForm uiForm)
        {
            if (!m_UIFormCacheDict.TryGetValue(uiForm.FormId, out var cache))
            {
                return;
            }

            int count = cache.Count;
            for (int i = 0; i < count; i++)
            {
                var form = cache.Dequeue();
                if (form != uiForm)
                {
                    cache.Enqueue(form);
                }
            }

            if (cache.Count == 0)
            {
                m_UIFormCacheDict.Remove(uiForm.FormId);
                cache.Dispose();
            }
        }

        /// <summary>
        /// 缓存界面实例。
        /// </summary>
        /// <param name="uiForm">界面实例。</param>
        private void CacheUIForm(UIForm uiForm)
        {
            if (!m_UIFormCacheDict.TryGetValue(uiForm.FormId, out var cache))
            {
                cache = ReusableQueue<UIForm>.Create();
                m_UIFormCacheDict[uiForm.FormId] = cache;
            }

            cache.Enqueue(uiForm);
        }

        /// <summary>
        /// 完成队列中尚未处理的请求，避免外部 await 挂起。
        /// </summary>
        private void CompletePendingRequests(int errorCode)
        {
            while (m_RequestQueue.Count > 0)
            {
                var request = m_RequestQueue.Dequeue();
                switch (request)
                {
                    case OpenFormRequest openFormRequest:
                        openFormRequest.SetResponse(CommonResponse.Create(errorCode));
                        break;
                    case CloseFormRequest closeFormRequest:
                        closeFormRequest.SetResponse(CommonResponse.Create(errorCode));
                        break;
                }
            }
        }

        protected override void OnInit()
        {
        }

        protected override void OnDestroy()
        {
            CompletePendingRequests(FrameworkErrorCode.UIFormWhileDestroying);

            // 销毁所有已打开的界面。单个界面关闭/销毁异常不中断销毁链。
            var node = m_UIFormOpenedList.First;
            while (node != null)
            {
                var form = node.Value;
                node = node.Next;

                try
                {
                    if (form.IsOpen)
                    {
                        form.CloseImmediate();
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"关闭界面 {form.FormId} 时发生异常：{e.Message}\n{e.StackTrace}");
                }

                try
                {
                    form.Destroy();
                }
                catch (Exception e)
                {
                    Log.Error($"销毁界面 {form.FormId} 时发生异常：{e.Message}\n{e.StackTrace}");
                }
            }

            m_UIFormOpenedList.Clear();
            m_UIFormGroupDict.Clear();
            m_UIFormGroupList.Clear();

            // 清空缓存。
            DestroyCacheForms();

            m_UIFormHelper?.Dispose();
            m_UIFormHelper = null;
            m_UIFormGroupHelper?.Dispose();
            m_UIFormGroupHelper = null;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            ProcessRequestQueue();
        }
    }
}
