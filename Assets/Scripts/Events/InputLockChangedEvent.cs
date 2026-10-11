using Game.Core.Events;

namespace Game.Events
{
    public readonly struct InputLockChangedEvent : IEvent
    {
        public readonly bool IsLocked;

        public InputLockChangedEvent(bool isLocked)
        {
            IsLocked = isLocked;
        }
    }
}
