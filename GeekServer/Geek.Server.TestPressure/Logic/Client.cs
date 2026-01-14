using Geek.Server.Core.Net;
using Geek.Server.Core.Net.Tcp;
using Geek.Server.Core.Net.Websocket;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace Geek.Server.TestPressure.Logic
{
    public enum ServerErrorCode
    {
        Success = 0,
        ConfigErr = 400, //配置表错误
        ParamErr, //客户端传递参数错误
        CostNotEnough, //消耗不足

        Notice = 100000, //正常通知
        FuncNotOpen, //功能未开启，主消息屏蔽
        Other //其他
    }

    public class Client
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        long id;
        RequestWaiter m_Waiter = new();
        NetChannel netChannel;
        int msgUniId = 200;
        
        Dictionary<int, Action<Message>> m_MsgHandlers = new();

        public Client(long id)
        {
            this.id = id;
            AddMsgHandler<UserInfo>(OnReceiveUserInfo);
            AddMsgHandler<UserBagInfo>(OnReceiveUserBagInfo);
        }

        /// <summary>
        /// 收到玩家信息推送。
        /// </summary>
        private void OnReceiveUserInfo(UserInfo obj)
        {
            Log.Info($"{id} 收到玩家信息推送:{JsonConvert.SerializeObject(obj)}");
        }

        /// <summary>
        /// 收到背包信息推送。
        /// </summary>
        private void OnReceiveUserBagInfo(UserBagInfo obj)
        {
            Log.Info($"{id} 收到背包信息推送:{JsonConvert.SerializeObject(obj)}");
        }

        public async void Start()
        {
            if (TestSettings.Ins.useWebSocket)
            {
                var ws = new ClientWebSocket();
                await ws.ConnectAsync(new Uri(TestSettings.Ins.webSocketServerUrl), CancellationToken.None);

                if (ws.State == WebSocketState.Open)
                {
                    Log.Info($"Connected to {TestSettings.Ins.webSocketServerUrl}");
                    netChannel = new WebSocketChannel(ws, TestSettings.Ins.webSocketServerUrl, OnRevice);
                    _ = netChannel.StartAsync();
                }
                else
                {
                    Log.Error($"连接服务器失败...");
                    return;
                }
            }
            else
            {
                var socket = new TcpClient(AddressFamily.InterNetwork);
                try
                {
                    socket.NoDelay = true;
                    await socket.ConnectAsync(TestSettings.Ins.serverIp, TestSettings.Ins.serverPort);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    return;
                }

                netChannel = new ClientTcpChannel(socket, OnRevice);
                _ = netChannel.StartAsync();
            }

            await ReqLogin();
            await ReqComposePet();
        }

        private async Task ReqLogin()
        {
            //登陆
            var req = new LoginReq();
            req.SdkType = 0;
            req.SdkToken = "555";
            req.UserName = "name" + id;
            req.Device = new Random().NextInt64().ToString();
            req.Platform = "android";
            var resp = await SendMsgAsync<LoginResp>(req);
            Log.Info($"{id} 登陆成功:{JsonConvert.SerializeObject(resp)}");
        }

        private async Task ReqComposePet()
        {
            var req = new BagComposePetReq();
            req.FragmentId = 103;
            var resp = await SendMsgAsync<BagComposePetResp>(req);
            Log.Info($"{id} 合成宠物成功:{JsonConvert.SerializeObject(resp)}");
        }
         
        private Task<ResponseMessage> SendMsgAsync(Message msg)
        {
            msg.UniId = (int)id*10000 +  msgUniId++;
            Log.Info($"{id} 发送消息:{JsonConvert.SerializeObject(msg)}");
            var awaiter = m_Waiter.CreateWait(msg.UniId);
            netChannel.Write(msg);
            return awaiter;
        }

        private async Task<T> SendMsgAsync<T>(Message msg) where T : ResponseMessage
        {
            var response = await SendMsgAsync(msg);
            return (T)response;
        }

        /// <summary>
        /// 添加消息处理函数(目前只支持单个)
        /// </summary>
        private void AddMsgHandler(int msgId, Action<Message> handler)
        {
            m_MsgHandlers[msgId] = handler;
        }

        /// <summary>
        /// 添加消息处理函数(目前只支持单个)
        /// </summary>
        private void AddMsgHandler<T>(Action<T> handler) where T : Message
        {
            var msgId = MsgFactory.GetMsgId(typeof(T));
            AddMsgHandler(msgId, msg => handler((T)msg));
        }

        private void OnRevice(Message msg)
        {
            Log.Info($"收到消息:{msg.MsgId} {MsgFactory.GetType(msg.MsgId)}"); 

            if (msg is ResponseMessage resp)
            {
                m_Waiter.SetResponse(resp.UniId, resp);
            }
            else if (m_MsgHandlers.TryGetValue(msg.MsgId, out var handler))
            {
                handler(msg);
            }
            else
            {
                Log.Warn($"未处理的消息:{msg.MsgId} {MsgFactory.GetType(msg.MsgId)}");
            }
        }
    }
}
