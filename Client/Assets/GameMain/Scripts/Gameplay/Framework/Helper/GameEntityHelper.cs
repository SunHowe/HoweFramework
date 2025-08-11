using System;
using System.Collections.Generic;
using System.Reflection;
using HoweFramework;
using UnityEngine;

namespace GameMain
{
    /// <summary>
    /// 游戏实体辅助工具。
    /// </summary>
    public static class GameEntityHelper
    {
        private static readonly Dictionary<Type, int> s_ComponentTypeDict = new();

        public static void Clear()
        {
            s_ComponentTypeDict.Clear();
        }

        /// <summary>
        /// 获取组件类型.
        /// </summary>
        public static int GetComponentType(Type type)
        {
            if (s_ComponentTypeDict.TryGetValue(type, out var componentType))
            {
                return componentType;
            }

            var attribute = type.GetCustomAttribute<GameComponentAttribute>();
            if (attribute == null)
            {
                throw new Exception(string.Format("Type '{0}' is not a game component type.", type.FullName));
            }

            componentType = attribute.ComponentType;
            s_ComponentTypeDict.Add(type, componentType);

            return componentType;
        }

        /// <summary>
        /// 获取组件类型.
        /// </summary>
        public static int GetComponentType<T>() where T : GameComponentBase
        {
            return GetComponentType(typeof(T));
        }

        /// <summary>
        /// 添加组件.
        /// </summary>
        public static T AddComponent<T>(this IGameEntity entity) where T : GameComponentBase, new()
        {
            var component = ReferencePool.Acquire<T>();
            entity.AddComponent(component);
            return component;
        }

        /// <summary>
        /// 获取组件实例。
        /// </summary>
        public static T GetComponent<T>(this IGameEntity entity) where T : GameComponentBase
        {
            return (T)entity.GetComponent(GetComponentType<T>());
        }

        /// <summary>
        /// 获取组件实例。
        /// </summary>
        public static IGameComponent GetComponent(this IGameEntity entity, Type type)
        {
            return entity.GetComponent(GetComponentType(type));
        }

        /// <summary>
        /// 移除组件.
        /// </summary>
        public static void RemoveComponent<T>(this IGameEntity entity) where T : GameComponentBase
        {
            entity.RemoveComponent(GetComponentType<T>());
        }

        /// <summary>
        /// 移除组件.
        /// </summary>
        public static void RemoveComponent(this IGameEntity entity, Type type)
        {
            entity.RemoveComponent(GetComponentType(type));
        }

        /// <summary>
        /// 添加组件.
        /// </summary>
        public static void AddComponent(this IGameComponent component, IGameComponent addComponent)
        {
            component.Entity.AddComponent(addComponent);
        }

        /// <summary>
        /// 添加组件.
        /// </summary>
        public static T AddComponent<T>(this IGameComponent component) where T : GameComponentBase, new()
        {
            return component.Entity.AddComponent<T>();
        }

        /// <summary>
        /// 获取组件实例。
        /// </summary>
        public static T GetComponent<T>(this IGameComponent component) where T : GameComponentBase
        {
            return component.Entity.GetComponent<T>();
        }

        /// <summary>
        /// 获取组件实例。
        /// </summary>
        public static IGameComponent GetComponent(this IGameComponent component, Type type)
        {
            return component.Entity.GetComponent(type);
        }

        /// <summary>
        /// 移除组件.
        /// </summary>
        public static void RemoveComponent<T>(this IGameComponent component) where T : GameComponentBase
        {
            component.Entity.RemoveComponent<T>();
        }

        /// <summary>
        /// 移除组件.
        /// </summary>
        public static void RemoveComponent(this IGameComponent component, Type type)
        {
            component.Entity.RemoveComponent(type);
        }

        /// <summary>
        /// 获取所有组件.
        /// </summary>
        public static IGameComponent[] GetComponents(this IGameEntity entity)
        {
            using var components = ReusableList<IGameComponent>.Create();
            entity.GetComponents(components);
            return components.ToArray();
        }

        /// <summary>
        /// 获取所有组件.
        /// </summary>
        public static IGameComponent[] GetComponents(this IGameComponent component)
        {
            return component.Entity.GetComponents();
        }

        /// <summary>
        /// 获取所有组件.
        /// </summary>
        public static void GetComponents(this IGameComponent component, List<IGameComponent> components)
        {
            component.Entity.GetComponents(components);
        }

        /// <summary>
        /// 获取管理器.
        /// </summary>
        public static T GetManager<T>(this IGameComponent component) where T : IGameManager
        {
            return component.Context.GetManager<T>();
        }

        /// <summary>
        /// 获取游戏实体。
        /// </summary>
        /// <param name="gameObject">游戏对象。</param>
        /// <returns>游戏实体。</returns>
        public static IGameEntity GetGameEntity(this GameObject gameObject)
        {
            var blackboard = gameObject.GetComponentInSelfOrParent<BlackboardComponent>();
            if (blackboard == null)
            {
                return null;
            }

            return blackboard.GetValue<IGameEntity>("GameEntity");
        }

        /// <summary>
        /// 获取游戏实体。
        /// </summary>
        /// <param name="hits">射线检测结果。</param>
        /// <param name="entities">检测到的游戏实体。</param>
        /// <returns>检测到的游戏实体数量。</returns>
        public static int GetGameEntities(in ReadOnlySpan<RaycastHit> hits, List<IGameEntity> entities)
        {
            entities.Clear();
            foreach (var hit in hits)
            {
                var gameObject = hit.collider.gameObject;
                if (gameObject == null)
                {
                    continue;
                }

                var entity = gameObject.GetGameEntity();
                if (entity == null)
                {
                    continue;
                }

                entities.Add(entity);
            }

            return entities.Count;
        }

        /// <summary>
        /// 射线检测游戏实体。
        /// </summary>
        /// <param name="ray">射线。</param>
        /// <param name="entities">检测到的游戏实体。</param>
        /// <returns>检测到的游戏实体数量。</returns>
        public static int RaycastGameEntities(in Ray ray, List<IGameEntity> entities)
        {
            var hits = PhysicsUtility.Raycast(ray);
            return GetGameEntities(hits, entities);
        }

        /// <summary>
        /// 射线检测游戏实体。
        /// </summary>
        /// <param name="ray">射线。</param>
        /// <param name="entities">检测到的游戏实体。</param>
        /// <param name="maxDistance">最大距离。</param>
        /// <returns>检测到的游戏实体数量。</returns>
        public static int RaycastGameEntities(in Ray ray, List<IGameEntity> entities, float maxDistance)
        {
            var hits = PhysicsUtility.Raycast(ray, maxDistance);
            return GetGameEntities(hits, entities);
        }

        /// <summary>
        /// 射线检测游戏实体。
        /// </summary>
        /// <param name="ray">射线。</param>
        /// <param name="entities">检测到的游戏实体。</param>
        /// <param name="layerMask">层掩码。</param>
        /// <returns>检测到的游戏实体数量。</returns>
        public static int RaycastGameEntities(in Ray ray, List<IGameEntity> entities, int layerMask)
        {
            var hits = PhysicsUtility.Raycast(ray, layerMask);
            return GetGameEntities(hits, entities);
        }

        /// <summary>
        /// 射线检测游戏实体。
        /// </summary>
        /// <param name="ray">射线。</param>
        /// <param name="entities">检测到的游戏实体。</param>
        /// <param name="layerMask">层掩码。</param>
        /// <param name="maxDistance">最大距离。</param>
        /// <returns>检测到的游戏实体数量。</returns>
        public static int RaycastGameEntities(in Ray ray, List<IGameEntity> entities, int layerMask, float maxDistance)
        {
            var hits = PhysicsUtility.Raycast(ray, layerMask, maxDistance);
            return GetGameEntities(hits, entities);
        }

        /// <summary>
        /// 射线检测游戏实体。
        /// </summary>
        /// <param name="ray">射线。</param>
        /// <param name="entities">检测到的游戏实体。</param>
        /// <param name="layerMask">层掩码。</param>
        /// <param name="maxDistance">最大距离。</param>
        /// <param name="queryTriggerInteraction">触发器交互。</param>
        /// <returns>检测到的游戏实体数量。</returns>
        public static int RaycastGameEntities(in Ray ray, List<IGameEntity> entities, int layerMask, float maxDistance, QueryTriggerInteraction queryTriggerInteraction)
        {
            var hits = PhysicsUtility.Raycast(ray, layerMask, maxDistance, queryTriggerInteraction);
            return GetGameEntities(hits, entities);
        }
    }
}
