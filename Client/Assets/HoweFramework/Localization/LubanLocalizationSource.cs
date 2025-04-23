using Cysharp.Threading.Tasks;
using Luban;

namespace HoweFramework
{
    /// <summary>
    /// 基于Luban的本地化数据源。
    /// </summary>
    public sealed class LubanLocalizationSource : ILocalizationSource
    {
        private readonly string m_PathFormat;
        private readonly IResLoader m_ResLoader;

        public LubanLocalizationSource(string pathFormat = "Assets/GameMain/Localization/{0}.bytes")
        {
            m_PathFormat = pathFormat;
            m_ResLoader = ResModule.Instance.CreateResLoader();
        }

        public void Dispose()
        {
            m_ResLoader.Dispose();
        }

        public async UniTask Load(Language language)
        {
            var assetPath = string.Format(m_PathFormat, language);

            var bytes = await m_ResLoader.LoadBinaryAsync(assetPath);
            if (bytes == null)
            {
                throw new ErrorCodeException(ErrorCode.ResNotFound, assetPath);
            }

            var byteBuf = new ByteBuf(bytes);
            
            for (var n = byteBuf.ReadSize(); n > 0; --n)
            {
                var key = byteBuf.ReadString();
                var value = byteBuf.ReadString();

                LocalizationModule.Instance.AddText(key, value);
            }

            byteBuf.Release();
        }
    }
}

