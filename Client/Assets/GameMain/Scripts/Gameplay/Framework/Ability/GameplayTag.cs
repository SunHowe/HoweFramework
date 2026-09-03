using System;

namespace GameMain
{
    /// <summary>
    /// 层次化玩法标签，由 <see cref="GameplayTagRegistry"/> 分配 id。
    /// </summary>
    public readonly struct GameplayTag : IEquatable<GameplayTag>
    {
        public static GameplayTag Invalid => default;

        public int Id { get; }

        public GameplayTag(int id)
        {
            Id = id;
        }

        public bool IsValid => Id != 0;

        public bool Equals(GameplayTag other) => Id == other.Id;

        public override bool Equals(object obj) => obj is GameplayTag other && Equals(other);

        public override int GetHashCode() => Id;

        public static bool operator ==(GameplayTag left, GameplayTag right) => left.Equals(right);

        public static bool operator !=(GameplayTag left, GameplayTag right) => !left.Equals(right);

        public override string ToString() => IsValid ? $"Tag({Id})" : "Tag(Invalid)";
    }
}
