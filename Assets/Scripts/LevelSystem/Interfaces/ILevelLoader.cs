using Cysharp.Threading.Tasks;
using Game.Core.Services;

namespace Game.LevelSystem
{
    public interface ILevelLoader : IInitializable
    {
        UniTask<LevelData> LoadAsync(int levelNumber);
    }
}
