namespace Grains.Extensions;

public static class CollectionExtensions
{
    /// <summary>
    /// 深度克隆字典。
    /// </summary>
    public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> dict) where TKey : notnull
    {
        var clone = new Dictionary<TKey, TValue>(dict.Count);
        foreach (var kvp in dict)
        {
            clone.Add(kvp.Key, kvp.Value);
        }
        return clone;
    }
}