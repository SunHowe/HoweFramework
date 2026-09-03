using System.Collections.Generic;

namespace GameMain
{
    /// <summary>
    /// 由 Task 序列驱动的技能。全部完成后自动结束；取消会向所有 Task 传播 OnEnd。
    /// </summary>
    public abstract class TaskGameplayAbility : IGameplayAbility
    {
        public void Activate(in AbilityActivationContext context)
        {
            var tasks = BuildTasks(in context);
            if (tasks == null || tasks.Count == 0)
            {
                context.EndAbility(false);
                return;
            }

            context.Manager.AddAbilityTasks(context.Owner, context.ActiveHandle, tasks);
            OnTasksStarted(in context);
        }

        public void OnEnd(in AbilityActivationContext context, bool wasCancelled)
        {
            OnAbilityEnded(in context, wasCancelled);
        }

        protected abstract IReadOnlyList<AbilityTask> BuildTasks(in AbilityActivationContext context);

        protected virtual void OnTasksStarted(in AbilityActivationContext context)
        {
        }

        protected virtual void OnAbilityEnded(in AbilityActivationContext context, bool wasCancelled)
        {
        }
    }
}
