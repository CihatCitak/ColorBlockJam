using System;
using Game.PopupSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GameplaySystem
{
    internal class OutOfTimePopupView : BasePopupView
    {
        [SerializeField] private TMP_Text _bonusText;
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private TMP_Text _costText;
        [SerializeField] private TMP_Text _adBonusText;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _watchAdButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Color _costColor = Color.white;
        [SerializeField] private Color _unaffordableCostColor = Color.red;

        private Action _onContinue;
        private Action _onWatchAd;
        private Action _onClose;

        private void Awake()
        {
            _continueButton.onClick.AddListener(() => _onContinue());
            _watchAdButton.onClick.AddListener(() => _onWatchAd());
            _closeButton.onClick.AddListener(() => _onClose());
        }

        public void Setup(int seconds, int cost, bool canAfford, Action onContinue, Action onWatchAd, Action onClose)
        {
            _onContinue = onContinue;
            _onWatchAd = onWatchAd;
            _onClose = onClose;
            _bonusText.SetText("+{0}", seconds);
            _messageText.SetText("Get {0} seconds to keep playing!", seconds);
            _costText.SetText("{0}", cost);
            _costText.color = canAfford ? _costColor : _unaffordableCostColor;
            _adBonusText.SetText("+{0} sec", seconds);
        }
    }
}
