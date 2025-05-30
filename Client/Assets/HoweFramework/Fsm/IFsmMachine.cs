using System;

namespace HoweFramework
{
    /// <summary>
    /// 状态处理函数。
    /// </summary>
    public delegate void FsmStateHandler();

    /// <summary>
    /// 有限状态机。
    /// </summary>
    public interface IFsmMachine : IDisposable
    {
        /// <summary>
        /// 当前状态。
        /// </summary>
        int CurrentState { get; }

        /// <summary>
        /// 切换状态。
        /// </summary>
        /// <param name="stateId">状态id。</param>
        void ChangeState(int stateId);

        /// <summary>
        /// 添加状态。
        /// </summary>
        /// <param name="stateId">状态id。</param>
        void AddState(int stateId);

        /// <summary>
        /// 注册状态进入处理函数。
        /// </summary>
        /// <param name="handler">事件处理函数。</param>
        void RegisterStateEnter(int stateId, FsmStateHandler handler);

        /// <summary>
        /// 注册状态退出处理函数。
        /// </summary>
        /// <param name="stateId">状态id。</param>
        /// <param name="handler">事件处理函数。</param>
        void RegisterStateExit(int stateId, FsmStateHandler handler);
    }
}

