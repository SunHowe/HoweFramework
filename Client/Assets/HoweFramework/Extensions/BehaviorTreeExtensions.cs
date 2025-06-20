using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 行为树扩展。
    /// </summary>
    public static class BehaviorTreeExtensions
    {
        /// <summary>
        /// 配置行为树节点。
        /// </summary>
        /// <typeparam name="T">行为树节点类型。</typeparam>
        /// <param name="node">行为树节点。</param>
        /// <param name="configure">配置行为树节点。</param>
        /// <returns>返回行为树节点。</returns>
        public static T Configure<T>(this T node, Action<T> configure) where T : IBehaviorNode
        {
            configure?.Invoke(node);
            return node;
        }

        /// <summary>
        /// 通过协程每帧执行行为树，直到行为树返回成功或失败。
        /// </summary>
        public static async UniTask<int> ExecuteEveryFrame(this BehaviorRoot root, CancellationToken token = default, bool disposeAfterFinished = false)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var result = root.Execute();
                    if (result != ErrorCode.BehaviorRunningState)
                    {
                        return result;
                    }

                    await UniTask.NextFrame(token).SuppressCancellationThrow();
                }

                return ErrorCode.RequestCanceled;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return ErrorCode.Exception;
            }
            finally
            {
                if (disposeAfterFinished)
                {
                    root.Dispose();
                }
            }
        }
    }
}