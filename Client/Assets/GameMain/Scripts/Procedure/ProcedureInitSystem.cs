using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 初始化游戏系统流程。
    /// </summary>
    public sealed class ProcedureInitSystem : ProcedureBase
    {
        public override int Id => (int)ProcedureId.InitSystem;

        public override void OnEnter()
        {
            SystemModule.Instance.RegisterSystem<ILoginSystem, OfflineLoginSystem>();
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

