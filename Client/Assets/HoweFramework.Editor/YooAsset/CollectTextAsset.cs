using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 收集文本资源。
    /// </summary>
    public class CollectTextAsset : IAssetFilterRule
    {
        public string FindAssetType => "TextAsset";
        public bool IsCollectAsset(AssetFilterRuleData data)
        {
            return AssetDatabase.LoadAssetAtPath<TextAsset>(data.AssetPath) != null;
        }
    }
}
