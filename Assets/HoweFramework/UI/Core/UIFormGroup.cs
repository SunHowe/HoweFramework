namespace HoweFramework
{
    /// <summary>
    /// UI界面分组。
    /// </summary>
    internal sealed class UIFormGroup : IUIFormGroup
    {
        public int GroupId { get; }

        public string GroupName { get; }

        public object GroupInstance { get; }

        public UIFormGroup(int groupId, string groupName, object groupInstance)
        {
            GroupId = groupId;
            GroupName = groupName;
            GroupInstance = groupInstance;
        }

        /// <summary>
        /// 添加界面。
        /// </summary>
        /// <param name="uiForm">界面。</param>
        internal void AddUIForm(IUIForm uiForm)
        {
        }

        /// <summary>
        /// 移除界面。
        /// </summary>
        /// <param name="uiForm">界面。</param>
        internal void RemoveUIForm(IUIForm uiForm)
        {
        }
    }
}

