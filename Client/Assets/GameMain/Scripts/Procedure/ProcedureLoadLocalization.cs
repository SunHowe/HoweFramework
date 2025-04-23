using Cysharp.Threading.Tasks;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 加载本地化流程。
    /// </summary>
    public sealed class ProcedureLoadLocalization : ProcedureBase
    {
        public override int Id => (int)ProcedureId.LoadLocalization;

        private bool m_IsComplete;

        public override void OnEnter()
        {
            m_IsComplete = false;

            // 添加本地化数据源。
            LocalizationModule.Instance.AddSource(new LubanLocalizationSource());

            // 异步加载本地化数据。
            LocalizationModule.Instance.LoadAsync().ContinueWith(() => m_IsComplete = true).Forget();
        }

        public override void OnLeave()
        {
        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (m_IsComplete)
            {
                ChangeNextProcedure();
            }
        }
    }
}
