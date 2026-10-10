using Cysharp.Threading.Tasks;

namespace Game.SceneSystem
{
    public interface ISceneLoader
    {
        UniTask LoadGameplayAsync();
    }
}
