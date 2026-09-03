using System.Collections.Generic;

namespace GameMain
{
    /// <summary>
    /// 带引用计数的标签集合，支持层次化 <see cref="HasTag"/> 查询。
    /// </summary>
    public sealed class GameplayTagContainer
    {
        private readonly Dictionary<int, int> m_Counts = new Dictionary<int, int>();

        public int ExactCount => m_Counts.Count;

        public void Clear()
        {
            m_Counts.Clear();
        }

        /// <summary>
        /// 为精确标签增加一层引用。返回新的计数。
        /// </summary>
        public int AddTag(GameplayTag tag)
        {
            if (!tag.IsValid)
            {
                return 0;
            }

            m_Counts.TryGetValue(tag.Id, out var count);
            count++;
            m_Counts[tag.Id] = count;
            return count;
        }

        /// <summary>
        /// 为精确标签减少一层引用。返回剩余计数（移除后为 0）。
        /// </summary>
        public int RemoveTag(GameplayTag tag)
        {
            if (!tag.IsValid || !m_Counts.TryGetValue(tag.Id, out var count))
            {
                return 0;
            }

            count--;
            if (count <= 0)
            {
                m_Counts.Remove(tag.Id);
                return 0;
            }

            m_Counts[tag.Id] = count;
            return count;
        }

        public bool HasExact(GameplayTag tag)
        {
            return tag.IsValid && m_Counts.TryGetValue(tag.Id, out var count) && count > 0;
        }

        /// <summary>
        /// 任一已拥有的精确标签匹配 <paramref name="tag"/>（自身或子孙）时为 true。
        /// </summary>
        public bool HasTag(GameplayTag tag, GameplayTagRegistry registry)
        {
            if (registry == null || !tag.IsValid)
            {
                return false;
            }

            if (HasExact(tag))
            {
                return true;
            }

            foreach (var pair in m_Counts)
            {
                if (pair.Value > 0 && registry.Matches(new GameplayTag(pair.Key), tag))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasAll(IReadOnlyList<GameplayTag> tags, GameplayTagRegistry registry)
        {
            if (tags == null)
            {
                return true;
            }

            for (var i = 0; i < tags.Count; i++)
            {
                if (!HasTag(tags[i], registry))
                {
                    return false;
                }
            }

            return true;
        }

        public bool HasAny(IReadOnlyList<GameplayTag> tags, GameplayTagRegistry registry)
        {
            if (tags == null || tags.Count == 0)
            {
                return false;
            }

            for (var i = 0; i < tags.Count; i++)
            {
                if (HasTag(tags[i], registry))
                {
                    return true;
                }
            }

            return false;
        }

        public void AddTags(IReadOnlyList<GameplayTag> tags)
        {
            if (tags == null)
            {
                return;
            }

            for (var i = 0; i < tags.Count; i++)
            {
                AddTag(tags[i]);
            }
        }

        public void RemoveTags(IReadOnlyList<GameplayTag> tags)
        {
            if (tags == null)
            {
                return;
            }

            for (var i = 0; i < tags.Count; i++)
            {
                RemoveTag(tags[i]);
            }
        }
    }
}
