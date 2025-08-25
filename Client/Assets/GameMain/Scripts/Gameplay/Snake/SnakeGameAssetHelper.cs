namespace GameMain
{
    /// <summary>
    /// 贪吃蛇游戏资源辅助器。
    /// </summary>
    public static class SnakeGameAssetHelper
    {
        /// <summary>
        /// 获取预制体路径。
        /// </summary>
        /// <param name="assetKey">资源键值。</param>
        /// <returns>预制体路径。</returns>
        public static string GetPrefabPath(string assetKey)
        {
            return $"Assets/GameMain/Prefab/{assetKey}.prefab";
        }

        /// <summary>
        /// 获取场景路径。
        /// </summary>
        /// <param name="assetKey">资源键值。</param>
        /// <returns>场景路径。</returns>
        public static string GetScenePath(string assetKey)
        {
            return $"Assets/GameMain/Scene/{assetKey}.unity";
        }
    }
}