namespace GameMain
{
    /// <summary>
    /// 移动方向。
    /// </summary>
    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right,
    }

    /// <summary>
    /// 移动方向扩展。
    /// </summary>
    public static class MoveDirectionExtensions
    {
        /// <summary>
        /// 获取反向。
        /// </summary>
        /// <param name="direction">移动方向。</param>
        /// <returns>反向。</returns>
        public static MoveDirection Opposite(this MoveDirection direction)
        {
            return direction switch
            {
                MoveDirection.Up => MoveDirection.Down,
                MoveDirection.Down => MoveDirection.Up,
                MoveDirection.Left => MoveDirection.Right,
                MoveDirection.Right => MoveDirection.Left,
                _ => direction
            };
        }
    }
}