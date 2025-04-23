using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

namespace HoweFramework.Editor
{
    /// <summary>
    /// 收集文本资源。
    /// </summary>
    public class CollectTextAsset : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            return AssetDatabase.LoadAssetAtPath<TextAsset>(data.AssetPath) != null;
        }
    }
}
