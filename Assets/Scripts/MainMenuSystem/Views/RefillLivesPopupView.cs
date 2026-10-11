using System;
using Game.PopupSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenuSystem
{
    internal class RefillLivesPopupView : BasePopupView
    {
        [SerializeField] private TMP_Text _adLivesText;
        [SerializeField] private TMP_Text _costText;
        [SerializeField] private Color _costColor = Color.white;
        [SerializeField] private Color _unaffordableCostColor = Color.red;
        [SerializeField] private Button _adButton;
        [SerializeField] private Button _refillButton;
        [SerializeField] private Button _closeButton;

        private Action _onAd;
        private Action _onRefill;
        private Action _onClose;

        private void Awake()
        {
            _adButton.onClick.AddListener(() => _onAd());
            _refillButton.onClick.AddListener(() => _onRefill());
            _closeButton.onClick.AddListener(() => _onClose());
        }

        public void Setup(int adLives, int cost, bool canAfford, Action onAd, Action onRefill, Action onClose)
        {
            _onAd = onAd;
            _onRefill = onRefill;
            _onClose = onClose;
            _adLivesText.SetText("+{0}", adLives);
            _costText.SetText("{0}", cost);
            _costText.color = canAfford ? _costColor : _unaffordableCostColor;
        }
    }
}
