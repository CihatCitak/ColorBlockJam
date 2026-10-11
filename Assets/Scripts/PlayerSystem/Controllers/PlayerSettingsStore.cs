using Game.Core.Services;
using Game.Core.Utilities;

namespace Game.PlayerSystem
{
    internal class PlayerSettingsStore : BaseService, IPlayerSettings
    {
        private const string FileName = "settings.json";

        private readonly PlayerSettingsData _data = new();

        protected override void BindServices()
        {
            ServiceLocator.Bind<IPlayerSettings>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<IPlayerSettings>(this);
        }

        protected override void Initialize()
        {
            JsonFileUtility.Load(FileName, _data);
        }

        public bool IsOn(PlayerSetting setting)
        {
            return setting switch
            {
                PlayerSetting.Vibration => _data.Vibration,
                PlayerSetting.Sound => _data.Sound,
                _ => _data.Music
            };
        }

        public void Set(PlayerSetting setting, bool isOn)
        {
            switch (setting)
            {
                case PlayerSetting.Vibration:
                    _data.Vibration = isOn;
                    break;
                case PlayerSetting.Sound:
                    _data.Sound = isOn;
                    break;
                default:
                    _data.Music = isOn;
                    break;
            }

            JsonFileUtility.Save(FileName, _data);
        }
    }
}
