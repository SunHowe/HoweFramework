using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 异步请求基类。
    /// </summary>
    public abstract class RequestBase : IReference
    {
        /// <summary>
        /// 引用id。
        /// </summary>
        public int ReferenceId { get; set; }

        /// <summary>
        /// 释放。
        /// </summary>
        public virtual void Clear()
        {
        }

        /// <summary>
        /// 执行请求，并返回响应。
        /// </summary>
        /// <returns>响应。</returns>
        public UniTask<ResponseBase> Execute(CancellationToken token = default)
        {
            try
            {
                if (token.IsCancellationRequested)
                {
                    return UniTask.FromResult<ResponseBase>(CommonResponse.Create(ErrorCode.RequestCanceled));
                }

                return OnExecute(token);
            }
            catch (ErrorCodeException e)
            {
                Log.Error($"Catch exception: ErrorCode={e.ErrorCode}, Message={e.Message}\n{e.StackTrace}");
                return UniTask.FromResult<ResponseBase>(CommonResponse.Create(e.ErrorCode));
            }
            catch (Exception e)
            {
                Log.Error($"Catch exception: {e.Message}\n{e.StackTrace}");
                return UniTask.FromResult<ResponseBase>(CommonResponse.Create(ErrorCode.Exception));
            }
            finally
            {
                ReferencePool.Release(this);
            }
        }

        /// <summary>
        /// 执行请求。
        /// </summary>
        /// <returns>响应。</returns>
        protected abstract UniTask<ResponseBase> OnExecute(CancellationToken token);
    }
}
