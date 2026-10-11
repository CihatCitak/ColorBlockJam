namespace Game.PlayerSystem
{
    public interface IPlayerLevelProgress
    {
        int CurrentLevel { get; }
        void CompleteLevel();
    }
}
