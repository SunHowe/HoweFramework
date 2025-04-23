namespace HoweFramework
{
    /// <summary>
    /// 错误码。
    /// </summary>
    public static class ErrorCode
    {
        /// <summary>
        /// 成功。
        /// </summary>
        public const int Success = 0;

        /// <summary>
        /// 触发异常。
        /// </summary>
        public const int Exception = 1;

        /// <summary>
        /// 响应包类型不匹配。
        /// </summary>
        public const int ResponseTypeMismatch = 2;

        /// <summary>
        /// 框架异常。
        /// </summary>
        public const int FrameworkException = 3;

        /// <summary>
        /// 请求已取消。
        /// </summary>
        public const int RequestCanceled = 4;

        /// <summary>
        /// 界面未打开。
        /// </summary>
        public const int UIFormNotOpen = 100;

        /// <summary>
        /// 界面辅助器未设置。
        /// </summary>
        public const int UIFormHelperNotSet = 101;

        /// <summary>
        /// UI界面分组辅助器未设置。
        /// </summary>
        public const int UIFormGroupHelperNotSet = 102;

        /// <summary>
        /// UI界面分组已存在。
        /// </summary>
        public const int UIFormGroupAlreadyExists = 103;

        /// <summary>
        /// UI界面分组不存在。
        /// </summary>
        public const int UIFormGroupNotExist = 104;

        /// <summary>
        /// 界面收到新的打开请求。
        /// </summary>
        public const int UIFormNewOpenRequest = 105;

        /// <summary>
        /// 界面正在销毁。
        /// </summary>
        public const int UIFormWhileDestroying = 106;

        /// <summary>
        /// 界面逻辑未找到。
        /// </summary>
        public const int UIFormLogicNotFound = 107;

        /// <summary>
        /// UI包映射文件未找到。
        /// </summary>
        public const int UIPackageMappingNotFound = 108;

        /// <summary>
        /// UI包未找到。
        /// </summary>
        public const int UIPackageNotFound = 109;

        /// <summary>
        /// 界面绑定信息未找到。
        /// </summary>
        public const int UIFormBindingNotFound = 110;

        /// <summary>
        /// 界面实例化失败。
        /// </summary>
        public const int UIFormInstantiateFailed = 111;

        /// <summary>
        /// 界面被关闭。
        /// </summary>
        public const int UIFormClosed = 112;

        /// <summary>
        /// 资源核心加载器未设置。
        /// </summary>
        public const int ResCoreLoaderNotSet = 201;

        /// <summary>
        /// 资源加载器已销毁。
        /// </summary>
        public const int ResLoaderDisposed = 202;
    }
}
