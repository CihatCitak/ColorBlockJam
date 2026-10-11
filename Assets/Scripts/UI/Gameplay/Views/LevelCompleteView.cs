using Cysharp.Threading.Tasks;
using Game.Core.Events;
using Game.Core.Services;
using Game.Events;
using Game.SceneSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Gameplay
{
    internal class LevelCompleteView : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private Button _continueButton;

        private void OnEnable()
        {
            EventBus.Subscribe<LevelCompletedEvent>(OnLevelCompleted);
            _continueButton.onClick.AddListener(OnContinueClicked);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<LevelCompletedEvent>(OnLevelCompleted);
            _continueButton.onClick.RemoveListener(OnContinueClicked);
        }

        private void OnLevelCompleted(LevelCompletedEvent e)
        {
            _root.SetActive(true);
        }

        private void OnContinueClicked()
        {
            _continueButton.interactable = false;
            ServiceLocator.Get<ISceneLoader>().LoadMainAsync().Forget();
        }
    }
}
