using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 本地化模块。
    /// </summary>
    public sealed class LocalizationModule : ModuleBase<LocalizationModule>
    {
        /// <summary>
        /// 默认语言。
        /// </summary>
        public Language DefaultLanguage => Language.ChineseSimplified;

        /// <summary>
        /// 当前游戏语言。
        /// </summary>
        public Language Language
        {
            get => m_Language;
            set
            {
                if (value == Language.Unspecified)
                {
                    throw new ErrorCodeException(FrameworkErrorCode.InvalidParam, "Language is unspecified.");
                }

                if (m_Language == value)
                {
                    return;
                }

                m_Language = value;

                // 保存到本地。
                SaveLanguage(value);

                ReloadLanguageAsync(value).Forget();
            }
        }

        /// <summary>
        /// 当前系统语言。
        /// </summary>
        public Language SystemLanguage
        {
            get
            {
                return Application.systemLanguage switch
                {
                    UnityEngine.SystemLanguage.Afrikaans => Language.Afrikaans,
                    UnityEngine.SystemLanguage.Arabic => Language.Arabic,
                    UnityEngine.SystemLanguage.Basque => Language.Basque,
                    UnityEngine.SystemLanguage.Belarusian => Language.Belarusian,
                    UnityEngine.SystemLanguage.Bulgarian => Language.Bulgarian,
                    UnityEngine.SystemLanguage.Catalan => Language.Catalan,
                    UnityEngine.SystemLanguage.Chinese => Language.ChineseSimplified,
                    UnityEngine.SystemLanguage.ChineseSimplified => Language.ChineseSimplified,
                    UnityEngine.SystemLanguage.ChineseTraditional => Language.ChineseTraditional,
                    UnityEngine.SystemLanguage.Czech => Language.Czech,
                    UnityEngine.SystemLanguage.Danish => Language.Danish,
                    UnityEngine.SystemLanguage.Dutch => Language.Dutch,
                    UnityEngine.SystemLanguage.English => Language.English,
                    UnityEngine.SystemLanguage.Estonian => Language.Estonian,
                    UnityEngine.SystemLanguage.Faroese => Language.Faroese,
                    UnityEngine.SystemLanguage.Finnish => Language.Finnish,
                    UnityEngine.SystemLanguage.French => Language.French,
                    UnityEngine.SystemLanguage.German => Language.German,
                    UnityEngine.SystemLanguage.Greek => Language.Greek,
                    UnityEngine.SystemLanguage.Hebrew => Language.Hebrew,
                    UnityEngine.SystemLanguage.Hungarian => Language.Hungarian,
                    UnityEngine.SystemLanguage.Icelandic => Language.Icelandic,
                    UnityEngine.SystemLanguage.Indonesian => Language.Indonesian,
                    UnityEngine.SystemLanguage.Italian => Language.Italian,
                    UnityEngine.SystemLanguage.Japanese => Language.Japanese,
                    UnityEngine.SystemLanguage.Korean => Language.Korean,
                    UnityEngine.SystemLanguage.Latvian => Language.Latvian,
                    UnityEngine.SystemLanguage.Lithuanian => Language.Lithuanian,
                    UnityEngine.SystemLanguage.Norwegian => Language.Norwegian,
                    UnityEngine.SystemLanguage.Polish => Language.Polish,
                    UnityEngine.SystemLanguage.Portuguese => Language.PortuguesePortugal,
                    UnityEngine.SystemLanguage.Romanian => Language.Romanian,
                    UnityEngine.SystemLanguage.Russian => Language.Russian,
                    UnityEngine.SystemLanguage.SerboCroatian => Language.SerboCroatian,
                    UnityEngine.SystemLanguage.Slovak => Language.Slovak,
                    UnityEngine.SystemLanguage.Slovenian => Language.Slovenian,
                    UnityEngine.SystemLanguage.Spanish => Language.Spanish,
                    UnityEngine.SystemLanguage.Swedish => Language.Swedish,
                    UnityEngine.SystemLanguage.Thai => Language.Thai,
                    UnityEngine.SystemLanguage.Turkish => Language.Turkish,
                    UnityEngine.SystemLanguage.Ukrainian => Language.Ukrainian,
                    UnityEngine.SystemLanguage.Unknown => Language.Unspecified,
                    UnityEngine.SystemLanguage.Vietnamese => Language.Vietnamese,
                    _ => Language.Unspecified,
                };
            }

        }

        /// <summary>
        /// 当前语言。
        /// </summary>
        private Language m_Language = Language.Unspecified;

        /// <summary>
        /// 本地化文本字典。
        /// </summary>
        private readonly Dictionary<string, string> m_LocalizationTextDict = new();

        /// <summary>
        /// 本地化数据源列表。
        /// </summary>
        private readonly List<ILocalizationSource> m_SourceList = new();

        /// <summary>
        /// 获取当前语言的本地化文本。
        /// </summary>
        /// <param name="key">文本键。</param>
        /// <returns>本地化文本。若不存在，则返回带文本键的错误信息。</returns>
        public string GetText(string key)
        {
            if (m_LocalizationTextDict.TryGetValue(key, out var text))
            {
                return text;
            }

            return string.Format("<NoKey>{0}", key);
        }

        /// <summary>
        /// 添加本地化文本。
        /// </summary>
        /// <param name="key">文本键。</param>
        /// <param name="text">本地化文本。</param>
        public void AddText(string key, string text)
        {
            m_LocalizationTextDict[key] = text;
        }

        /// <summary>
        /// 清空本地化文本。
        /// </summary>
        public void ClearText()
        {
            m_LocalizationTextDict.Clear();
        }

        /// <summary>
        /// 添加本地化数据源。
        /// </summary>
        /// <param name="source">本地化数据源。</param>
        public void AddSource(ILocalizationSource source)
        {
            m_SourceList.Add(source);
        }

        /// <summary>
        /// 移除本地化数据源。
        /// </summary>
        /// <param name="source">本地化数据源。</param>
        public void RemoveSource(ILocalizationSource source)
        {
            if (!m_SourceList.Remove(source))
            {
                return;
            }

            source.Dispose();
        }

        /// <summary>
        /// 异步加载本地化数据。
        /// </summary>
        public async UniTask LoadAsync()
        {
            if (m_Language == Language.Unspecified)
            {
                throw new ErrorCodeException(FrameworkErrorCode.FrameworkException, "Language is unspecified.");
            }

            // 加载前清空旧文本，避免重复加载/切换语言时旧语言文本残留。
            ClearText();

            using var uniTaskList = ReusableList<UniTask>.Create();

            foreach (var source in m_SourceList)
            {
                uniTaskList.Add(source.Load(m_Language));
            }

            await UniTask.WhenAll(uniTaskList);
        }

        /// <summary>
        /// 保存语言设置。
        /// </summary>
        private void SaveLanguage(Language language)
        {
            SettingModule.Instance.SetInt(FrameworkSettings.Language, (int)language);
            SettingModule.Instance.Save();
        }

        private async UniTaskVoid ReloadLanguageAsync(Language language)
        {
            try
            {
                await LoadAsync();
                EventModule.Instance.Dispatch(this, LocalizationLanguageUpdateEventArgs.Create(language));
            }
            catch (System.Exception e)
            {
                Log.Error($"切换语言失败：{e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// 加载语言设置。
        /// </summary>
        private Language LoadLanguage()
        {
            var language = (Language)SettingModule.Instance.GetInt(FrameworkSettings.Language, (int)DefaultLanguage);

            // 存档损坏可能产生未定义的枚举值，回退到默认语言。
            if (!System.Enum.IsDefined(typeof(Language), language))
            {
                language = DefaultLanguage;
            }

            return language;
        }

        protected override void OnInit()
        {
            m_Language = LoadLanguage();
        }

        protected override void OnDestroy()
        {
            foreach (var source in m_SourceList)
            {
                source.Dispose();
            }

            m_SourceList.Clear();
            m_LocalizationTextDict.Clear();
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}
