using System;
using DG.Tweening;
using Game.UI.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GameplaySystem
{
    internal class GameplayHudView : MonoBehaviour
    {
        [SerializeField] private HudConfig _config;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private Image _timeWarning;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private CanvasGroup[] _playControls;

        private Tween _warningFlash;
        private int _pulseFromSeconds;

        public void Show(int levelNumber, int timeLimit, Action onRestart, Action onPause)
        {
            _levelText.SetText("{0}", levelNumber);
            _pulseFromSeconds = Mathf.FloorToInt(timeLimit * _config.PulseFromTimeRatio);
            _restartButton.onClick.AddListener(onRestart.Invoke);
            _pauseButton.onClick.AddListener(onPause.Invoke);
        }

        public void SetTime(int seconds)
        {
            TimeTextUtility.SetMinutesSeconds(_timeText, seconds);

            var hasTimeLeft = seconds > 0;
            if (hasTimeLeft && seconds <= _pulseFromSeconds)
                PulseTime();

            SetWarning(hasTimeLeft && seconds <= _config.WarningFromSeconds);
        }

        public void SetInteractable(bool isInteractable)
        {
            _canvasGroup.interactable = isInteractable;
        }

        public void SetPlayControlsInteractable(bool isInteractable)
        {
            foreach (var control in _playControls)
            {
                control.interactable = isInteractable;
                control.alpha = isInteractable ? 1f : _config.DisabledControlAlpha;
            }
        }

        private void PulseTime()
        {
            var time = _timeText.transform;
            time.DOKill();
            time.localScale = Vector3.one;
            time.DOScale(_config.PulseScale, _config.PulseDuration * 0.5f)
                .SetLoops(2, LoopType.Yoyo)
                .SetLink(gameObject);
        }

        private void SetWarning(bool isOn)
        {
            if (isOn == (_warningFlash != null))
                return;

            if (!isOn)
            {
                _warningFlash.Kill();
                _warningFlash = null;
                SetWarningAlpha(0f);
                return;
            }

            _warningFlash = DOTween.To(() => _timeWarning.color.a, SetWarningAlpha, _config.WarningAlpha, _config.WarningFlashDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(gameObject);
        }

        private void SetWarningAlpha(float alpha)
        {
            var color = _timeWarning.color;
            color.a = alpha;
            _timeWarning.color = color;
        }
    }
}
