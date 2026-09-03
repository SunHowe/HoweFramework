namespace GameMain
{
    /// <summary>
    /// 技能的一个异步步骤。由 Manager 每逻辑帧推进，全部完成后技能自动结束。
    /// </summary>
    public abstract class AbilityTask
    {
        public bool IsComplete { get; protected set; }

        public bool IsCancelled { get; protected set; }

        public bool IsStarted { get; private set; }

        public virtual void OnStart(in AbilityActivationContext context)
        {
        }

        public virtual void Tick(in AbilityActivationContext context, float deltaTime)
        {
        }

        public virtual void OnEnd(in AbilityActivationContext context, bool cancelled)
        {
        }

        internal void StartOnce(in AbilityActivationContext context)
        {
            if (IsStarted)
            {
                return;
            }

            IsStarted = true;
            OnStart(in context);
        }

        protected void Complete()
        {
            IsComplete = true;
        }

        protected void Cancel()
        {
            IsCancelled = true;
            IsComplete = true;
        }
    }
}
