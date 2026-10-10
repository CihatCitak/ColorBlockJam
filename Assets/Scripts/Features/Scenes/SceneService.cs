using Cysharp.Threading.Tasks;
using Game.Contracts.Scenes;
using Game.Core.Events;
using Game.Core.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scenes
{
    public class SceneService : BaseService, ISceneService
    {
        [SerializeField] private string _mainSceneName = "Main";
        [SerializeField] private string _gameSceneName = "Game";

        private bool _isLoadingGame;

        protected override void BindServices()
        {
            ServiceLocator.Bind<ISceneService>(this);
        }

        protected override void UnbindServices()
        {
            ServiceLocator.Unbind<ISceneService>(this);
        }

        public override void Initialize()
        {
            SceneManager.LoadSceneAsync(_mainSceneName).ToUniTask().Forget();
        }

        public async UniTask LoadGameAsync()
        {
            if (_isLoadingGame || SceneManager.GetSceneByName(_gameSceneName).isLoaded)
                return;

            _isLoadingGame = true;
            EventBus.Fire(new GameSceneLoadStartedEvent());

            await SceneManager.LoadSceneAsync(_gameSceneName, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_gameSceneName));

            _isLoadingGame = false;
            EventBus.Fire(new GameSceneLoadedEvent());
        }
    }
}
