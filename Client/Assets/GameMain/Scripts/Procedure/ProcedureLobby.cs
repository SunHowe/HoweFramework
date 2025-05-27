using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 大厅流程。
    /// </summary>
    public sealed class ProcedureLobby : ProcedureBase
    {
        public override int Id => (int)ProcedureId.Lobby;

        public override void OnEnter()
        {
            // 打开大厅界面。
        }

        public override void OnLeave()
        {
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (LoginSystem.Instance.LoginState.Value < LoginStateType.OnGame)
            {
                ChangeProcedure((int)ProcedureId.Login);
            }
        }
    }
}
