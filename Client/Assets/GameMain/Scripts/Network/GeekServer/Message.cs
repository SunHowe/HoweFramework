using HoweFramework;
using MessagePack;


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

/// <summary>
/// 响应消息基类。
/// </summary>
[MessagePackObject(true)]
public abstract class ResponseMessage : Message, IResponse
{
    /// <summary>
    /// 错误码。0表示成功，其他表示失败。
    /// </summary>
    public int ErrorCode { get; set; }

    public virtual void Clear()
    {
        ErrorCode = 0;
    }

    public void Dispose()
    {
        ReferencePool.Release(this);
    }
}