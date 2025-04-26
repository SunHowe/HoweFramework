using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 异步请求基类。
    /// </summary>
    public abstract class RequestBase : IRequest, IReference
    {
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
        public async UniTask<IResponse> Execute(CancellationToken token = default)
        {
            try
            {
                if (token.IsCancellationRequested)
                {
                    return CommonResponse.Create(ErrorCode.RequestCanceled);
                }

                return await OnExecute(token);
            }
            catch (ErrorCodeException e)
            {
                Log.Error($"Catch exception: ErrorCode={e.ErrorCode}, Message={e.Message}\n{e.StackTrace}");
                return CommonResponse.Create(e.ErrorCode);
            }
            catch (Exception e)
            {
                Log.Error($"Catch exception: {e.Message}\n{e.StackTrace}");
                return CommonResponse.Create(ErrorCode.Exception);
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
        protected abstract UniTask<IResponse> OnExecute(CancellationToken token);
    }
}
