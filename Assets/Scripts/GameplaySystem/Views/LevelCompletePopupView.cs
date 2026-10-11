using System;
using Game.PopupSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GameplaySystem
{
    internal class LevelCompletePopupView : BasePopupView
    {
        [SerializeField] private TMP_Text _rewardText;
        [SerializeField] private TMP_Text _multipliedRewardText;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _multiplyButton;

        private int _reward;
        private int _multipliedReward;
        private Action<int> _onClaim;

        private void Awake()
        {
            _continueButton.onClick.AddListener(() => _onClaim(_reward));
            _multiplyButton.onClick.AddListener(() => _onClaim(_multipliedReward));
        }

        public void Setup(int reward, int multipliedReward, Action<int> onClaim)
        {
            _reward = reward;
            _multipliedReward = multipliedReward;
            _onClaim = onClaim;
            _rewardText.SetText("{0}", reward);
            _multipliedRewardText.SetText("{0}", multipliedReward);
        }
    }
}
