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
        /// 添加状态。
        /// </summary>
        public void AddState(int stateId)
        {
            if (stateId == 0)
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "状态id不能为0");
            }

            m_StateSet.Add(stateId);
        }

        /// <summary>
        /// 切换状态。
        /// </summary>
        public void ChangeState(int stateId)
        {
            if (CurrentState == stateId)
            {
                return;
            }

            if (stateId != 0 && !m_StateSet.Contains(stateId))
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, $"状态 {stateId} 不存在");
            }

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
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "状态id不能为0");
            }

            if (!m_StateSet.Contains(stateId))
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, $"状态 {stateId} 不存在");
            }

            if (m_StateEnterHandlerDict.ContainsKey(stateId))
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, $"状态进入处理函数 {stateId} 已存在");
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
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, "状态id不能为0");
            }

            if (!m_StateSet.Contains(stateId))
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, $"状态 {stateId} 不存在");
            }

            if (m_StateExitHandlerDict.ContainsKey(stateId))
            {
                throw new ErrorCodeException(ErrorCode.InvalidOperationException, $"状态退出处理函数 {stateId} 已存在");
            }

            m_StateExitHandlerDict[stateId] = handler;
        }

        public void Dispose()
        {
            // 停止状态机。
            ChangeState(0);

            ReferencePool.Release(this);
        }

        public void Clear()
        {
            m_StateEnterHandlerDict.Clear();
            m_StateExitHandlerDict.Clear();
            m_StateSet.Clear();
            CurrentState = 0;
            Blackboard.Clear();
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
