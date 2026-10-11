using System;
using Game.PlayerSystem;
using Game.PopupSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Common
{
    public class SettingsPopupView : BasePopupView
    {
        [SerializeField] private ToggleView _vibrationToggle;
        [SerializeField] private ToggleView _soundToggle;
        [SerializeField] private ToggleView _musicToggle;
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _closeButton;

        private Action _onHome;
        private Action _onClose;

        private void Awake()
        {
            _homeButton.onClick.AddListener(() => _onHome());
            _closeButton.onClick.AddListener(() => _onClose());
        }

        public void Setup(Func<PlayerSetting, bool> isOn, Action<PlayerSetting, bool> onToggled, Action onHome, Action onClose)
        {
            _onHome = onHome;
            _onClose = onClose;
            _vibrationToggle.Bind(isOn(PlayerSetting.Vibration), value => onToggled(PlayerSetting.Vibration, value));
            _soundToggle.Bind(isOn(PlayerSetting.Sound), value => onToggled(PlayerSetting.Sound, value));
            _musicToggle.Bind(isOn(PlayerSetting.Music), value => onToggled(PlayerSetting.Music, value));
        }
    }
}
