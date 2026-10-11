using Game.Core.Events;
using Game.Core.Services;
using Game.Events;
using UnityEngine;

namespace Game.PlayerSystem
{
    internal class WalletController : BaseService, ICoinWallet
    {
        [SerializeField] private PlayerDataStore _store;

        public int Coins => _store.Data.Coins;

        protected override void BindServices()
        {
            ServiceLocator.Bind<ICoinWallet>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<ICoinWallet>(this);
        }

        public bool CanAfford(int amount)
        {
            return Coins >= amount;
        }

        public void Add(int amount)
        {
            ChangeCoins(amount);
        }

        public bool TrySpend(int amount)
        {
            if (!CanAfford(amount))
                return false;

            ChangeCoins(-amount);
            return true;
        }

        private void ChangeCoins(int amount)
        {
            _store.Data.Coins += amount;
            _store.Save();
            EventBus.Fire(new CoinsChangedEvent(_store.Data.Coins));
        }
    }
}
