using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core.Services;
using Game.PlayerSystem;
using TMPro;
using UnityEngine;

namespace Game.UI.Common
{
    internal class LivesView : MonoBehaviour
    {
        private static readonly TimeSpan TickInterval = TimeSpan.FromSeconds(1);

        [SerializeField] private TMP_Text _countText;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private string _fullText = "Full";

        private ILives _lives;
        private CancellationTokenSource _ticking;

        private void OnEnable()
        {
            _lives = ServiceLocator.Get<ILives>();
            _ticking = new CancellationTokenSource();
            TickAsync(_ticking.Token).Forget();
        }

        private void OnDisable()
        {
            _ticking.Cancel();
            _ticking.Dispose();
        }

        private async UniTaskVoid TickAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Refresh();
                await UniTask.Delay(TickInterval, cancellationToken: token).SuppressCancellationThrow();
            }
        }

        private void Refresh()
        {
            _countText.SetText("{0}", _lives.Count);
            if (_lives.IsFull)
                _timerText.SetText(_fullText);
            else
                TimeTextUtility.SetMinutesSeconds(_timerText, (int)_lives.TimeToNextLife.TotalSeconds);
        }
    }
}
