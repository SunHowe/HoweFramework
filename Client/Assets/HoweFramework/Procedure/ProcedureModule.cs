using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 流程模块。
    /// </summary>
    public sealed class ProcedureModule : ModuleBase<ProcedureModule>
    {
        /// <summary>
        /// 当前的流程 id（类型 TypeId）。
        /// </summary>
        public int ProcedureId => Procedure?.Id ?? 0;

        /// <summary>
        /// 当前的流程实例。
        /// </summary>
        public ProcedureBase Procedure { get; private set; }

        /// <summary>
        /// 流程字典（TypeId → 实例）。
        /// </summary>
        private readonly Dictionary<int, ProcedureBase> m_ProcedureDict = new();

        /// <summary>
        /// 流程列表（Launch 顺序，供 ChangeNextProcedure 使用）。
        /// </summary>
        private readonly List<ProcedureBase> m_ProcedureList = new();

        /// <summary>
        /// 启动流程状态机。
        /// </summary>
        /// <typeparam name="T">初始流程类型。</typeparam>
        /// <param name="procedures">流程列表。</param>
        public void Launch<T>(params ProcedureBase[] procedures) where T : ProcedureBase
        {
            Launch(TypeId<T>.Id, procedures);
        }

        /// <summary>
        /// 启动流程状态机。
        /// </summary>
        /// <param name="procedureId">初始流程 id（类型 TypeId）。</param>
        /// <param name="procedures">流程列表。</param>
        public void Launch(int procedureId, params ProcedureBase[] procedures)
        {
            if (Procedure != null)
            {
                throw new ErrorCodeException(FrameworkErrorCode.ProcedureAlreadyLaunch);
            }

            m_ProcedureDict.Clear();
            m_ProcedureList.Clear();
            foreach (var procedure in procedures)
            {
                m_ProcedureDict.Add(procedure.Id, procedure);
                m_ProcedureList.Add(procedure);
            }

            if (!m_ProcedureDict.TryGetValue(procedureId, out var launchProcedure))
            {
                throw new ErrorCodeException(FrameworkErrorCode.ProcedureNotExist);
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
        /// <param name="procedureId">流程 id（类型 TypeId）。</param>
        internal void ChangeProcedure(int procedureId)
        {
            if (Procedure == null)
            {
                throw new ErrorCodeException(FrameworkErrorCode.ProcedureNotRunning);
            }

            if (!m_ProcedureDict.TryGetValue(procedureId, out var newProcedure))
            {
                throw new ErrorCodeException(FrameworkErrorCode.ProcedureNotExist);
            }

            Procedure.Leave();
            Procedure = newProcedure;
            Procedure.Enter();
        }

        /// <summary>
        /// 切换到下一个流程（按 Launch 时的数组顺序）。
        /// </summary>
        internal void ChangeNextProcedure()
        {
            if (Procedure == null)
            {
                throw new ErrorCodeException(FrameworkErrorCode.ProcedureNotRunning);
            }

            var currentIndex = m_ProcedureList.IndexOf(Procedure);
            if (currentIndex < 0 || currentIndex + 1 >= m_ProcedureList.Count)
            {
                throw new ErrorCodeException(FrameworkErrorCode.ProcedureNotExist);
            }

            ChangeProcedure(m_ProcedureList[currentIndex + 1].Id);
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
