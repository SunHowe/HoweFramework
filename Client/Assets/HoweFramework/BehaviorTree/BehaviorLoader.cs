using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 行为树加载器。
    /// </summary>
    internal sealed class BehaviorLoader : IBehaviorLoader, IReference
    {
        /// <summary>
        /// 资源加载器。
        /// </summary>
        private IResLoader m_ResLoader;

        /// <summary>
        /// 行为树配置字典。
        /// </summary>
        private readonly Dictionary<string, BehaviorTreeConfig> m_BehaviorConfigDict = new();

        /// <summary>
        /// 创建行为树加载器。
        /// </summary>
        /// <param name="resLoader">资源加载器。若为null，则会分配一个新的资源加载器。</param>
        /// <returns>行为树加载器。</returns>
        public static BehaviorLoader Create(IResLoader resLoader = null)
        {
            var loader = ReferencePool.Acquire<BehaviorLoader>();
            loader.m_ResLoader = ResModule.Instance.CreateResLoader(resLoader);
            return loader;
        }

        public async UniTask<BehaviorRoot> LoadBehaviorTree(string assetKey, CancellationToken token = default)
        {
            if (m_BehaviorConfigDict.TryGetValue(assetKey, out var behaviorConfig))
            {
                return behaviorConfig.CreateBehaviorTree();
            }

            var bytes = await m_ResLoader.LoadBinaryAsync(assetKey, token);
            if (token.IsCancellationRequested)
            {
                return null;
            }

            if (bytes == null)
            {
                return null;
            }

            // 再次检测是否已加载。
            if (m_BehaviorConfigDict.TryGetValue(assetKey, out behaviorConfig))
            {
                return behaviorConfig.CreateBehaviorTree();
            }

            behaviorConfig = BehaviorTreeConfig.Create();
            using var reader = DefaultBufferReader.Create(bytes);
            behaviorConfig.Deserialize(reader);
            m_BehaviorConfigDict[assetKey] = behaviorConfig;
            return behaviorConfig.CreateBehaviorTree();
        }

        public void Clear()
        {
            foreach (var behaviorConfig in m_BehaviorConfigDict.Values)
            {
                behaviorConfig.Dispose();
            }
            
            m_BehaviorConfigDict.Clear();
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }
    }
}