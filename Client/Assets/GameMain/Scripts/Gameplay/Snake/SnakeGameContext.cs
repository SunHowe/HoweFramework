using System.Threading;
using Cysharp.Threading.Tasks;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 贪吃蛇游戏上下文。
    /// </summary>
    public sealed class SnakeGameContext : GameContextBase, ILoadable
    {
        /// <summary>
        /// 随机数种子。
        /// </summary>
        private int m_Seed;

        /// <summary>
        /// 地图id。
        /// </summary>
        private int m_MapId;

        protected override void OnAwake()
        {
        }

        protected override void OnDispose()
        {
        }
        
        protected override void OnAfterAwake()
        {
        }

        /// <summary>
        /// 创建贪吃蛇游戏上下文。
        /// </summary>
        /// <param name="mapId">地图id。</param>
        /// <param name="seed">随机种子。</param>
        /// <returns>贪吃蛇游戏上下文。</returns>
        public static SnakeGameContext Create(int mapId, int seed)
        {
            var context = new SnakeGameContext();
            context.m_MapId = mapId;
            context.m_Seed = seed;
            return context;
        }

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="onProgress">加载进度回调。</param>
        /// <param name="cancellationToken">取消令牌。</param>
        /// <returns>异步加载任务。</returns>
        public async UniTask LoadAsync(LoadableProgress onProgress = null, CancellationToken cancellationToken = default)
        {
            UseManagedResLoader();
            UseManagedGameObjectPool();

            using var group = LoadableGroup.Create();

            // 预加载地图。
            group.AddLoadable(SceneModule.Instance.CreateLoadSceneTask(SnakeGameAssetHelper.GetScenePath($"Map_{m_MapId}")), 10);

            // 预加载蛇节点。
            group.AddLoadable(GameObjectPool.CreatePreloadTask(SnakeGameAssetHelper.GetPrefabPath("SnakeNode"), 20), 5);
            
            // 预加载食物。
            group.AddLoadable(GameObjectPool.CreatePreloadTask(SnakeGameAssetHelper.GetPrefabPath("Food"), 1));

            await group.LoadAsync(onProgress, cancellationToken);
        }
    }
}