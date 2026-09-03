using System;
using System.Collections.Generic;

namespace GameMain
{
    /// <summary>
    /// 将点分标签名（如 <c>State.Debuff.Stun</c>）实习为稳定 id，并自动注册父节点。
    /// </summary>
    public sealed class GameplayTagRegistry
    {
        private readonly Dictionary<string, int> m_NameToId = new Dictionary<string, int>(StringComparer.Ordinal);
        private readonly Dictionary<int, string> m_IdToName = new Dictionary<int, string>();
        private readonly Dictionary<int, int> m_ParentById = new Dictionary<int, int>();
        private int m_NextId = 1;

        /// <summary>
        /// 按点分名注册或解析标签。父路径会一并注册。
        /// </summary>
        public GameplayTag Request(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Tag name must contain at least one segment.", nameof(name));
            }

            if (m_NameToId.TryGetValue(name, out var existing))
            {
                return new GameplayTag(existing);
            }

            var parts = name.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                throw new ArgumentException("Tag name must contain at least one segment.", nameof(name));
            }

            var path = string.Empty;
            var parentId = 0;
            var result = GameplayTag.Invalid;

            for (var i = 0; i < parts.Length; i++)
            {
                var segment = parts[i].Trim();
                if (segment.Length == 0)
                {
                    continue;
                }

                path = i == 0 ? segment : path + "." + segment;
                if (!m_NameToId.TryGetValue(path, out var id))
                {
                    id = m_NextId++;
                    m_NameToId[path] = id;
                    m_IdToName[id] = path;
                    if (parentId != 0)
                    {
                        m_ParentById[id] = parentId;
                    }
                }

                parentId = id;
                result = new GameplayTag(id);
            }

            if (!result.IsValid)
            {
                throw new ArgumentException("Tag name must contain at least one segment.", nameof(name));
            }

            return result;
        }

        public bool TryGet(string name, out GameplayTag tag)
        {
            if (name != null && m_NameToId.TryGetValue(name, out var id))
            {
                tag = new GameplayTag(id);
                return true;
            }

            tag = GameplayTag.Invalid;
            return false;
        }

        public string GetName(GameplayTag tag)
        {
            return tag.IsValid && m_IdToName.TryGetValue(tag.Id, out var name) ? name : string.Empty;
        }

        public GameplayTag GetParent(GameplayTag tag)
        {
            if (tag.IsValid && m_ParentById.TryGetValue(tag.Id, out var parentId))
            {
                return new GameplayTag(parentId);
            }

            return GameplayTag.Invalid;
        }

        /// <summary>
        /// 当 <paramref name="tag"/> 等于 <paramref name="parentOrSelf"/> 或其子孙时为 true。
        /// </summary>
        public bool Matches(GameplayTag tag, GameplayTag parentOrSelf)
        {
            if (!tag.IsValid || !parentOrSelf.IsValid)
            {
                return false;
            }

            var current = tag;
            while (current.IsValid)
            {
                if (current == parentOrSelf)
                {
                    return true;
                }

                current = GetParent(current);
            }

            return false;
        }

        public void Clear()
        {
            m_NameToId.Clear();
            m_IdToName.Clear();
            m_ParentById.Clear();
            m_NextId = 1;
        }
    }
}
