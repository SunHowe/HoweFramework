using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 游戏流程。
    /// </summary>
    public sealed class ProcedureGame : ProcedureBase
    {
        public override int Id => (int)ProcedureId.Game;

        private IGameContext m_GameContext;
        private IGameUpdateManager m_GameUpdateManager;

        protected override void OnEnter()
        {
            // 取出游戏上下文。
            m_GameContext = IOCModule.Instance.Get<IGameContext>();
            m_GameContext.Awake();

            m_GameUpdateManager = m_GameContext.GetManager<IGameUpdateManager>();

            // 单机游戏在Awake后可以立即StartGame。
            m_GameContext.StartGame();
        }

        protected override void OnLeave()
        {
            // 清理游戏上下文。
            IOCModule.Instance.UnRegister(m_GameContext);
            
            m_GameContext.Dispose();
            m_GameContext = null;
            m_GameUpdateManager = null;
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            m_GameUpdateManager?.Update(elapseSeconds);
        }
    }
}