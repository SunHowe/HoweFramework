using UnityEngine;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 贪吃蛇节点组件。
    /// </summary>
    [GameComponent(GameComponentType.SnakeNode)]
    public sealed class SnakeNodeComponent : GameComponentBase
    {
        /// <summary>
        /// 位置。
        /// </summary>
        public Vector2 Position 
        {
            get => m_Position;
            set
            {
                PrevPosition = Position;
                m_Position = value;
                TransformComponent.Position = new Vector3(m_Position.x, 0f, m_Position.y);
            }
        }

        /// <summary>
        /// 上一次位置。
        /// </summary>
        public Vector2 PrevPosition { get; private set; }

        /// <summary>
        /// 方向。
        /// </summary>
        public MoveDirection Direction 
        {
            get => m_Direction;
            set
            {
                PrevDirection = Direction;
                m_Direction = value;
                TransformComponent.EulerAngles = value switch
                {
                    MoveDirection.Up => new Vector3(0, 90, 0),
                    MoveDirection.Down => new Vector3(0, -90, 0),
                    MoveDirection.Left => new Vector3(0, 180, 0),
                    MoveDirection.Right => new Vector3(0, 0, 0),
                    _ => Vector3.zero
                };
            }
        }

        /// <summary>
        /// 上一次方向。
        /// </summary>
        public MoveDirection PrevDirection { get; private set; }

        /// <summary>
        /// Transform组件。
        /// </summary>
        public TransformComponent TransformComponent { get; private set; }

        /// <summary>
        /// View组件。
        /// </summary>
        public ViewComponent ViewComponent { get; private set; }

        /// <summary>
        /// 视图与Transform同步组件。
        /// </summary>
        public ViewTransformSyncComponent ViewTransformSyncComponent { get; private set; }

        private MoveDirection m_Direction;
        private Vector2 m_Position;

        protected override void OnAwake()
        {
            TransformComponent = Entity.AddComponent<TransformComponent>();
            ViewComponent = Entity.AddComponent<ViewComponent>();
            ViewComponent.ResKey = SnakeGameAssetHelper.GetPrefabPath("SnakeNode");
            ViewTransformSyncComponent = Entity.AddComponent<ViewTransformSyncComponent>();
        }

        protected override void OnDispose()
        {
            TransformComponent = null;
            m_Position = Vector2.zero;
            PrevPosition = Vector2.zero;
            m_Direction = MoveDirection.Up;
            PrevDirection = MoveDirection.Up;
        }
    }
}