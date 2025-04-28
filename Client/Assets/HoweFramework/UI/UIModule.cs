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
                throw new ErrorCodeException(ErrorCode.UIFormGroupAlreadyExists);
            }

            if (m_UIFormGroupHelper == null)
            {
                throw new ErrorCodeException(ErrorCode.UIFormGroupHelperNotSet);
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

            throw new ErrorCodeException(ErrorCode.UIFormGroupNotExist);
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

            while (m_RequestQueue.Count > 0)
            {
                var request = m_RequestQueue.Dequeue();

                switch (request)
                {
                    case OpenFormRequest openFormRequest:
                        if (!openFormRequest.CancellationToken.IsCancellationRequested)
                        {
                            RealHandleOpenFormRequest(openFormRequest);
                        }
                        break;
                    case CloseFormRequest closeFormRequest:
                        RealHandleCloseFormRequest(closeFormRequest);
                        break;
                }
            }

            m_IsProcessingRequest = false;
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
                    // 若界面已存在，且不允许存在多个实例，则将旧界面从分组上移除。
                    ((UIFormGroup)uiForm.FormGroup).RemoveUIForm(uiForm);
                    m_UIFormOpenedList.Remove(uiForm);
                }
                else
                {
                    // 创建界面。
                    if (m_UIFormHelper == null)
                    {
                        throw new ErrorCodeException(ErrorCode.UIFormHelperNotSet);
                    }

                    var uiFormLogic = m_UIFormHelper.CreateUIFormLogic(request.FormId);
                    if (uiFormLogic == null)
                    {
                        throw new ErrorCodeException(ErrorCode.UIFormLogicNotFound);
                    }

                    uiForm = CreateUIForm(request.FormId);
                    uiForm.Init(++m_UIFormSerialId, GetUIFormGroup(request.FormId), m_UIFormHelper, uiFormLogic);
                }

                // 处理界面打开前逻辑。
                BeforeUIFormOpen(uiForm);

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
                request.SetResponse(CommonResponse.Create(ErrorCode.Exception));
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
                var uiForm = FindOpenedUIForm(request.FormId, request.FormSerialId);
                if (uiForm == null)
                {
                    // 界面未打开。
                    request.SetResponse(CommonResponse.Create(ErrorCode.UIFormNotOpen));
                    return;
                }

                // 触发界面关闭。
                uiForm.CloseImmediate();

                // 将界面从分组移除。
                ((UIFormGroup)uiForm.FormGroup).RemoveUIForm(uiForm);

                // 将界面从已打开界面列表移除。
                m_UIFormOpenedList.Remove(uiForm);

                // 更新UI栈。
                UpdateUIStack();

                // 将界面加入缓存。
                CacheUIForm(uiForm);

                // 设置请求成功。
                request.SetResponse(CommonResponse.Create(ErrorCode.Success));
            }
            catch (ErrorCodeException e)
            {
                Log.Error($"处理UI关闭请求时发生异常：ErrorCode={e.ErrorCode}, Message={e.Message}\n{e.StackTrace}");
                request.SetResponse(CommonResponse.Create(e.ErrorCode));
            }
            catch (Exception e)
            {
                Log.Error($"处理UI关闭请求时发生异常：{e.Message}\n{e.StackTrace}");
                request.SetResponse(CommonResponse.Create(ErrorCode.Exception));
            }
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
                            node = node.Next;
                            if (!form.IsAllowControlCloseByFramework)
                            {
                                continue;
                            }

                            form.CloseImmediate();
                            m_UIFormOpenedList.Remove(node.Previous);
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

                if (uiForm.FormType == UIFormType.Main)
                {
                    // 处理到主界面为止，不可能存在更底下的界面。
                    break;
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
                }

                return uiForm;
            }

            return new UIForm();
        }

        /// <summary>
        /// 缓存界面实例。
        /// </summary>
        /// <param name="uiForm">界面实例。</param>
        private void CacheUIForm(UIForm uiForm)
        {
            if (!m_UIFormCacheDict.TryGetValue(uiForm.FormId, out var cache))
            {
                cache = new ReusableQueue<UIForm>();
                m_UIFormCacheDict[uiForm.FormId] = cache;
            }

            cache.Enqueue(uiForm);
        }

        protected override void OnInit()
        {
        }

        protected override void OnDestroy()
        {
            // 销毁所有已打开的界面。
            var node = m_UIFormOpenedList.First;
            while (node != null)
            {
                var form = node.Value;
                node = node.Next;

                form.CloseImmediate();
                form.Destroy();
            }

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
