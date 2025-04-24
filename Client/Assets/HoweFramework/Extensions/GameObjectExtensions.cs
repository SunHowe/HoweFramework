using UnityEngine;

namespace HoweFramework
{
    /// <summary>
    /// 游戏对象扩展。
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// 获取或添加组件。
        /// </summary>
        /// <typeparam name="T">组件类型。</typeparam>
        /// <param name="gameObject">游戏对象。</param>
        /// <returns>组件。</returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }

        /// <summary>
        /// 设置父级。
        /// </summary>
        /// <param name="gameObject">游戏对象。</param>
        /// <param name="parent">父级。</param>
        public static void SetParent(this GameObject gameObject, Transform parent)
        {
            gameObject.transform.SetParent(parent);
        }
    }
}
