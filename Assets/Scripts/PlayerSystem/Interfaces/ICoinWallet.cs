namespace Game.PlayerSystem
{
    public interface ICoinWallet
    {
        int Coins { get; }
        bool CanAfford(int amount);
        void Add(int amount);
        bool TrySpend(int amount);
    }
}
