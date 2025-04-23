namespace HoweFramework
{
    /// <summary>
    /// 本地化语言更新事件。
    /// </summary>
    public sealed class LocalizationLanguageUpdateEventArgs : GameEventArgs
    {
        /// <summary>
        /// 事件编号。
        /// </summary>
        public static readonly int EventId = typeof(LocalizationLanguageUpdateEventArgs).GetHashCode();

        /// <summary>
        /// 获取事件编号。
        /// </summary>
        public override int Id => EventId;

        /// <summary>
        /// 新设置的语言。
        /// </summary>
        public Language Language { get; private set; }

        /// <summary>
        /// 初始化事件新实例。
        /// </summary>
        public LocalizationLanguageUpdateEventArgs()
        {
        }

        /// <summary>
        /// 创建事件。
        /// </summary>
        public static LocalizationLanguageUpdateEventArgs Create(Language language)
        {
            var eventArgs = ReferencePool.Acquire<LocalizationLanguageUpdateEventArgs>();
            eventArgs.Language = language;
            return eventArgs;
        }

        /// <summary>
        /// 清理事件。
        /// </summary>
        public override void Clear()
        {
            Language = Language.Unspecified;
        }
    }
}