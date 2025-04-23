using System;
using Cysharp.Threading.Tasks;

namespace HoweFramework
{
    /// <summary>
    /// 异步请求扩展。
    /// </summary>
    public static class RequestExtensions
    {
        /// <summary>
        /// 获取错误码。
        /// </summary>
        /// <param name="task">异步请求任务。</param>
        /// <returns>错误码。</returns>
        public static async UniTask<int> GetErrorCode(this UniTask<ResponseBase> task)
        {
            var response = await task;
            var errorCode = response.ErrorCode;
            response.Dispose();
            return errorCode;
        }

        /// <summary>
        /// 忽略异步请求结果。
        /// </summary>
        /// <param name="task">异步请求任务。</param>
        public static void Forget(this UniTask<ResponseBase> task)
        {
            task.GetErrorCode().Forget();
        }

        /// <summary>
        /// 将应答包转换为指定类型。
        /// </summary>
        /// <typeparam name="T">指定类型。</typeparam>
        /// <param name="task">异步请求任务。</param>
        /// <returns>指定类型。</returns>
        public static async UniTask<T> As<T>(this UniTask<ResponseBase> task) where T : ResponseBase, new()
        {
            var raw = await task;
            if (raw is T response)
            {
                // 符合目标类型。
                return response;
            }
            else if (raw.ErrorCode != 0)
            {
                // 不符合目标类型时，若返回的是失败，则创建对应类型的实例返回。
                response = ReferencePool.Acquire<T>();
                response.ErrorCode = raw.ErrorCode;
                raw.Dispose();
                return response;
            }
            else if (typeof(T) == typeof(CommonResponse))
            {
                // 不符合目标类型时，若期望的是通用响应包，则创建通用响应包实例返回，并将原始响应包作为业务透传数据。
                return CommonResponse.Create(raw.ErrorCode, raw) as T;
            }
            else
            {
                // 不符合目标类型时，若返回的是成功，则返回类型不匹配的错误。
                response = ReferencePool.Acquire<T>();
                response.ErrorCode = ErrorCode.ResponseTypeMismatch;
                raw.Dispose();
                return response;
            }
        }
    }
}
