using System;
using System.Collections.Generic;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 资源组件。用于管理血量、魔法值等资源数值。
    /// </summary>
    [GameComponent(GameComponentType.Resource)]
    public sealed class ResourceComponent : GameComponentBase
    {
        /// <summary>
        /// 资源字典。
        /// </summary>
        private readonly Dictionary<int, ResourceInfo> m_ResourceDict = new();

        /// <summary>
        /// 获取或设置资源值。
        /// </summary>
        /// <param name="id">资源id。</param>
        /// <returns>资源值。</returns>
        public long this[int id]
        {
            get => Get(id);
            set => Set(id, value);
        }

        /// <summary>
        /// 获取资源值。
        /// </summary>
        /// <param name="id">资源id。</param>
        /// <returns>资源值。</returns>
        public long Get(int id)
        {
            return m_ResourceDict.TryGetValue(id, out var resourceInfo) ? resourceInfo.Value : 0;
        }

        /// <summary>
        /// 设置资源值。
        /// </summary>
        /// <param name="id">资源id。</param>
        /// <param name="value">资源值。</param>
        public void Set(int id, long value)
        {
            GetOrCreateResourceInfo(id).Value = value;
        }

        /// <summary>
        /// 修改资源值。
        /// </summary>
        /// <param name="id">资源id。</param>
        /// <param name="value">修改值。</param>
        public void Modify(int id, long value)
        {
            GetOrCreateResourceInfo(id).Value += value;
        }

        /// <summary>
        /// 消耗资源。
        /// </summary>
        /// <param name="id">资源id。</param>
        /// <param name="value">消耗值。</param>
        /// <returns>是否消耗成功。</returns>
        public bool Cost(int id, long value)
        {
            var resourceInfo = GetOrCreateResourceInfo(id);
            if (resourceInfo.Value < value)
            {
                return false;
            }

            resourceInfo.Value -= value;
            return true;
        }

        /// <summary>
        /// 消耗资源。
        /// </summary>
        /// <param name="resourceDict">资源字典。</param>
        /// <returns>是否消耗成功。</returns>
        public bool Cost(Dictionary<int, long> resourceDict)
        {
            using var commandList = ReusableList<ResourceInfoModifyCommand>.Create();
            var anyFailed = false;

            foreach (var resource in resourceDict)
            {
                var resourceInfo = GetOrCreateResourceInfo(resource.Key);
                if (resourceInfo.Value < resource.Value)
                {
                    anyFailed = true;
                    break;
                }

                commandList.Add(ResourceInfoModifyCommand.Create(resourceInfo, -resource.Value));
            }

            if (!anyFailed)
            {
                foreach (var command in commandList)
                {
                    command.Execute();
                    command.Dispose();
                }
            }
            else
            {
                foreach (var command in commandList)
                {
                    command.Dispose();
                }
            }

            return !anyFailed;
        }

        /// <summary>
        /// 恢复到最大值。仅当最大值大于等于0时有效。
        /// </summary>
        /// <param name="id">资源id。</param>
        public void RecoverToMax(int id)
        {
            var resourceInfo = GetOrCreateResourceInfo(id);
            if (resourceInfo.MaxValue >= 0)
            {
                resourceInfo.Value = resourceInfo.MaxValue;
            }
        }

        /// <summary>
        /// 获取最大资源值。-1表示无上限。
        /// </summary>
        /// <param name="id">资源id。</param>
        /// <returns>最大资源值。</returns>
        public long GetMax(int id)
        {
            return GetOrCreateResourceInfo(id).MaxValue;
        }

        /// <summary>
        /// 设置最大资源值。0表示无上限。
        /// </summary>
        /// <param name="id">资源id。</param>
        /// <param name="value">最大资源值。</param>
        public void SetMax(int id, long value)
        {
            GetOrCreateResourceInfo(id).MaxValue = value;
        }

        /// <summary>
        /// 修改最大资源值。
        /// </summary>
        /// <param name="id">资源id。</param>
        /// <param name="value">修改值。</param>
        public void ModifyMax(int id, long value)
        {
            GetOrCreateResourceInfo(id).MaxValue += value;
        }

        /// <summary>
        /// 将最大值与指定属性绑定。
        /// </summary>
        /// <param name="id">资源id。</param>
        /// <param name="numericId">属性id。</param>
        /// <param name="numericComponent">属性组件。</param>
        public void BindNumericMax(int id, int numericId, NumericComponent numericComponent)
        {
            GetOrCreateResourceInfo(id).BindNumeric(numericId, numericComponent);
        }

        /// <summary>
        /// 取消绑定属性。
        /// </summary>
        /// <param name="id">资源id。</param>
        public void UnbindNumericMax(int id)
        {
            if (!m_ResourceDict.TryGetValue(id, out var resourceInfo))
            {
                return;
            }

            resourceInfo.UnbindNumeric();
        }

        /// <summary>
        /// 订阅资源变更事件。
        /// </summary>
        /// <param name="id">资源id。</param>
        /// <param name="handler">资源变更事件处理函数。</param>
        public void Subscribe(int id, SimpleEventHandler<long> handler, bool notifyImmediately = false)
        {
            var resourceInfo = GetOrCreateResourceInfo(id);
            resourceInfo.ChangeEvent.Subscribe(handler);
            if (notifyImmediately)
            {
                handler.Invoke(resourceInfo.Value);
            }
        }

        /// <summary>
        /// 取消订阅资源变更事件。
        /// </summary>
        /// <param name="id">资源id。</param>
        /// <param name="handler">资源变更事件处理函数。</param>
        public void Unsubscribe(int id, SimpleEventHandler<long> handler)
        {
            if (!m_ResourceDict.TryGetValue(id, out var resourceInfo))
            {
                return;
            }

            resourceInfo.ChangeEvent.Unsubscribe(handler);
        }

        /// <summary>
        /// 订阅最大资源值变更事件。
        /// </summary>
        /// <param name="id">资源id。</param>
        /// <param name="handler">最大资源值变更事件处理函数。</param>
        /// <param name="notifyImmediately">是否立即通知。</param>
        public void SubscribeMax(int id, SimpleEventHandler<long> handler, bool notifyImmediately = false)
        {
            var resourceInfo = GetOrCreateResourceInfo(id);
            resourceInfo.MaxChangeEvent.Subscribe(handler);
            if (notifyImmediately)
            {
                handler.Invoke(resourceInfo.MaxValue);
            }
        }

        /// <summary>
        /// 取消订阅最大资源值变更事件。
        /// </summary>
        /// <param name="id">资源id。</param>
        /// <param name="handler">最大资源值变更事件处理函数。</param>
        public void UnsubscribeMax(int id, SimpleEventHandler<long> handler)
        {
            if (!m_ResourceDict.TryGetValue(id, out var resourceInfo))
            {
                return;
            }

            resourceInfo.MaxChangeEvent.Unsubscribe(handler);
        }

        /// <summary>
        /// 获取或创建资源信息。
        /// </summary>
        /// <param name="id">资源id。</param>
        /// <returns>资源信息。</returns>
        private ResourceInfo GetOrCreateResourceInfo(int id)
        {
            if (m_ResourceDict.TryGetValue(id, out var resourceInfo))
            {
                return resourceInfo;
            }

            resourceInfo = ResourceInfo.Create();
            m_ResourceDict.Add(id, resourceInfo);
            return resourceInfo;
        }


        protected override void OnAwake()
        {
        }

        protected override void OnDispose()
        {
            foreach (var resourceInfo in m_ResourceDict)
            {
                resourceInfo.Value.Dispose();
            }

            m_ResourceDict.Clear();
        }

        /// <summary>
        /// 资源值修改命令。
        /// </summary>
        private sealed class ResourceInfoModifyCommand : IDisposable, IReference
        {
            public ResourceInfo ResourceInfo { get; set; }
            public long ModifyValue { get; set; }

            public void Dispose()
            {
                ReferencePool.Release(this);
            }

            public void Clear()
            {
                ResourceInfo = null;
                ModifyValue = 0;
            }

            public void Execute()
            {
                ResourceInfo.Value += ModifyValue;
            }

            public static ResourceInfoModifyCommand Create(ResourceInfo resourceInfo, long modifyValue)
            {
                var command = ReferencePool.Acquire<ResourceInfoModifyCommand>();
                command.ResourceInfo = resourceInfo;
                command.ModifyValue = modifyValue;
                return command;
            }
        }

        private sealed class ResourceInfo : IDisposable, IReference
        {
            /// <summary>
            /// 资源值。
            /// </summary>
            public long Value
            {
                get => m_Value;
                set
                {
                    if (m_Value == value)
                    {
                        return;
                    }

                    if (m_MaxValue != -1 && value > m_MaxValue)
                    {
                        value = m_MaxValue;

                        if (m_Value == value)
                        {
                            return;
                        }
                    }

                    m_Value = value;
                    ChangeEvent.Dispatch(m_Value);
                }
            }

            /// <summary>
            /// 最大资源值，-1表示无上限。
            /// </summary>
            public long MaxValue
            {
                get => m_MaxValue;
                set
                {
                    if (m_MaxValue == value)
                    {
                        return;
                    }

                    m_MaxValue = value;

                    if (m_Value > value)
                    {
                        Value = value;
                    }

                    MaxChangeEvent.Dispatch(m_MaxValue);
                }
            }

            /// <summary>
            /// 资源变更事件。
            /// </summary>
            public SimpleEvent<long> ChangeEvent { get; private set; }

            /// <summary>
            /// 最大资源值变更事件。
            /// </summary>
            public SimpleEvent<long> MaxChangeEvent { get; private set; }

            private long m_Value;
            private long m_MaxValue = -1;
            private int m_NumericId;
            private GameComponentRef<NumericComponent> m_NumericComponent;

            public void Dispose()
            {
                ReferencePool.Release(this);
            }

            public void Clear()
            {
                UnbindNumeric();
                m_Value = 0;
                m_MaxValue = -1;
                ChangeEvent.Dispose();
                ChangeEvent = null;
                MaxChangeEvent.Dispose();
                MaxChangeEvent = null;
            }

            /// <summary>
            /// 将最大值与指定属性绑定。
            /// </summary>
            /// <param name="numericId">属性id。</param>
            /// <param name="numericComponent">属性组件。</param>
            public void BindNumeric(int numericId, NumericComponent numericComponent)
            {
                UnbindNumeric();

                m_NumericId = numericId;
                m_NumericComponent = numericComponent;

                if (numericComponent != null && numericId != 0)
                {
                    numericComponent.Subscribe(m_NumericId, NumericSubType.Final, OnNumericChange, true);
                }
            }

            /// <summary>
            /// 取消绑定属性。
            /// </summary>
            public void UnbindNumeric()
            {
                NumericComponent numericComponent = m_NumericComponent;
                if (numericComponent != null && m_NumericId != 0)
                {
                    numericComponent.Unsubscribe(m_NumericId, NumericSubType.Final, OnNumericChange);
                }

                m_NumericId = 0;
                m_NumericComponent = default;
            }

            /// <summary>
            /// 属性值变更事件。
            /// </summary>
            /// <param name="value">属性值。</param>
            private void OnNumericChange(long value)
            {
                MaxValue = value;
            }

            public static ResourceInfo Create()
            {
                var info = ReferencePool.Acquire<ResourceInfo>();
                info.ChangeEvent = SimpleEvent<long>.Create();
                info.MaxChangeEvent = SimpleEvent<long>.Create();
                return info;
            }
        }
    }
}