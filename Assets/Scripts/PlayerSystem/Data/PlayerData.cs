using System;

namespace Game.PlayerSystem
{
    [Serializable]
    internal class PlayerData
    {
        public int CurrentLevel = 1;
        public int Coins;
        public int Lives;
        public long LifeRefillStartTicks;
    }
}
