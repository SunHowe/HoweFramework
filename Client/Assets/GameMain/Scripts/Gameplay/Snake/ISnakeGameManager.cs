using HoweFramework;
using UnityEngine;

namespace GameMain
{
    /// <summary>
    /// 贪吃蛇游戏管理器。
    /// </summary>
    [GameManager(GameManagerType.Snake)]
    public interface ISnakeGameManager : IGameManager
    {
        /// <summary>
        /// 分数。
        /// </summary>
        Bindable<int> Score { get; }

        /// <summary>
        /// 移动方向。
        /// </summary>
        Bindable<MoveDirection> MoveDirection { get; }

        /// <summary>
        /// 移动速度。
        /// </summary>
        Bindable<float> MoveSpeed { get; }

        /// <summary>
        /// 初始节点数量。
        /// </summary>
        int InitialNodeCount { get; }

        /// <summary>
        /// 提交移动方向。
        /// </summary>
        void SubmitMoveDirection(MoveDirection direction);
    }

    public sealed class SnakeGameManager : GameManagerBase, ISnakeGameManager
    {
        public Bindable<int> Score { get; } = new();    

        public Bindable<MoveDirection> MoveDirection { get; } = new();

        public Bindable<float> MoveSpeed { get; } = new();

        public int InitialNodeCount { get; } = 3;

        public void SubmitMoveDirection(MoveDirection direction)
        {
            if (MoveDirection.Value == direction)
            {
                return;
            }

            // 不允许直接反向。
            if (MoveDirection.Value == direction.Opposite())
            {
                return;
            }

            MoveDirection.Value = direction;
        }

        protected override void OnAwake()
        {
            Score.Value = 0;
            MoveDirection.Value = GameMain.MoveDirection.Up;
            MoveSpeed.Value = 5f;

            var entityManager = Context.GetManager<IGameEntityManager>();
            var entity = entityManager.CreateEntity();
            var snakeComponent = entity.AddComponent<SnakeComponent>();
            snakeComponent.InitSnake(InitialNodeCount, new Vector2(0, 0), MoveDirection.Value);
        }

        protected override void OnDispose()
        {
        }
    }
}