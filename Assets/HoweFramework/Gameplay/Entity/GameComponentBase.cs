namespace HoweFramework
{
    /// <summary>
    /// 游戏实体组件抽象类。
    /// </summary>
    public abstract class GameComponentBase : IGameComponent, IReference
    {
        public int ReferenceId { get; set; }

        public int ComponentType => GameEntityHelper.GetComponentType(GetType());

        public int ComponentId { get; private set; }

        public IGameEntity Entity { get; private set; }

        public IGameContext Context { get; private set; }

        public void Awake(int componentId, IGameEntity entity)
        {
            ComponentId = componentId;
            Entity = entity;
            Context = entity.Context;

            OnAwake();
        }

        public void Dispose()
        {
            if (Entity != null)
            {
                Entity.RemoveComponent(ComponentType);
            }
            else
            {
                DisposeFromEntity();
            }
        }

        public void DisposeFromEntity()
        {
            OnDispose();

            ComponentId = 0;
            Entity = null;
            Context = null;
        }

        public void Clear()
        {
            // do nothing.
        }

        protected abstract void OnAwake();
        protected abstract void OnDispose();
    }
}

