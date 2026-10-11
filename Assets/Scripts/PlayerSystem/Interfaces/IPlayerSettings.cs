namespace Game.PlayerSystem
{
    public interface IPlayerSettings
    {
        bool IsOn(PlayerSetting setting);
        void Set(PlayerSetting setting, bool isOn);
    }
}
