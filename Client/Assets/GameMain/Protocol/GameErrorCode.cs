namespace Protocol
{
    /// <summary>
    /// 错误码定义.
    /// </summary>
    public static class GameErrorCode
    {
        public const int Success = 0;
        public const int Internal = 1;
        public const int NoHandler = 2;
        public const int ProtocolUnpackError = 3;
        public const int NoLogin = 10;
        public const int RepeatLogin = 11;
    }

}