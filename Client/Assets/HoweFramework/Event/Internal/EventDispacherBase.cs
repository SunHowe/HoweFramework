using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 事件派发器基类。
    /// </summary>
    internal abstract class EventDispatcherBase : IEventDispatcher
    {
        /// <summary>
        /// 事件处理器字典。
        /// </summary>
        private readonly MultiDictionary<int, GameEventHandler> m_EventHandlerDict = new();

        /// <summary>
        /// 缓存的事件处理器节点。key 为派发序号，避免同一事件实例重入 Dispatch 时互相覆盖。
        /// </summary>
        private readonly Dictionary<int, LinkedListNode<GameEventHandler>> m_CachedNodes = new();

        /// <summary>
        /// 派发序号对应的事件 Id，供 Unsubscribe 过滤。
        /// </summary>
        private readonly Dictionary<int, int> m_CachedEventIds = new();

        /// <summary>
        /// 临时的事件处理器节点。
        /// </summary>
        private readonly Dictionary<int, LinkedListNode<GameEventHandler>> m_TempNodes = new();

        private int m_DispatchSerial;

        private GameEventHandlerFunc m_DefaultHandler;
        private EventDispatcherMode m_Mode;

        /// <summary>
        /// 设置默认事件处理函数。
        /// </summary>
        /// <param name="handler">要设置的默认事件处理函数。</param>
        public void SetDefaultHandler(GameEventHandlerFunc handler)
        {
            m_DefaultHandler = handler;
        }

        /// <summary>
        /// 设置事件调度器模式。
        /// </summary>
        /// <param name="mode">事件调度器模式。</param>
        public void SetMode(EventDispatcherMode mode)
        {
            m_Mode = mode;
        }

        /// <summary>
        /// 检查是否存在事件处理函数。
        /// </summary>
        /// <param name="id">事件类型编号。</param>
        /// <param name="handler">要检查的事件处理函数。</param>
        /// <returns>是否存在事件处理函数。</returns>
        public bool Check(int id, GameEventHandler handler)
        {
            if (handler == null)
            {
                throw new ErrorCodeException(FrameworkErrorCode.InvalidOperationException, "Event handler is invalid.");
            }

            return m_EventHandlerDict.Contains(id, handler);
        }

        /// <summary>
        /// 订阅事件。
        /// </summary>
        /// <param name="id">事件id。</param>
        /// <param name="handler">事件处理器。</param>
        public void Subscribe(int id, GameEventHandler handler)
        {
            if (handler == null)
            {
                throw new ErrorCodeException(FrameworkErrorCode.InvalidOperationException, "Event handler is invalid.");
            }

            if (!m_EventHandlerDict.Contains(id))
            {
                m_EventHandlerDict.Add(id, handler);
            }
            else if ((m_Mode & EventDispatcherMode.AllowMultiHandler) != EventDispatcherMode.AllowMultiHandler)
            {
                throw new ErrorCodeException(FrameworkErrorCode.InvalidOperationException, $"Event '{id}' not allow multi handler.");
            }
            else if ((m_Mode & EventDispatcherMode.AllowDuplicateHandler) != EventDispatcherMode.AllowDuplicateHandler && Check(id, handler))
            {
                throw new ErrorCodeException(FrameworkErrorCode.InvalidOperationException, $"Event '{id}' not allow duplicate handler.");
            }
            else
            {
                m_EventHandlerDict.Add(id, handler);
            }
        }

        /// <summary>
        /// 取消订阅事件。
        /// </summary>
        /// <param name="id">事件id。</param>
        /// <param name="handler">事件处理器。</param>
        public void Unsubscribe(int id, GameEventHandler handler)
        {
            if (handler == null)
            {
                throw new ErrorCodeException(FrameworkErrorCode.InvalidOperationException, "Event handler is invalid.");
            }

            if (m_CachedNodes.Count > 0)
            {
                foreach (KeyValuePair<int, LinkedListNode<GameEventHandler>> cachedNode in m_CachedNodes)
                {
                    if (m_CachedEventIds.TryGetValue(cachedNode.Key, out var eventId) && eventId != id)
                    {
                        continue;
                    }

                    if (cachedNode.Value != null && cachedNode.Value.Value == handler)
                    {
                        m_TempNodes.Add(cachedNode.Key, cachedNode.Value.Next);
                    }
                }

                if (m_TempNodes.Count > 0)
                {
                    foreach (KeyValuePair<int, LinkedListNode<GameEventHandler>> cachedNode in m_TempNodes)
                    {
                        m_CachedNodes[cachedNode.Key] = cachedNode.Value;
                    }

                    m_TempNodes.Clear();
                }
            }

            if (!m_EventHandlerDict.Remove(id, handler))
            {
                throw new ErrorCodeException(FrameworkErrorCode.InvalidOperationException, $"Event '{id}' not exists specified handler.");
            }
        }

        /// <summary>
        /// 派发事件。
        /// </summary>
        /// <param name="sender">事件发送者。</param>
        /// <param name="eventArgs">事件参数。</param>
        public void Dispatch(object sender, GameEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                throw new ErrorCodeException(FrameworkErrorCode.InvalidParam, "Event args is invalid.");
            }

            HandleEvent(sender, eventArgs);
        }

        protected void HandleEvent(object sender, GameEventArgs e)
        {
            bool noHandlerException = true;
            int dispatchToken = ++m_DispatchSerial;
            m_CachedEventIds[dispatchToken] = e.Id;
            try
            {
                if (m_EventHandlerDict.TryGetValue(e.Id, out var range))
                {
                    noHandlerException = false;

                    LinkedListNode<GameEventHandler> current = range.First;
                    while (current != null && current != range.Terminal)
                    {
                        m_CachedNodes[dispatchToken] = current.Next != range.Terminal ? current.Next : null;

                        try
                        {
                            current.Value(sender, e);
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Handle event '{e.Id}' error: {ex.Message}\n{ex.StackTrace}");
                        }

                        current = m_CachedNodes[dispatchToken];
                    }

                    // 如果存在默认事件处理函数，并且事件调度器模式为总是触发默认事件处理函数，则触发默认事件处理函数。
                    if (m_DefaultHandler != null && (m_Mode & EventDispatcherMode.AlwaysInvokeDefaultHandler) == EventDispatcherMode.AlwaysInvokeDefaultHandler)
                    {
                        try
                        {
                            m_DefaultHandler(sender, e);
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Handle event '{e.Id}' error: {ex.Message}\n{ex.StackTrace}");
                        }
                    }
                }
                else if (m_DefaultHandler != null)
                {
                    try
                    {
                        noHandlerException = !m_DefaultHandler(sender, e);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Handle event '{e.Id}' error: {ex.Message}\n{ex.StackTrace}");
                    }
                }
            }
            finally
            {
                m_CachedNodes.Remove(dispatchToken);
                m_CachedEventIds.Remove(dispatchToken);
            }

            // 无人处理事件，检测是否需要抛异常。
            if (noHandlerException && (m_Mode & EventDispatcherMode.AllowNoHandler) == EventDispatcherMode.AllowNoHandler)
            {
                noHandlerException = false;
            }

            if (noHandlerException)
            {
                throw new ErrorCodeException(FrameworkErrorCode.InvalidOperationException, $"Event '{e.Id}' not allow no handler.");
            }

            if (e.IsReleaseAfterFire)
            {
                ReferencePool.Release(e);
            }
        }

        public virtual void Dispose()
        {
            m_EventHandlerDict.Clear();
            m_CachedNodes.Clear();
            m_CachedEventIds.Clear();
            m_TempNodes.Clear();
        }
    }
}
