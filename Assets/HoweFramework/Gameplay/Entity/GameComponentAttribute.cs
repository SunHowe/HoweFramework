using System;

namespace HoweFramework
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class GameComponentAttribute : Attribute
    {
        public int ComponentType { get; }

        public GameComponentAttribute(int componentType)
        {
            ComponentType = componentType;
        }
    }
}
