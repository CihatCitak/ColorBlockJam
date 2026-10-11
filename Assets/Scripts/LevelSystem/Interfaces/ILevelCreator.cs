using Game.Core.Services;

namespace Game.LevelSystem
{
    public interface ILevelCreator : IInitializable
    {
        void Create(LevelData data);
    }
}
