using System.IO;
using Game.Core.Services;
using UnityEngine;

namespace Game.PlayerSystem
{
    internal class PlayerDataStore : BaseService, IPlayerDataStore
    {
        private const string FileName = "player.json";

        private PlayerData _data;

        public int CurrentLevel => _data.CurrentLevel;

        protected override void BindServices()
        {
            ServiceLocator.Bind<IPlayerDataStore>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<IPlayerDataStore>(this);
        }

        public override void Initialize()
        {
            var path = Path.Combine(Application.persistentDataPath, FileName);
            _data = File.Exists(path) ? JsonUtility.FromJson<PlayerData>(File.ReadAllText(path)) : new PlayerData();
        }
    }
}
