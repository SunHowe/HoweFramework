using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 初始化游戏系统流程。
    /// </summary>
    public sealed class ProcedureInitSystem : ProcedureBase
    {
        public override int Id => (int)ProcedureId.InitSystem;

        protected override void OnEnter()
        {
            SystemModule.Instance.RegisterSystem<ILoginSystem, OfflineLoginSystem>();
        }

        protected override void OnLeave()
        {
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            ChangeNextProcedure();
        }
    }
}

