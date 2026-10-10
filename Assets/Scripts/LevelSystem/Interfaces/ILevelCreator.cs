using Cysharp.Threading.Tasks;
using Game.Core.Services;

namespace Game.LevelSystem
{
    public interface ILevelCreator : IInitializable
    {
        UniTask CreateAsync(LevelData data);
    }
}
