using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenuSystem
{
    internal class MainMenuView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _livesButton;

        public void Show(int levelNumber, Action onPlay, Action onSettings, Action onLives)
        {
            _levelText.SetText("Level {0}", levelNumber);
            _playButton.onClick.AddListener(onPlay.Invoke);
            _settingsButton.onClick.AddListener(onSettings.Invoke);
            _livesButton.onClick.AddListener(onLives.Invoke);
        }

        public void SetInteractable(bool isInteractable)
        {
            _playButton.interactable = isInteractable;
        }
    }
}
