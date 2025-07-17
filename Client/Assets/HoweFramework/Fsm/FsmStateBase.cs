namespace HoweFramework
{
    /// <summary>
    /// 有限状态机状态基类。
    /// </summary>
    public abstract class FsmStateBase
    {
        /// <summary>
        /// 有限状态机。
        /// </summary>
        protected IFsmMachine Fsm { get; private set; }

        /// <summary>
        /// 状态机黑板实例。
        /// </summary>
        protected IBlackboard Blackboard => Fsm?.Blackboard;

        /// <summary>
        /// 状态id。
        /// </summary>
        protected abstract int State { get; }

        /// <summary>
        /// 初始化。
        /// </summary>
        /// <param name="fsm">有限状态机。</param>
        internal void Initialize(IFsmMachine fsm)
        {
            Fsm = fsm;
            Fsm.AddState(State);
            Fsm.RegisterStateEnter(State, OnEnter);
            Fsm.RegisterStateExit(State, OnExit);
        }

        /// <summary>
        /// 切换状态。
        /// </summary>
        protected void ChangeState(int state)
        {
            Fsm.ChangeState(state);
        }

        /// <summary>
        /// 进入状态。
        /// </summary>
        protected abstract void OnEnter();

        /// <summary>
        /// 退出状态。
        /// </summary>
        protected abstract void OnExit();
    }
}
