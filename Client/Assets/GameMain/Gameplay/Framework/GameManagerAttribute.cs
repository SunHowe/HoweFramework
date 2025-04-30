using System;

namespace GameMain
{
    [AttributeUsage(AttributeTargets.Interface)]
    public sealed class GameManagerAttribute : Attribute
    {
        public int ManagerType { get; }

        public GameManagerAttribute(GameManagerType managerType)
        {
            ManagerType = (int)managerType;
        }
    }
}