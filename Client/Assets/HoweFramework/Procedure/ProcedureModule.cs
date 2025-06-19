using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 流程模块。
    /// </summary>
    public sealed class ProcedureModule : ModuleBase<ProcedureModule>
    {
        /// <summary>
        /// 当前的流程id。
        /// </summary>
        public int ProcedureId => Procedure?.Id ?? 0;

        /// <summary>
        /// 当前的流程实例。
        /// </summary>
        public ProcedureBase Procedure { get; private set; }

        /// <summary>
        /// 流程字典。
        /// </summary>
        private readonly Dictionary<int, ProcedureBase> m_ProcedureDict = new();

        /// <summary>
        /// 启动流程状态机。
        /// </summary>
        /// <param name="procedureId">初始流程id。</param>
        /// <param name="procedures">流程列表。</param>
        public void Launch(int procedureId, params ProcedureBase[] procedures)
        {
            if (Procedure != null)
            {
                throw new ErrorCodeException(ErrorCode.ProcedureAlreadyLaunch);
            }

            m_ProcedureDict.Clear();
            foreach (var procedure in procedures)
            {
                m_ProcedureDict.Add(procedure.Id, procedure);
            }

            if (!m_ProcedureDict.TryGetValue(procedureId, out var launchProcedure))
            {
                throw new ErrorCodeException(ErrorCode.ProcedureNotExist);
            }

            Procedure = launchProcedure;
            Procedure.Enter();
        }

        /// <summary>
        /// 停止流程状态机。
        /// </summary>
        public void Stop()
        {
            if (Procedure == null)
            {
                return;
            }

            Procedure.Leave();
            Procedure = null;
        }

        /// <summary>
        /// 切换流程，应从流程实例类中调用。
        /// </summary>
        /// <param name="procedureId">流程id。</param>
        internal void ChangeProcedure(int procedureId)
        {
            if (Procedure == null)
            {
                throw new ErrorCodeException(ErrorCode.ProcedureNotRunning);
            }

            if (!m_ProcedureDict.TryGetValue(procedureId, out var newProcedure))
            {
                throw new ErrorCodeException(ErrorCode.ProcedureNotExist);
            }

            Procedure.Leave();
            Procedure = newProcedure;
            Procedure.Enter();
        }

        protected override void OnInit()
        {
        }

        protected override void OnDestroy()
        {
            if (Procedure != null)
            {
                Procedure.Leave();
                Procedure = null;
            }
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (Procedure == null)
            {
                return;
            }

            Procedure.Update(elapseSeconds, realElapseSeconds);
        }
    }
}
