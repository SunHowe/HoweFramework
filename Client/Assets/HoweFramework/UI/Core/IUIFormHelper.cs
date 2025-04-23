using System;

namespace HoweFramework
{
    /// <summary>
    /// UI界面辅助器接口。
    /// </summary>
    public interface IUIFormHelper : IDisposable
    {
        /// <summary>
        /// 创建界面逻辑。
        /// </summary>
        /// <param name="uiFormId">界面编号。</param>
        /// <returns>界面逻辑。</returns>
        IUIFormLogic CreateUIFormLogic(int uiFormId);
        
        /// <summary>
        /// 加载界面实例。
        /// </summary>
        /// <param name="uiFormId">界面编号。</param>
        /// <param name="onLoadSuccess">加载成功回调。</param>
        /// <returns>加载任务id。</returns>
        int LoadUIFormInstance(int uiFormId, Action<object> onLoadSuccess);

        /// <summary>
        /// 卸载界面实例。
        /// </summary>
        /// <param name="uiFormInstance">界面实例。</param>
        void UnloadUIFormInstance(object uiFormInstance);

        /// <summary>
        /// 取消加载界面实例。
        /// </summary>
        /// <param name="loadId">加载任务id。</param>
        void CancelLoadUIFormInstance(int loadId);

        /// <summary>
        /// 设置界面实例是否可见。
        /// </summary>
        /// <param name="uiFormInstance">界面实例。</param>
        /// <param name="visible">是否可见。</param>
        void SetUIFormInstanceIsVisible(object uiFormInstance, bool visible);

        /// <summary>
        /// 设置界面实例是否打开。
        /// </summary>
        /// <param name="uiFormInstance">界面实例。</param>
        /// <param name="uiGroupInstance">界面组实例。</param>
        /// <param name="isOpen">是否打开。</param>
        void SetUIFormInstanceIsOpen(object uiFormInstance, object uiGroupInstance, bool isOpen);
    }
}
