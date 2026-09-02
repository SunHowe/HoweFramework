using UnityEditor;
using YooAsset.Editor;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 收集纹理。
    /// </summary>
    public class CollectTexture : IAssetFilterRule
    {
        public string FindAssetType => "Texture2D";
        public bool IsCollectAsset(AssetFilterRuleData data)
        {
            var textureImporter = AssetImporter.GetAtPath(data.AssetPath) as TextureImporter;
            return textureImporter != null && textureImporter.textureType == TextureImporterType.Default;
        }
    }
}
