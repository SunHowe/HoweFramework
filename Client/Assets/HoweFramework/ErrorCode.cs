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
        /// 框架异常。
        /// </summary>
        public const int FrameworkException = 2;

        /// <summary>
        /// 参数无效。
        /// </summary>
        public const int InvalidParam = 3;

        /// <summary>
        /// 操作无效。
        /// </summary>
        public const int InvalidOperationException = 4;

        /// <summary>
        /// 未知错误。
        /// </summary>
        public const int Unknown = 5;

        #region [UI]

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

        #endregion

        #region [Resource]

        /// <summary>
        /// 资源核心加载器未设置。
        /// </summary>
        public const int ResCoreLoaderNotSet = 201;

        /// <summary>
        /// 资源加载器已销毁。
        /// </summary>
        public const int ResLoaderDisposed = 202;

        /// <summary>
        /// 资源未找到。
        /// </summary>
        public const int ResNotFound = 203;

        /// <summary>
        /// 场景已加载。
        /// </summary>
        public const int ResSceneAlreadyLoaded = 204;

        /// <summary>
        /// 场景加载中。
        /// </summary>
        public const int ResSceneLoading = 205;

        /// <summary>
        /// 场景未加载。
        /// </summary>
        public const int ResSceneNotLoad = 206;

        /// <summary>
        /// 场景加载失败。
        /// </summary>
        public const int ResSceneLoadFailed = 207;

        /// <summary>
        /// 场景卸载失败。
        /// </summary>
        public const int ResSceneUnloadFailed = 208;

        /// <summary>
        /// 场景卸载中。
        /// </summary>
        public const int ResSceneUnloading = 209;

        #endregion

        #region [Procedure]

        /// <summary>
        /// 流程状态机已经启动。
        /// </summary>
        public const int ProcedureAlreadyLaunch = 301;

        /// <summary>
        /// 流程不存在。
        /// </summary>
        public const int ProcedureNotExist = 302;

        /// <summary>
        /// 流程状态机未运行。
        /// </summary>
        public const int ProcedureNotRunning = 303;

        #endregion

        #region [Network]   

        /// <summary>
        /// 地址族错误。
        /// </summary>
        public const int NetworkAddressFamilyError = 401;

        /// <summary>
        /// Socket 错误。
        /// </summary>
        public const int NetworkSocketError = 402;

        /// <summary>
        /// 连接错误。
        /// </summary>
        public const int NetworkConnectError = 403;

        /// <summary>
        /// 发送错误。
        /// </summary>
        public const int NetworkSendError = 404;

        /// <summary>
        /// 接收错误。
        /// </summary>
        public const int NetworkReceiveError = 405;

        /// <summary>
        /// 序列化错误。
        /// </summary>
        public const int NetworkSerializeError = 406;

        /// <summary>
        /// 反序列化消息包头错误。
        /// </summary>
        public const int NetworkDeserializePacketHeaderError = 407;

        /// <summary>
        /// 反序列化消息包错误。
        /// </summary>
        public const int NetworkDeserializePacketError = 408;

        /// <summary>
        /// 网络频道已存在。
        /// </summary>
        public const int NetworkChannelAlreadyExists = 409;

        /// <summary>
        /// 不支持的网络服务类型。
        /// </summary>
        public const int NetworkNotSupportServiceType = 410;

        /// <summary>
        /// 网络频道不存在。
        /// </summary>
        public const int NetworkChannelNotExist = 411;

        /// <summary>
        /// 网络协议包不是远程请求。
        /// </summary>
        public const int NetworkPacketRequestNotRemoteRequest = 412;

        /// <summary>
        /// 不支持的网络协议包。
        /// </summary>
        public const int NetworkNotSupportPacket = 413;

        #endregion

        #region [WebRequest]

        public const int WebRequestBadRequest = 501;
        public const int WebRequestUnauthorized = 502;
        public const int WebRequestPaymentRequired = 503;
        public const int WebRequestForbidden = 504;
        public const int WebRequestNotFound = 505;
        public const int WebRequestMethodNotAllowed = 506;
        public const int WebRequestNotAcceptable = 507;
        public const int WebRequestProxyAuthenticationRequired = 508;
        public const int WebRequestRequestTimeout = 509;
        public const int WebRequestConflict = 510;
        public const int WebRequestGone = 511;
        public const int WebRequestLengthRequired = 512;
        public const int WebRequestPreconditionFailed = 513;
        public const int WebRequestRequestEntityTooLarge = 514;
        public const int WebRequestRequestUriTooLong = 515;
        public const int WebRequestUnsupportedMediaType = 516;
        public const int WebRequestRequestedRangeNotSatisfiable = 517;
        public const int WebRequestExpectationFailed = 518;
        public const int WebRequestMisdirectedRequest = 519;
        public const int WebRequestUnprocessableEntity = 520;
        public const int WebRequestLocked = 521;
        public const int WebRequestFailedDependency = 522;
        public const int WebRequestUpgradeRequired = 523;
        public const int WebRequestPreconditionRequired = 524;
        public const int WebRequestTooManyRequests = 525;
        public const int WebRequestRequestHeaderFieldsTooLarge = 526;
        public const int WebRequestUnavailableForLegalReasons = 527;
        public const int WebRequestInternalServerError = 528;
        public const int WebRequestNotImplemented = 529;
        public const int WebRequestBadGateway = 530;
        public const int WebRequestServiceUnavailable = 531;
        public const int WebRequestGatewayTimeout = 532;
        public const int WebRequestHttpVersionNotSupported = 505;
        public const int WebRequestVariantAlsoNegotiates = 534;
        public const int WebRequestInsufficientStorage = 535;
        public const int WebRequestLoopDetected = 536;
        public const int WebRequestNotExtended = 537;
        public const int WebRequestNetworkAuthenticationRequired = 538;

        #endregion

        #region [Sound]

        /// <summary>
        /// 声音组已存在。
        /// </summary>
        public const int SoundGroupAlreadyExists = 601;

        /// <summary>
        /// 声音组不存在。
        /// </summary>
        public const int SoundGroupNotExist = 602;

        /// <summary>
        /// 声音不存在。
        /// </summary>
        public const int SoundNotExist = 603;

        #endregion

        #region [Request]

        /// <summary>
        /// 响应包类型不匹配。
        /// </summary>
        public const int ResponseTypeMismatch = 701;

        /// <summary>
        /// 请求已取消。
        /// </summary>
        public const int RequestCanceled = 702;

        /// <summary>
        /// 请求调度器正在销毁。
        /// </summary>
        public const int RequestDispatcherDisposing = 703;

        #endregion

        #region [Login]

        /// <summary>
        /// 重复登录。
        /// </summary>
        public const int LoginDuplicate = 801;

        /// <summary>
        /// 未登录。
        /// </summary>
        public const int NoLogin = 802;

        #endregion

        #region [BehaviorTree]

        /// <summary>
        /// 行为节点处于Running状态，下次将从这个节点继续执行。
        /// </summary>
        public const int BehaviorRunningState = 901;

        /// <summary>
        /// 行为树装饰节点已存在子节点。
        /// </summary>
        public const int BehaviorTreeDecorNodeAlreadyHasChild = 902;

        /// <summary>
        /// 行为树装饰节点没有子节点。
        /// </summary>
        public const int BehaviorTreeDecorNodeNoChild = 903;

        /// <summary>
        /// 行为树复合节点没有子节点。
        /// </summary>
        public const int BehaviorTreeCompositeNodeNoChild = 904;

        /// <summary>
        /// 行为树选择器节点没有成功。
        /// </summary>
        public const int BehaviorTreeSelectorNoSuccess = 905;

        /// <summary>
        /// 行为节点创建失败。
        /// </summary>
        public const int BehaviorNodeCreateFailed = 906;

        /// <summary>
        /// 行为节点ID重复。
        /// </summary>
        public const int BehaviorNodeDuplicateId = 907;

        /// <summary>
        /// 行为节点未找到。
        /// </summary>
        public const int BehaviorNodeNotFound = 908;

        /// <summary>
        /// 行为节点配置未找到。
        /// </summary>
        public const int BehaviorNodeConfigNotFound = 909;

        /// <summary>
        /// 行为节点无效。
        /// </summary>
        public const int BehaviorNodeInvalid = 910;

        #endregion

        #region [SwipeCard 10001-10099]

        /// <summary>
        /// 周目状态不是等待操作。
        /// </summary>
        public const int SwipeCardRoundStateNotWaitOperate = 10001;

        /// <summary>
        /// 周目状态不是等待显示。
        /// </summary>
        public const int SwipeCardRoundStateNotWaitDisplay = 10002;

        /// <summary>
        /// 周目状态不是周目结束。
        /// </summary>
        public const int SwipeCardRoundStateNotRoundEnd = 10003;

        /// <summary>
        /// 卡牌操作方法无效。
        /// </summary>
        public const int SwipeCardMethodInvalid = 10004;

        /// <summary>
        /// 卡牌操作方法未注册。
        /// </summary>
        public const int SwipeCardMethodNotRegister = 10005;

        /// <summary>
        /// 卡牌操作方法条件不匹配。
        /// </summary>
        public const int SwipeCardMethodConditionNotMatch = 10006;

        /// <summary>
        /// 周目状态不是初始化游戏。
        /// </summary>
        public const int SwipeCardRoundStateNotGameInit = 10007;

        /// <summary>
        /// 周目状态不是初始化周目。
        /// </summary>
        public const int SwipeCardRoundStateNotRoundInit = 10008;

        /// <summary>
        /// 周目状态不是游戏未初始化。
        /// </summary>
        public const int SwipeCardRoundStateNotNone = 10009;

        /// <summary>
        /// 主题配置不存在。
        /// </summary>
        public const int SwipeCardThemeConfigNotFound = 10010;

        /// <summary>
        /// 周目卡牌数量无效。
        /// </summary>
        public const int SwipeCardRoundCardCountInvalid = 10011;

        /// <summary>
        /// 卡牌配置不存在。
        /// </summary>
        public const int SwipeCardCardConfigNotFound = 10012;

        /// <summary>
        /// 周目配置列表为空。
        /// </summary>
        public const int SwipeCardRoundConfigListEmpty = 10013;

        /// <summary>
        /// 游戏失败。
        /// </summary>
        public const int SwipeCardGameLose = 10014;

        /// <summary>
        /// 通关条件未注册。
        /// </summary>
        public const int SwipeCardPassGameConditionNotRegister = 10015;

        /// <summary>
        /// 资源不足。
        /// </summary>
        public const int SwipeCardResourceNotEnough = 10016;

        #endregion
    }
}
