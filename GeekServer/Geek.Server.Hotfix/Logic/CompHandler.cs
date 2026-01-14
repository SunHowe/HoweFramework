using Geek.Server.Core.Hotfix.Agent;
using Geek.Server.Core.Net.BaseHandler;

namespace Server.Logic
{
    /// <summary>
    /// 封装了请求处理流程的请求协议处理器基类。
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <typeparam name="TReq">请求类型</typeparam>
    /// <typeparam name="TResp">响应类型</typeparam>
    public abstract class RoleCompReqHandler<T, TReq, TResp> : RoleCompHandler<T> where T : ICompAgent where TReq : Message where TResp : ResponseMessage, new()
    {
        protected readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        public sealed override async Task ActionAsync()
        {
            var req = (TReq)Msg;
            var resp = new TResp();

            try
            {
                await OnHandleRequest(req, resp);
            }
            catch (ServerErrorCodeException e)
            {
                LOGGER.Warn($"Request {req.MsgId} Response: {resp.ErrorCode} {e.Message}");
                resp.ErrorCode = (int)e.ErrorCode;
            }
            catch (Exception e)
            {
                LOGGER.Error($"{GetType().Name} Handle Request Error: {e.Message}");
                resp.ErrorCode = (int)ServerErrorCode.InternalError;
            }

            resp.UniId = req.UniId;
            Channel.Write(resp);
        }

        protected abstract Task OnHandleRequest(TReq req, TResp resp);
    }

    /// <summary>
    /// 使用通用响应包的请求处理器。
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <typeparam name="TReq">请求类型</typeparam>
    public abstract class RoleCompReqHandler<T, TReq> : RoleCompReqHandler<T, TReq, CommonResp> where T : ICompAgent where TReq : Message
    {
    }

    /// <summary>
    /// 封装了请求处理流程的请求协议处理器基类。
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <typeparam name="TReq">请求类型</typeparam>
    /// <typeparam name="TResp">响应类型</typeparam>
    public abstract class GlobalCompReqHandler<T, TReq, TResp> : GlobalCompHandler<T> where T : ICompAgent where TReq : Message where TResp : ResponseMessage, new()
    {
        protected readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        public sealed override async Task ActionAsync()
        {
            var req = (TReq)Msg;
            var resp = new TResp();

            try
            {
                await OnHandleRequest(req, resp);
            }
            catch (ServerErrorCodeException e)
            {
                LOGGER.Warn($"Request {req.MsgId} Response: {resp.ErrorCode} {e.Message}");
                resp.ErrorCode = (int)e.ErrorCode;
            }
            catch (Exception e)
            {
                LOGGER.Error($"{GetType().Name} Handle Request Error: {e.Message}");
                resp.ErrorCode = (int)ServerErrorCode.InternalError;
            }

            resp.UniId = req.UniId;
            Channel.Write(resp);
        }

        protected abstract Task OnHandleRequest(TReq req, TResp resp);
    }

    /// <summary>
    /// 使用通用响应包的请求处理器。
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <typeparam name="TReq">请求类型</typeparam>
    public abstract class GlobalCompReqHandler<T, TReq> : GlobalCompReqHandler<T, TReq, CommonResp> where T : ICompAgent where TReq : Message
    {
    }
}