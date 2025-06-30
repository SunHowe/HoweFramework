using System;
using HoweFramework;

namespace GameMain
{
    /// <summary>
    /// 随机数管理器。
    /// </summary>
    public sealed class GameRandomManager : GameManagerBase, IGameRandomManager
    {
        private Random m_Random;

        public int GetRandom(int min, int max)
        {
            m_Random ??= new Random();
            return m_Random.Next(min, max);
        }

        public int GetRandom(int max)
        {
            m_Random ??= new Random();
            return m_Random.Next(max);
        }

        public void SetSeed(int seed)
        {
            m_Random = new Random(seed);
        }

        protected override void OnAwake()
        {
        }

        protected override void OnDispose()
        {
            m_Random = null;
        }
    }
}