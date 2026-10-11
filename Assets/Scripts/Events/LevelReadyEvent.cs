using Game.Core.Events;

namespace Game.Events
{
    public readonly struct LevelReadyEvent : IEvent
    {
        public readonly int LevelNumber;
        public readonly int BlockCount;
        public readonly int TimeLimit;

        public LevelReadyEvent(int levelNumber, int blockCount, int timeLimit)
        {
            LevelNumber = levelNumber;
            BlockCount = blockCount;
            TimeLimit = timeLimit;
        }
    }
}
