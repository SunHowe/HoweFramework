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

        /// <summary>
        /// 从自己或父节点上搜索指定类型的组件。
        /// </summary>
        public static T GetComponentInSelfOrParent<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.GetComponentInParent<T>();
            }

            return component;
        }

        /// <summary>
        /// 从自己或父节点上搜索指定类型的组件。
        /// </summary>
        public static T GetComponentInSelfOrParent<T>(this GameObject gameObject, bool includeInactive) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.GetComponentInParent<T>(includeInactive);
            }

            return component;
        }

        /// <summary>
        /// 从自己或父节点上搜索指定类型的组件。
        /// </summary>
        public static T GetComponentInSelfOrParent<T>(this Component component) where T : Component
        {
            return component.gameObject.GetComponentInSelfOrParent<T>();
        }

        /// <summary>
        /// 从自己或父节点上搜索指定类型的组件。
        /// </summary>
        public static T GetComponentInSelfOrParent<T>(this Component component, bool includeInactive) where T : Component
        {
            return component.gameObject.GetComponentInSelfOrParent<T>(includeInactive);
        }

        /// <summary>
        /// 销毁游戏对象。
        /// </summary>
        /// <param name="gameObject">游戏对象。</param>
        public static void Destroy(this GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }

            using var customDestroyBuffer = ReusableList<ICustomDestroy>.Create();
            gameObject.GetComponents(customDestroyBuffer);
            
            if (customDestroyBuffer.Count == 0)
            {
                Object.Destroy(gameObject);
                return;
            }
            else if (customDestroyBuffer.Count > 1)
            {
                Debug.LogWarningFormat("GameObject '{0}' has multiple ICustomDestroy components.", gameObject.name);
            }

            customDestroyBuffer[0].CustomDestroy();
        }
    }
}
