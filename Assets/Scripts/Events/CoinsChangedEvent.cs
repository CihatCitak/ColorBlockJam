using Game.Core.Events;

namespace Game.Events
{
    public readonly struct CoinsChangedEvent : IEvent
    {
        public readonly int Coins;

        public CoinsChangedEvent(int coins)
        {
            Coins = coins;
        }
    }
}
