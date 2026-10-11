using Game.Core.Services;
using Game.Core.Utilities;
using UnityEngine;

namespace Game.PlayerSystem
{
    internal class PlayerDataStore : BaseService, IPlayerLevelProgress
    {
        private const string FileName = "player.json";

        [SerializeField] private PlayerConfig _config;

        public PlayerData Data { get; private set; }
        public int CurrentLevel => Data.CurrentLevel;

        protected override void BindServices()
        {
            ServiceLocator.Bind<IPlayerLevelProgress>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<IPlayerLevelProgress>(this);
        }

        protected override void Initialize()
        {
            Data = new PlayerData { Coins = _config.StartCoins, Lives = _config.MaxLives };
            JsonFileUtility.Load(FileName, Data);
        }

        public void CompleteLevel()
        {
            Data.CurrentLevel++;
            Save();
        }

        public void Save()
        {
            JsonFileUtility.Save(FileName, Data);
        }
    }
}
