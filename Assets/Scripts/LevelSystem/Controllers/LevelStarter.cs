using Cysharp.Threading.Tasks;
using Game.Core.Events;
using Game.Core.Services;
using Game.Events;
using Game.PlayerSystem;
using UnityEngine;

namespace Game.LevelSystem
{
    internal class LevelStarter : MonoBehaviour
    {
        private void Start()
        {
            StartLevelAsync().Forget();
        }

        private async UniTaskVoid StartLevelAsync()
        {
            var loader = ServiceLocator.Get<ILevelLoader>();
            var creator = ServiceLocator.Get<ILevelCreator>();

            await UniTask.WaitUntil(() => loader.IsInitialized && creator.IsInitialized);

            var levelNumber = ServiceLocator.Get<IPlayerDataStore>().CurrentLevel;
            var data = await loader.LoadAsync(levelNumber);
            await creator.CreateAsync(data);

            EventBus.Fire(new LevelReadyEvent(levelNumber, data.Blocks.Count, data.TimeLimit));
        }
    }
}
