using Cysharp.Threading.Tasks;

namespace Game.Contracts.Scenes
{
    public interface ISceneService
    {
        UniTask LoadGameAsync();
    }
}
