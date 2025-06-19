using System.Collections.Generic;

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
        /// 流程控制器列表。
        /// </summary>
        private readonly List<IProcedureController> m_ProcedureControllers = new();

        /// <summary>
        /// 进入流程时调用。
        /// </summary>
        public void Enter()
        {
            OnEnter();

            foreach (var controller in m_ProcedureControllers)
            {
                controller.Initialize();
            }
        }

        /// <summary>
        /// 离开流程时调用。
        /// </summary>
        public void Leave()
        {
            foreach (var controller in m_ProcedureControllers)
            {
                controller.Dispose();
            }

            m_ProcedureControllers.Clear();

            OnLeave();
        }

        /// <summary>
        /// 轮询时调用。
        /// </summary>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            OnUpdate(elapseSeconds, realElapseSeconds);
        }

        /// <summary>
        /// 添加流程控制器。
        /// </summary>
        /// <param name="controller">流程控制器。</param>
        protected void AddController(IProcedureController controller)
        {
            m_ProcedureControllers.Add(controller);
        }

        /// <summary>
        /// 添加流程控制器。
        /// </summary>
        /// <typeparam name="T">流程控制器类型。</typeparam>
        protected void AddController<T>() where T : IProcedureController, new()
        {
            m_ProcedureControllers.Add(new T());
        }

        /// <summary>
        /// 进入流程时调用。
        /// </summary>
        protected abstract void OnEnter();

        /// <summary>
        /// 离开流程时调用。
        /// </summary>
        protected abstract void OnLeave();
        
        /// <summary>
        /// 轮询时调用。
        /// </summary>
        protected abstract void OnUpdate(float elapseSeconds, float realElapseSeconds);

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
