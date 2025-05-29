using System;
using System.Collections.Generic;

namespace HoweFramework
{
    /// <summary>
    /// 事件处理函数。
    /// </summary>
    public delegate void SimpleEventHandler();

    /// <summary>
    /// 事件处理函数。
    /// </summary>
    /// <typeparam name="T">事件参数类型。</typeparam>
    /// <param name="value">事件参数。</param>
    public delegate void SimpleEventHandler<T>(T value);

    /// <summary>
    /// 简单事件派发器，用于局部单一事件的派发。
    /// </summary>
    public sealed class SimpleEvent : IReference, IDisposable
    {
        /// <summary>
        /// 事件处理函数列表。
        /// </summary>
        private readonly LinkedListEx<SimpleEventHandler> m_EventHandlerList = new();

        /// <summary>
        /// 链表节点缓存。
        /// </summary>
        private readonly LinkedListEx<Variable<LinkedListNode<SimpleEventHandler>>> m_EventHandlerCache = new();

        /// <summary>
        /// 订阅事件。
        /// </summary>
        /// <param name="handler">事件处理函数。</param>
        public void Subscribe(SimpleEventHandler handler)
        {
            m_EventHandlerList.AddLast(handler);
        }

        /// <summary>
        /// 取消订阅事件。
        /// </summary>
        /// <param name="handler">事件处理函数。</param>
        public void Unsubscribe(SimpleEventHandler handler)
        {
            if (m_EventHandlerCache.Count > 0)
            {
                var node = m_EventHandlerCache.First;

                while (node != null)
                {
                    var variable = node.Value;
                    var originNode = variable.Value;
                    if (originNode.Value == handler)
                    {
                        variable.Value = originNode.Next;
                    }

                    node = node.Next;
                }
            }

            m_EventHandlerList.Remove(handler);
        }

        /// <summary>
        /// 派发事件。
        /// </summary>
        public void Dispatch()
        {
            var current = m_EventHandlerList.First;
            if (current == null)
            {
                return;
            }

            var variable = Variable<LinkedListNode<SimpleEventHandler>>.Create(current);
            m_EventHandlerCache.AddLast(variable);

            while (current != null)
            {
                // 缓存下一个节点。
                variable.Value = current.Next;

                try
                {
                    current.Value();
                }
                catch (Exception ex)
                {
                    Log.Error($"SimpleEvent Dispatch error: {ex.Message}\n{ex.StackTrace}");
                }

                current = variable.Value;
            }

            m_EventHandlerCache.Remove(variable);
            variable.Dispose();
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public void Clear()
        {
            m_EventHandlerList.Clear();
            m_EventHandlerCache.Clear();
        }

        /// <summary>
        /// 创建简单事件派发器。
        /// </summary>
        public static SimpleEvent Create()
        {
            return ReferencePool.Acquire<SimpleEvent>();
        }
    }

    /// <summary>
    /// 简单事件派发器，用于局部单一事件的派发。
    /// </summary>
    public sealed class SimpleEvent<T> : IReference, IDisposable
    {
        /// <summary>
        /// 事件处理函数列表。
        /// </summary>
        private readonly LinkedListEx<SimpleEventHandler<T>> m_EventHandlerList = new();

        /// <summary>
        /// 链表节点缓存。
        /// </summary>
        private readonly LinkedListEx<Variable<LinkedListNode<SimpleEventHandler<T>>>> m_EventHandlerCache = new();

        /// <summary>
        /// 订阅事件。
        /// </summary>
        /// <param name="handler">事件处理函数。</param>
        public void Subscribe(SimpleEventHandler<T> handler)
        {
            m_EventHandlerList.AddLast(handler);
        }

        /// <summary>
        /// 取消订阅事件。
        /// </summary>
        /// <param name="handler">事件处理函数。</param>
        public void Unsubscribe(SimpleEventHandler<T> handler)
        {
            if (m_EventHandlerCache.Count > 0)
            {
                var node = m_EventHandlerCache.First;

                while (node != null)
                {
                    var variable = node.Value;
                    var originNode = variable.Value;
                    if (originNode.Value == handler)
                    {
                        variable.Value = originNode.Next;
                    }

                    node = node.Next;
                }
            }

            m_EventHandlerList.Remove(handler);
        }

        /// <summary>
        /// 派发事件。
        /// </summary>
        /// <param name="value">事件参数。</param>
        public void Dispatch(T value)
        {
            var current = m_EventHandlerList.First;
            if (current == null)
            {
                return;
            }

            var variable = Variable<LinkedListNode<SimpleEventHandler<T>>>.Create(current);
            m_EventHandlerCache.AddLast(variable);

            while (current != null)
            {
                // 缓存下一个节点。
                variable.Value = current.Next;

                try
                {
                    current.Value(value);
                }
                catch (Exception ex)
                {
                    Log.Error($"SimpleEvent<{typeof(T)}> Dispatch error: {ex.Message}\n{ex.StackTrace}");
                }

                current = variable.Value;
            }

            m_EventHandlerCache.Remove(variable);
            variable.Dispose();
        }

        public void Dispose()
        {
            ReferencePool.Release(this);
        }

        public void Clear()
        {
            m_EventHandlerList.Clear();
            m_EventHandlerCache.Clear();
        }

        /// <summary>
        /// 创建简单事件派发器。
        /// </summary>
        public static SimpleEvent<T> Create()
        {
            return ReferencePool.Acquire<SimpleEvent<T>>();
        }
    }
}
