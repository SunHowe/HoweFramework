using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 行为树加载器接口。
    /// </summary>
    public interface IBehaviorLoader : IDisposable
    {
        /// <summary>
        /// 从配置文件加载行为树。
        /// </summary>
        /// <param name="assetKey">资源Key。</param>
        /// <param name="token">取消令牌。</param>
        /// <returns>行为树。</returns>
        UniTask<BehaviorRoot> LoadBehaviorTree(string assetKey, CancellationToken token = default);
    }
}