using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 大厅流程。
    /// </summary>
    public sealed class ProcedureLobby : ProcedureBase
    {
        public override int Id => (int)ProcedureId.Lobby;

        protected override void OnEnter()
        {
            // 打开大厅界面。
        }

        protected override void OnLeave()
        {
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (LoginSystem.Instance.LoginState.Value < LoginStateType.OnGame)
            {
                ChangeProcedure((int)ProcedureId.Login);
            }
        }
    }
}
