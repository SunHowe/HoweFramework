using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 有限状态机。
    /// </summary>
    public sealed class FsmMachine : IFsmMachine, IReference
    {
        /// <summary>
        /// 状态进入事件。
        /// </summary>
        public event FsmStateChangeHandler OnStateEnter;

        /// <summary>
        /// 状态退出事件。
        /// </summary>
        public event FsmStateChangeHandler OnStateExit;

        /// <summary>
        /// 当前状态。
        /// </summary>
        public int CurrentState { get; private set; }

        /// <summary>
        /// 状态机黑板实例。
        /// </summary>
        public IBlackboard Blackboard { get; } = new Blackboard();

        /// <summary>
        /// 状态集合。
        /// </summary>
        private readonly HashSet<int> m_StateSet = new();

        /// <summary>
        /// 状态进入处理函数字典。
        /// </summary>
        private readonly Dictionary<int, FsmStateHandler> m_StateEnterHandlerDict = new();

        /// <summary>
        /// 状态退出处理函数字典。
        /// </summary>
        private readonly Dictionary<int, FsmStateHandler> m_StateExitHandlerDict = new();

        /// <summary>
        /// 是否正在切换状态。
        /// </summary>
        private bool m_IsChangingState;

        /// <summary>
        /// 切换状态期间是否有重入的待切换状态。
        /// </summary>
        private bool m_HasPendingState;

        /// <summary>
        /// 切换状态期间重入的待切换状态（0 表示停机，需配合 m_HasPendingState 区分）。
        /// </summary>
        private int m_PendingState;

        /// <summary>
        /// 是否已释放。
        /// </summary>
        private bool m_IsDisposed;

        /// <summary>
        /// 添加状态。
        /// </summary>
        public void AddState(int stateId)
        {
            if (stateId == 0)
            {
                throw new ErrorCodeException(FrameworkErrorCode.InvalidOperationException, "状态id不能为0");
            }

            m_StateSet.Add(stateId);
        }

        /// <summary>
        /// 切换状态。在状态进入/退出回调中重入调用时，将延迟到本次切换完成后生效（多次重入以最后一次为准）。
        /// </summary>
        public void ChangeState(int stateId)
        {
            if (stateId != 0 && !m_StateSet.Contains(stateId))
            {
                throw new ErrorCodeException(FrameworkErrorCode.InvalidOperationException, $"状态 {stateId} 不存在");
            }

            if (m_IsChangingState)
            {
                // 正在切换状态，记录目标状态，待本次切换完成后生效。
                m_PendingState = stateId;
                m_HasPendingState = true;
                return;
            }

            if (CurrentState == stateId)
            {
                return;
            }

            m_IsChangingState = true;
            try
            {
                while (true)
                {
                    m_HasPendingState = false;
                    m_PendingState = 0;

                    if (CurrentState != 0)
                    {
                        OnStateExit?.Invoke(CurrentState);

                        if (m_StateExitHandlerDict.TryGetValue(CurrentState, out var exitHandler))
                        {
                            exitHandler.Invoke();
                        }
                    }

                    CurrentState = stateId;

                    if (CurrentState != 0)
                    {
                        if (m_StateEnterHandlerDict.TryGetValue(CurrentState, out var enterHandler))
                        {
                            enterHandler.Invoke();
                        }

                        OnStateEnter?.Invoke(CurrentState);
                    }

                    // 处理切换期间重入的切换请求。
                    if (!m_HasPendingState || m_PendingState == CurrentState)
                    {
                        break;
                    }

                    stateId = m_PendingState;
                }
            }
            finally
            {
                m_IsChangingState = false;
                m_HasPendingState = false;
                m_PendingState = 0;
            }
        }

        /// <summary>
        /// 注册状态进入处理函数。
        /// </summary>
        /// <param name="stateId">状态id。</param>
        /// <param name="handler">事件处理函数。</param>
        /// <exception cref="ErrorCodeException">状态不存在或状态进入处理函数已存在。</exception>
        public void RegisterStateEnter(int stateId, FsmStateHandler handler)
        {
            if (stateId == 0)
            {
                throw new ErrorCodeException(FrameworkErrorCode.InvalidOperationException, "状态id不能为0");
            }

            if (!m_StateSet.Contains(stateId))
            {
                throw new ErrorCodeException(FrameworkErrorCode.InvalidOperationException, $"状态 {stateId} 不存在");
            }

            if (m_StateEnterHandlerDict.ContainsKey(stateId))
            {
                throw new ErrorCodeException(FrameworkErrorCode.InvalidOperationException, $"状态进入处理函数 {stateId} 已存在");
            }

            m_StateEnterHandlerDict[stateId] = handler;
        }

        /// <summary>
        /// 注册状态退出处理函数。
        /// </summary>
        /// <param name="stateId">状态id。</param>
        /// <param name="handler">事件处理函数。</param>
        /// <exception cref="ErrorCodeException">状态不存在或状态退出处理函数已存在。</exception>
        public void RegisterStateExit(int stateId, FsmStateHandler handler)
        {
            if (stateId == 0)
            {
                throw new ErrorCodeException(FrameworkErrorCode.InvalidOperationException, "状态id不能为0");
            }

            if (!m_StateSet.Contains(stateId))
            {
                throw new ErrorCodeException(FrameworkErrorCode.InvalidOperationException, $"状态 {stateId} 不存在");
            }

            if (m_StateExitHandlerDict.ContainsKey(stateId))
            {
                throw new ErrorCodeException(FrameworkErrorCode.InvalidOperationException, $"状态退出处理函数 {stateId} 已存在");
            }

            m_StateExitHandlerDict[stateId] = handler;
        }

        public void Dispose()
        {
            if (m_IsDisposed)
            {
                return;
            }

            m_IsDisposed = true;

            // 停止状态机。
            ChangeState(0);

            ReferencePool.Release(this);
        }

        public void Clear()
        {
            OnStateEnter = null;
            OnStateExit = null;
            m_StateEnterHandlerDict.Clear();
            m_StateExitHandlerDict.Clear();
            m_StateSet.Clear();
            CurrentState = 0;
            Blackboard.Clear();
            m_IsChangingState = false;
            m_HasPendingState = false;
            m_PendingState = 0;
            m_IsDisposed = false;
        }

        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <returns>有限状态机。</returns>
        public static FsmMachine Create()
        {
            return ReferencePool.Acquire<FsmMachine>();
        }
    }
}
