namespace HoweFramework
{
    /// <summary>
    /// 有限状态机模块扩展。
    /// </summary>
    public static class FsmModuleExtensions
    {
        /// <summary>
        /// 添加状态。
        /// </summary>
        /// <typeparam name="TState">状态类型。</typeparam>
        /// <param name="fsm">有限状态机。</param>
        public static void AddState<TState>(this IFsmMachine fsm) where TState : FsmStateBase, new()
        {
            var state = new TState();
            state.Initialize(fsm);
        }

        /// <summary>
        /// 添加状态。
        /// </summary>
        /// <param name="fsm">有限状态机。</param>
        /// <param name="state">状态。</param>
        public static void AddState(this IFsmMachine fsm, FsmStateBase state)
        {
            state.Initialize(fsm);
        }
    }
}