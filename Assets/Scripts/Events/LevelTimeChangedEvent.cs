using Game.Core.Events;

namespace Game.Events
{
    public readonly struct LevelTimeChangedEvent : IEvent
    {
        public readonly int SecondsLeft;

        public LevelTimeChangedEvent(int secondsLeft)
        {
            SecondsLeft = secondsLeft;
        }
    }
}
