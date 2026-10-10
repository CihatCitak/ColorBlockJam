using Cysharp.Threading.Tasks;
using Game.Contracts.Scenes;
using Game.Core.Events;
using Game.Core.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.MainMenu
{
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private Button _playButton;

        private void OnEnable()
        {
            _playButton.onClick.AddListener(OnPlayClicked);
            EventBus.Subscribe<GameSceneLoadStartedEvent>(OnGameSceneLoadStarted);
            EventBus.Subscribe<GameSceneLoadedEvent>(OnGameSceneLoaded);
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveListener(OnPlayClicked);
            EventBus.Unsubscribe<GameSceneLoadStartedEvent>(OnGameSceneLoadStarted);
            EventBus.Unsubscribe<GameSceneLoadedEvent>(OnGameSceneLoaded);
        }

        private void OnPlayClicked()
        {
            ServiceLocator.Get<ISceneService>().LoadGameAsync().Forget();
        }

        private void OnGameSceneLoadStarted(GameSceneLoadStartedEvent e)
        {
            _playButton.interactable = false;
        }

        private void OnGameSceneLoaded(GameSceneLoadedEvent e)
        {
            _root.SetActive(false);
        }
    }
}
