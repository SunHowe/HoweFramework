namespace ServerProtocol;

/// <summary>
/// 游戏异常类.
/// </summary>
[GenerateSerializer]
public class GameException : Exception
{
    /// <summary>
    /// 错误码
    /// </summary>
    [Id(0)]
    public int ErrorCode { get; set; }
    
    /// <summary>
    /// 错误描述.
    /// </summary>
    [Id(1)]
    public string? ErrorMessage { get; set; }

    public GameException()
    {
    }

    public GameException(int errorCode)
    {
        ErrorCode = errorCode;
    }

    public GameException(int errorCode, string? errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
}