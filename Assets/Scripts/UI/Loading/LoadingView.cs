using Game.Contracts.Scenes;
using Game.Core.Events;
using UnityEngine;

namespace Game.UI.Loading
{
    public class LoadingView : MonoBehaviour
    {
        [SerializeField] private GameObject _root;

        private void OnEnable()
        {
            EventBus.Subscribe<GameSceneLoadStartedEvent>(OnGameSceneLoadStarted);
            EventBus.Subscribe<GameSceneLoadedEvent>(OnGameSceneLoaded);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameSceneLoadStartedEvent>(OnGameSceneLoadStarted);
            EventBus.Unsubscribe<GameSceneLoadedEvent>(OnGameSceneLoaded);
        }

        private void OnGameSceneLoadStarted(GameSceneLoadStartedEvent e)
        {
            _root.SetActive(true);
        }

        private void OnGameSceneLoaded(GameSceneLoadedEvent e)
        {
            _root.SetActive(false);
        }
    }
}
