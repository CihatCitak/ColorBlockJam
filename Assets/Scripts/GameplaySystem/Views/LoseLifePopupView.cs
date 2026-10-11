using System;
using Game.PopupSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GameplaySystem
{
    internal class LoseLifePopupView : BasePopupView
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _closeButton;

        private Action _onConfirm;
        private Action _onClose;

        private void Awake()
        {
            _retryButton.onClick.AddListener(() => _onConfirm());
            _homeButton.onClick.AddListener(() => _onConfirm());
            _closeButton.onClick.AddListener(() => _onClose());
        }

        public void SetupRetry(int levelNumber, Action onRetry, Action onClose)
        {
            Setup(levelNumber, true, onRetry, onClose);
        }

        public void SetupHome(int levelNumber, Action onHome, Action onClose)
        {
            Setup(levelNumber, false, onHome, onClose);
        }

        private void Setup(int levelNumber, bool isRetry, Action onConfirm, Action onClose)
        {
            _onConfirm = onConfirm;
            _onClose = onClose;
            _titleText.SetText("Level {0}", levelNumber);
            _retryButton.gameObject.SetActive(isRetry);
            _homeButton.gameObject.SetActive(!isRetry);
        }
    }
}
