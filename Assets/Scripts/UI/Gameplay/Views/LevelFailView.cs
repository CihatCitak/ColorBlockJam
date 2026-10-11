using Cysharp.Threading.Tasks;
using Game.Core.Events;
using Game.Core.Services;
using Game.Events;
using Game.SceneSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Gameplay
{
    internal class LevelFailView : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private Button _retryButton;

        private void OnEnable()
        {
            EventBus.Subscribe<LevelFailedEvent>(OnLevelFailed);
            _retryButton.onClick.AddListener(OnRetryClicked);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<LevelFailedEvent>(OnLevelFailed);
            _retryButton.onClick.RemoveListener(OnRetryClicked);
        }

        private void OnLevelFailed(LevelFailedEvent e)
        {
            _root.SetActive(true);
        }

        private void OnRetryClicked()
        {
            _retryButton.interactable = false;
            ServiceLocator.Get<ISceneLoader>().LoadGameplayAsync().Forget();
        }
    }
}
