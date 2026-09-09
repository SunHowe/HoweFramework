namespace HoweFramework
{
    /// <summary>
    /// 行为节点基类。
    /// </summary>
    public abstract class BehaviorNodeBase : IBehaviorNode, IReference
    {
        /// <summary>
        /// 行为树上下文。
        /// </summary>
        public IBehaviorContext Context { get; private set; }

        /// <summary>
        /// 设置行为树上下文。
        /// </summary>
        /// <param name="context">行为树上下文。</param>
        internal void SetContext(IBehaviorContext context)
        {
            Context = context;
        }

        /// <summary>
        /// 设置属性。
        /// </summary>
        /// <param name="property">属性配置。</param>
        internal protected virtual void SetProperty(BehaviorPropertyConfig property)
        {
            var propertyInfo = GetType().GetProperty(property.Name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (propertyInfo == null)
            {
                // 配置中的属性名与节点类不匹配（如重命名后未同步），告警以便排查。
                Log.Warning($"行为节点 {GetType().Name} 不存在属性 '{property.Name}'，该配置项被忽略。");
                return;
            }

            propertyInfo.SetValue(this, property.Value);
        }

        /// <summary>
        /// 执行行为。
        /// </summary>
        /// <returns>返回执行结果。</returns>
        public abstract int Execute();

        /// <summary>
        /// 重置状态。
        /// </summary>
        public abstract void ResetState();
        
        /// <summary>
        /// 释放。
        /// </summary>
        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        /// <summary>
        /// 清理。
        /// </summary>
        public virtual void Clear()
        {
            Context = null;
        }
    }
}