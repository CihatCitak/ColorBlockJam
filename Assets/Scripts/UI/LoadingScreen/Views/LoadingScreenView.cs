using Game.Core.Events;
using Game.Events;
using UnityEngine;

namespace Game.UI.LoadingScreen
{
    internal class LoadingScreenView : MonoBehaviour
    {
        [SerializeField] private GameObject _root;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameplayLoadStartedEvent>(OnGameplayLoadStarted);
            EventBus.Subscribe<LevelReadyEvent>(OnLevelReady);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameplayLoadStartedEvent>(OnGameplayLoadStarted);
            EventBus.Unsubscribe<LevelReadyEvent>(OnLevelReady);
        }

        private void OnGameplayLoadStarted(GameplayLoadStartedEvent e)
        {
            _root.SetActive(true);
        }

        private void OnLevelReady(LevelReadyEvent e)
        {
            _root.SetActive(false);
        }
    }
}
