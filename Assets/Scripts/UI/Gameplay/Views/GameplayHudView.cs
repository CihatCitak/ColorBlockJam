using Game.Core.Events;
using Game.Events;
using TMPro;
using UnityEngine;

namespace Game.UI.Gameplay
{
    internal class GameplayHudView : MonoBehaviour
    {
        private const int SecondsPerMinute = 60;

        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _timeText;

        private void OnEnable()
        {
            EventBus.Subscribe<LevelReadyEvent>(OnLevelReady);
            EventBus.Subscribe<LevelTimeChangedEvent>(OnLevelTimeChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<LevelReadyEvent>(OnLevelReady);
            EventBus.Unsubscribe<LevelTimeChangedEvent>(OnLevelTimeChanged);
        }

        private void OnLevelReady(LevelReadyEvent e)
        {
            _levelText.SetText("Level {0}", e.LevelNumber);
        }

        private void OnLevelTimeChanged(LevelTimeChangedEvent e)
        {
            _timeText.SetText("{0:00}:{1:00}", e.SecondsLeft / SecondsPerMinute, e.SecondsLeft % SecondsPerMinute);
        }
    }
}
