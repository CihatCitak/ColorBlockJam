using System;

namespace Game.PlayerSystem
{
    public interface ILives
    {
        int Count { get; }
        bool IsFull { get; }
        TimeSpan TimeToNextLife { get; }
        void LoseLife();
    }
}
