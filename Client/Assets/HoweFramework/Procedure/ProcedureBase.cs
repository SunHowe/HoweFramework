namespace HoweFramework
{
    /// <summary>
    /// 流程基类。
    /// </summary>
    public abstract class ProcedureBase
    {
        /// <summary>
        /// 流程id。
        /// </summary>
        public abstract int Id { get; }

        /// <summary>
        /// 进入流程时调用。
        /// </summary>
        public abstract void OnEnter();

        /// <summary>
        /// 离开流程时调用。
        /// </summary>
        public abstract void OnLeave();
        
        /// <summary>
        /// 轮询时调用。
        /// </summary>
        public abstract void OnUpdate(float elapseSeconds, float realElapseSeconds);

        /// <summary>
        /// 切换流程。
        /// </summary>
        /// <param name="procedureId">流程id。</param>
        protected void ChangeProcedure(int procedureId)
        {
            ProcedureModule.Instance.ChangeProcedure(procedureId);
        }

        /// <summary>
        /// 切换到下一个流程(依赖于Id的顺序)。
        /// </summary>
        protected void ChangeNextProcedure() => ChangeProcedure(Id + 1);
    }
}
