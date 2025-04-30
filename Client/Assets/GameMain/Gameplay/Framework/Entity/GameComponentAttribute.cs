using System;

namespace GameMain
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class GameComponentAttribute : Attribute
    {
        public int ComponentType { get; }

        public GameComponentAttribute(GameComponentType componentType)
        {
            ComponentType = (int)componentType;
        }
    }
}
