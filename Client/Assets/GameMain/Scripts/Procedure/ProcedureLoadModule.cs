using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 加载模块流程。
    /// </summary>
    public sealed class ProcedureLoadModule : ProcedureBase
    {
        public override int Id => (int)ProcedureId.LoadModule;

        public override void OnEnter()
        {
        }

        public override void OnLeave()
        {
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            ChangeNextProcedure();
        }
    }
}

