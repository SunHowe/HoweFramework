using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 游戏流程。
    /// </summary>
    public sealed class ProcedureGame : ProcedureBase
    {
        public override int Id => (int)ProcedureId.Game;

        protected override void OnEnter()
        {
        }

        protected override void OnLeave()
        {
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}