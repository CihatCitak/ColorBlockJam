using System.IO;
using Game.Core.Services;
using UnityEngine;

namespace Game.PlayerSystem
{
    internal class PlayerDataStore : BaseService, IPlayerDataStore
    {
        private const string FileName = "player.json";

        private PlayerData _data;
        private string _path;

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
            _path = Path.Combine(Application.persistentDataPath, FileName);
            _data = File.Exists(_path) ? JsonUtility.FromJson<PlayerData>(File.ReadAllText(_path)) : new PlayerData();
        }

        public void CompleteLevel()
        {
            _data.CurrentLevel++;
            File.WriteAllText(_path, JsonUtility.ToJson(_data));
        }
    }
}
