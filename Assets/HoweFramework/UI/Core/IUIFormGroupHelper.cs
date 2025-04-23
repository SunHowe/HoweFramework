using System;

namespace HoweFramework
{
    /// <summary>
    /// UI界面分组辅助器接口。
    /// </summary>
    public interface IUIFormGroupHelper : IDisposable
    {
        /// <summary>
        /// 创建UI界面分组实例。
        /// </summary>
        /// <param name="name">分组名称。</param>
        /// <returns>UI界面分组实例。</returns>
        object CreateUIFormGroupInstance(string name);
    }
}
