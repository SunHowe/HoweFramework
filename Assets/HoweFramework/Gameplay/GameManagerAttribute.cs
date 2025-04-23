using System;

namespace HoweFramework
{
    [AttributeUsage(AttributeTargets.Interface)]
    public sealed class GameManagerAttribute : Attribute
    {
        public int ManagerType { get; }

        public GameManagerAttribute(int managerType)
        {
            ManagerType = managerType;
        }
    }
}