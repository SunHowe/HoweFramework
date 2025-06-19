using System;

namespace HoweFramework
{
    /// <summary>
    /// 流程控制器接口。
    /// </summary>
    public interface IProcedureController : IDisposable
    {
        /// <summary>
        /// 初始化。
        /// </summary>
        void Initialize();
    }
}