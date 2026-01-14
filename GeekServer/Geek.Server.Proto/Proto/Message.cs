using MessagePack;

//外部message定义，不要修改此类 
[MessagePackObject(true)]
public class Message
{
    /// <summary>
    /// 消息唯一id
    /// </summary>
    public int UniId { get; set; }
    [IgnoreMember]
    public virtual int MsgId { get; }
}

[MessagePackObject(true)]
public abstract class ResponseMessage : Message
{
    /// <summary>
    /// 错误码。0表示成功，其他表示失败。
    /// </summary>
    public int ErrorCode { get; set; }
}