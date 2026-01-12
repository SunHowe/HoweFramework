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

        public override sealed async Task ActionAsync()
        {
            var req = Msg as TReq;
            var resp = new TResp();

            try
            {
                await OnHandleRequest(req, resp);
            }
            catch (Exception e)
            {
                LOGGER.Error($"{GetType().Name} Handle Request Error: {e.Message}");
                resp.ErrorCode = (int)ServerErrorCode.InternalError;
                resp.Desc = e.Message;
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
    public abstract class GlobalCompHandler<T, TReq, TResp> : GlobalCompHandler<T> where T : ICompAgent where TReq : Message where TResp : ResponseMessage, new()
    {
        protected readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        public override sealed async Task ActionAsync()
        {
            var req = Msg as TReq;
            var resp = new TResp();

            try
            {
                await OnHandleRequest(req, resp);
            }
            catch (Exception e)
            {
                LOGGER.Error($"{GetType().Name} Handle Request Error: {e.Message}");
                resp.ErrorCode = (int)ServerErrorCode.InternalError;
                resp.Desc = e.Message;
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
    public abstract class GlobalCompHandler<T, TReq> : GlobalCompHandler<T, TReq, CommonResp> where T : ICompAgent where TReq : Message
    {
    }
}