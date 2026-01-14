using MessagePack; 

namespace ClientProto
{ 

    [MessagePackObject(true)]
    public class NetConnectMessage : Message
    { 
    }

    [MessagePackObject(true)]
    public class NetDisConnectMessage : Message
    { 
    } 

    /// <summary>
    /// 双向心跳/收到恢复同样的消息
    /// </summary>
    [MessagePackObject(true)]
    public class HearBeat : Message
    {
        /// <summary>
        /// 当前时间
        /// </summary>
        public long TimeTick { get; set; }
    }
}
